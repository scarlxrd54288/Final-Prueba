using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private GridData carTrafficData;
    private Grid grid;
    private Vector3Int currentCell;

    private Vector3 direction;
    private float speed;
    private float damage;
    private int typeIndex;
    private CarPoolManager poolManager;
    private Obstacle currentObstacle;

    public bool isStopped;
    private bool isInitialized = false;

    private float spawnGraceTime = 0.1f;
    private float spawnTimer = 0f;

    public event Action OnCarRemoved;

    [SerializeField] private float maxHealth = 10f;
    private float currentHealth;

    [SerializeField] private LayerMask detectionMask;

    private Animator animator;
    private bool isPushingObstacle = false;


    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void Initialize(Vector3 direction, float speed, float damagePerSecond, int typeIndex, CarPoolManager poolManager, GridData carTrafficData, Grid grid)
    {
        this.direction = new Vector3(direction.x, 0f, direction.z).normalized;
        this.speed = speed;
        this.damage = damagePerSecond;
        this.typeIndex = typeIndex;
        this.poolManager = poolManager;
        this.carTrafficData = carTrafficData;
        this.grid = grid;

        currentHealth = maxHealth;

        isStopped = false;
        currentObstacle = null;
        spawnTimer = spawnGraceTime;

        currentCell = grid.WorldToCell(transform.position);

        if (!carTrafficData.HasObjectAt(currentCell))
        {
            CarDataSO carData = poolManager.GetCarData(typeIndex);
            carTrafficData.AddObjectAt(currentCell, carData.size, -1, typeIndex, GridObjectType.Car);

        }
        else
        {
            Debug.LogWarning($"[CarController] Spawn en celda ocupada: {currentCell}");
        }

        isInitialized = true;
    }

    void Update()
    {
        Debug.DrawRay(transform.position, direction * 0.6f, Color.red);
        //Debug.Log($"isStopped: {isStopped}, isPushingObstacle: {isPushingObstacle}");

        if (!isInitialized) return;

        if (spawnTimer > 0f)
        {
            spawnTimer -= Time.deltaTime;
        }

        if (isStopped)
        {
            TryResumeMovement();
        }

        // NO pongas shouldPush aquí todavía
        bool shouldPush = false;

        if (!isStopped)
        {
            Move(); // <-- Aquí es donde puede cambiar isStopped

            // Vuelve a evaluar raycast ofensivo por seguridad
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 0.6f, detectionMask))
            {
                Obstacle obs = hit.collider.GetComponent<Obstacle>();
                if (obs != null && obs.IsOffensive())
                {
                    obs.ApplyDamageToCar(this);
                    shouldPush = true;
                }
            }
        }

        // Si está detenido, puede atacar
        if (isStopped)
        {
            // Solo activa animación si hay obstáculo válido ofensivo o no-ofensivo
            if (currentObstacle != null)
            {
                shouldPush = true;

                if (!currentObstacle.IsOffensive())
                {
                    Attack();
                }
            }
            else
            {
                // choca con otro auto: no se empuja
                shouldPush = false;
            }
        }


        SetPushingAnimation(shouldPush);
        CheckOutOfBounds();
    }


    void Move()
    {
        Vector3 targetPosition = transform.position + direction * speed * Time.deltaTime;
        Vector3Int nextCell = grid.WorldToCell(targetPosition);

        if (spawnTimer <= 0f)
        {
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 0.7f, detectionMask))
            {
                Debug.Log($"Raycast hit: {hit.collider.name}, Tag: {hit.collider.tag}");

                //if (hit.collider.CompareTag("Obstacle"))
                if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("eObstacle"))
                {
                    Debug.Log("Detenido por obstáculo");
                    currentObstacle = hit.collider.GetComponent<Obstacle>();
                    isStopped = true;

                    if (currentObstacle != null && currentObstacle.IsOffensive())
                    {
                        currentObstacle.ApplyDamageToCar(this);
                    }

                    AudioManager.Instance.PlayCrashSound();
                    return;
                }

                if (hit.collider.CompareTag("Car") && hit.collider.gameObject != this.gameObject)
                {
                    isStopped = true;
                    currentObstacle = null;
                    AudioManager.Instance.PlayCrashSound();
                    return;
                }
            }
        }

        if (nextCell != currentCell)
        {
            transform.position = targetPosition;
            CarDataSO carData = poolManager.GetCarData(typeIndex);
            carTrafficData.RemoveObjectAt(currentCell);
            carTrafficData.AddObjectAt(nextCell, carData.size, -1, typeIndex, GridObjectType.Car);
            currentCell = nextCell;

        }
        else
        {
            transform.position = targetPosition;
        }
    }

    void Attack()
    {
        if (currentHealth <= 0 || currentObstacle == null || currentObstacle.IsDestroyed())
        {
            isStopped = false;
            currentObstacle = null;
            return;
        }

        if (!currentObstacle.IsOffensive())
        {
            currentObstacle.TakeDamage(damage * Time.deltaTime);
        }
    }

    void TryResumeMovement()
    {
        if (!Physics.Raycast(transform.position, direction, out RaycastHit hit, 0.5f, detectionMask))
        {
            isStopped = false;
            currentObstacle = null;
        }
    }

    public void ReceiveDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            SetPushingAnimation(false);
            carTrafficData.RemoveObjectAt(currentCell);
            poolManager.ReturnCarToPool(gameObject, typeIndex);
            OnCarRemoved?.Invoke();
        }
    }


    void CheckOutOfBounds()
    {
        if (transform.position.x < -20f || transform.position.x > 100f ||
            transform.position.z < -20f || transform.position.z > 100f)
        {
            carTrafficData.RemoveObjectAt(currentCell);
            poolManager.ReturnCarToPool(gameObject, typeIndex);
            OnCarRemoved?.Invoke();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || grid == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(grid.GetCellCenterWorld(currentCell) + new Vector3(0, 0.1f, 0), new Vector3(1, 0.1f, 1));

        Vector3 nextPos = transform.position + direction.normalized * 0.5f;
        Vector3Int nextCell = grid.WorldToCell(nextPos);

        Gizmos.color = isStopped ? Color.red : Color.cyan;
        Gizmos.DrawWireCube(grid.GetCellCenterWorld(nextCell) + new Vector3(0, 0.2f, 0), new Vector3(1, 0.1f, 1));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + direction.normalized * 1.5f);
    }

    public bool IsStopped() => isStopped;

    public void SetPushingAnimation(bool isPushing)
    {
        if (animator == null)
        {
            Debug.LogWarning("Animator es null!");
            return;
        }

        if (isPushingObstacle == isPushing) return;

        Debug.Log($"SetPushingAnimation: {isPushing}");//Nunca sale esto en la consola
        isPushingObstacle = isPushing;
        animator.SetBool("isPushing", isPushing);
    }



}


