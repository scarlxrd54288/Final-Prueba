using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    [SerializeField] public PlacementSystem placementSystem;
    [SerializeField] private CarPoolManager carPool;
    [SerializeField] public List<Lane> lanes;

    [Header("Grid References")]
    [SerializeField] private Grid grid;

    private int currentWave = 0;

    public static TrafficManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (PointManager.Instance != null)
        {
            PointManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }
    }

    private void OnDisable()
    {
        if (PointManager.Instance != null)
        {
            PointManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }
    }

    private void Start()
    {
        if (grid == null)
            Debug.LogError("Grid reference en TrafficManager es NULL!");
        else
            Debug.Log("Grid reference asignada correctamente.");

        //Spawn valor bajo--------
        foreach (Lane lane in lanes)
        {
            //lane.currentTimer = Random.Range(0f, lane.baseSpawnInterval);
            //lane.currentTimer = lane.baseSpawnInterval;
            lane.currentTimer = Random.Range(lane.baseSpawnInterval * 0.7f, lane.baseSpawnInterval);

        }
    }

    private void Update()
    {
        for (int i = 0; i < lanes.Count; i++)
        {
            Lane lane = lanes[i];
            if (!lane.active) continue;

            lane.currentTimer -= Time.deltaTime;

            if (lane.currentTimer <= 0f)
            {
                TrySpawnCarInLane(lane, i);
                //lane.currentTimer = lane.GetSpawnInterval(currentWave);
                lane.currentTimer = lane.GetSpawnInterval(currentWave) * Random.Range(0.9f, 1.1f);

            }
        }
    }

    private void TrySpawnCarInLane(Lane lane, int laneIndex)
    {
        if (lane.activeCars.Count >= 5) return; //Limitee----
        Vector3Int spawnCell = grid.WorldToCell(lane.spawnPoint.position);

        if (placementSystem.GlobalGridData.HasObjectAt(spawnCell)) return;

        int randomType = Random.Range(0, carPool.CarTypeCount);
        var carData = carPool.GetCarData(randomType);
        GameObject car = carPool.GetCarFromPool(randomType);

        if (car != null)
        {
            Vector3 offset = lane.direction.normalized * Random.Range(0.5f, 1.5f);
            Vector3 spawnPos = lane.spawnPoint.position + offset;
            spawnPos.y = 0f;

            car.transform.position = spawnPos;

            Vector3 dir = new Vector3(lane.direction.x, 0f, lane.direction.z).normalized;
            if (dir != Vector3.zero)
                car.transform.rotation = Quaternion.LookRotation(dir);
            else
                Debug.LogError($"[TrafficManager] Direcci�n inv�lida en lane #{laneIndex}");

            var carController = car.GetComponent<CarController>();
            carController.Initialize(
                lane.direction,
                lane.speed,
                carData.baseDamage,
                randomType,
                carPool,
                placementSystem.GlobalGridData,
                grid
            );

            lane.activeCars.Add(carController);

            carController.OnCarRemoved += () =>
            {
                lane.activeCars.Remove(carController);
            };
        }
    }

    public void SetLaneSpeed(int laneIndex, float newSpeed)
    {
        if (laneIndex >= 0 && laneIndex < lanes.Count)
            lanes[laneIndex].speed = newSpeed;
    }

    public void SetLaneDirection(int laneIndex, Vector3 newDirection)
    {
        if (laneIndex >= 0 && laneIndex < lanes.Count)
            lanes[laneIndex].direction = newDirection.normalized;
    }

    public void StartNextWave()
    {
        currentWave++;
        Debug.Log($"[OLEADA] Nueva oleada: {currentWave}");
    }

    //Cambios de dificultad seg�n oleada
    private void OnGameStateChanged(PointManager.GameState newState)
    {
        switch (newState)
        {
            case PointManager.GameState.Wave1:
                currentWave = 1;
                AdjustLaneSpeeds(1.2f);
                AdjustLaneSpawnMultipliers(0.9f); //10% m�s r�pido
                Debug.Log("[TrafficManager] Wave1: autos un poco m�s r�pidos y m�s frecuentes.");
                break;

            case PointManager.GameState.BetweenWaves:
                currentWave = 0;
                AdjustLaneSpeeds(1.0f);
                AdjustLaneSpawnMultipliers(1.0f); //vuelve a calm
                Debug.Log("[TrafficManager] Entre oleadas: ritmo normal.");
                break;

            case PointManager.GameState.Wave2:
                currentWave = 2;
                AdjustLaneSpeeds(1.5f);
                AdjustLaneSpawnMultipliers(0.7f); //30% m�s r�pido
                Debug.Log("[TrafficManager] Wave2: autos r�pidos y aparecen m�s seguido.");
                break;

            case PointManager.GameState.Victory:
            case PointManager.GameState.GameOver:
                StopAllTraffic();
                Debug.Log("[TrafficManager] Tr�fico detenido (fin de juego).");
                break;
        }
    }

    private void AdjustLaneSpeeds(float multiplier)
    {
        foreach (var lane in lanes)
        {
            lane.speed *= multiplier;
        }
    }

    private void AdjustLaneSpawnMultipliers(float multiplier)
    {
        foreach (var lane in lanes)
        {
            lane.spawnMultiplier = multiplier;
        }
    }

    private void StopAllTraffic()
    {
        foreach (var lane in lanes)
        {
            lane.active = false;
        }
    }
}


