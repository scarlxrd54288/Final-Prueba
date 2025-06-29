using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.EditorTools;
using UnityEngine;

public class CarControllerAlternate : MonoBehaviour
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

    //Tiempo de gracia para evitar que el raycast detecte el propio coche al spawnear
    private float spawnGraceTime = 0.1f;
    private float spawnTimer = 0f;

    //Evento que avisa cuando el auto se retira
    public event Action OnCarRemoved;

    [SerializeField] private float maxHealth = 10f;
    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
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
            carTrafficData.AddObjectAt(currentCell, Vector2Int.one, -1, -1, GridObjectType.Car);
        }
        else
        {
            Debug.LogWarning($"[CarController] Auto aparece sobre celda ya ocupada ({currentCell}). Se permitir� movimiento inicial.");
        }

        isInitialized = true;
        //Debug.Log($" Auto inicializado en celda {currentCell} con direcci�n {direction} y velocidad {speed}");
    }

    void Update()
    {
        if (!isInitialized) return;

        if (spawnTimer > 0f)
        {
            spawnTimer -= Time.deltaTime;
        }

        if (isStopped)
        {
            TryResumeMovement();
        }

        if (!isStopped)
        {
            Move();
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 0.6f))
            {
                Obstacle obs = hit.collider.GetComponent<Obstacle>();
                if (obs != null && obs.IsOffensive())
                {
                    //CAmibiar en osbtaclee-----------------
                    //obs.ApplyDamageToCar(this);
                    return; 
                }
            }
        }
        else
        {
            if (currentObstacle != null && !currentObstacle.IsOffensive())
            {
                Attack();
            }
        }

        CheckOutOfBounds();
    }

    private bool hasPlayedCrashSound = false;
    void Move()
    {
        if (grid == null || carTrafficData == null) return;

        Vector3 targetPosition = transform.position + direction * speed * Time.deltaTime;
        Vector3Int nextCell = grid.WorldToCell(targetPosition);

        
        if (spawnTimer <= 0f)
        {
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 0.2f))
            {
                Obstacle obs = hit.collider.GetComponent<Obstacle>();
                if (obs != null)
                {
                    currentObstacle = obs;
                    isStopped = true;
                    if (!hasPlayedCrashSound) 
                    {
                        AudioManager.Instance.PlayCrashSound();
                        hasPlayedCrashSound = true; 
                    }
                    return;
                }
                
                CarControllerAlternate otherCar = hit.collider.GetComponent<CarControllerAlternate>();
                if (otherCar != null && otherCar != this)
                {
                    isStopped = true;
                    currentObstacle = null;
                    if (!hasPlayedCrashSound) 
                    {
                        AudioManager.Instance.PlayCrashSound();
                        hasPlayedCrashSound = true; 
                    }
                    return;
                }
            }
        }
        
        if (nextCell != currentCell)
        {
            /*
            if (carTrafficData.HasObjectAt(nextCell))
            {
                isStopped = true;
                currentObstacle = null;
                return;
            }
            */
            transform.position = targetPosition;
            carTrafficData.RemoveObjectAt(currentCell);
            carTrafficData.AddObjectAt(nextCell, Vector2Int.one, -1, -1, GridObjectType.Car);
            currentCell = nextCell;
            //Sonido de choque---
            hasPlayedCrashSound = false;
        }

                else
        {
            transform.position = targetPosition;
        }
        
    }

    void Attack()
    {
        if (currentHealth <= 0)
            return;

        if (currentObstacle == null || currentObstacle.IsDestroyed())
        {
            Debug.Log($"{name} deja de atacar porque el obst�culo est� destruido o no existe.");
            isStopped = false;
            currentObstacle = null;

            Vector3 forward = transform.position + direction * 0.5f;
            Vector3Int forwardCell = grid.WorldToCell(forward);
            carTrafficData.RemoveObjectAt(forwardCell);

            return;
        }

        Debug.Log($"{name} est� atacando a {currentObstacle.gameObject.name} con da�o {damage * Time.deltaTime}");

        if (currentObstacle.IsOffensive())
        {
            Debug.Log($"{name} no ataca porque el obst�culo es ofensivo.");
            return;
        }

        currentObstacle.TakeDamage(damage * Time.deltaTime);
    }


    void TryResumeMovement()
    {
        Vector3 nextPos = transform.position + direction * 0.5f;
        Vector3Int nextCell = grid.WorldToCell(nextPos);

        RaycastHit hit;
        bool obstacleCleared = !Physics.Raycast(transform.position, direction, out hit, 0.5f);
        /*
        if (!obstacleCleared)
        {
            Debug.Log($"{name} [TryResume] nextCell: {nextCell}, occupied: {carTrafficData.HasObjectAt(nextCell)}, rayHit: True, objectHit: {hit.collider.gameObject.name}");
        }
        else
        {
            Debug.Log($"{name} [TryResume] nextCell: {nextCell}, occupied: {carTrafficData.HasObjectAt(nextCell)}, rayHit: False");
        }
        */
        bool cellFree = !carTrafficData.HasObjectAt(nextCell);
        if (cellFree && obstacleCleared)
        {
            isStopped = false;
            currentObstacle = null;
            //Debug.Log($"{gameObject.name} desbloqueado, reanuda movimiento");
        }
    }

    public bool IsStopped()
    {
        return isStopped;
    }

    void CheckOutOfBounds()
    {
        if (transform.position.x < -20f || transform.position.x > 100f ||
            transform.position.z < -20f || transform.position.z > 100f)
        {
            carTrafficData.RemoveObjectAt(currentCell);
            poolManager.ReturnCarToPool(gameObject, typeIndex);
            Debug.Log($"{gameObject.name} se elimin� fuera de l�mites en {transform.position}");

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

    public void ReceiveDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibi� da�o. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} destruido por da�o");
            carTrafficData.RemoveObjectAt(currentCell);
            poolManager.ReturnCarToPool(gameObject, typeIndex);
            OnCarRemoved?.Invoke();
        }
    }
    //maybe a initialize
    

}
