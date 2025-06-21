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


    private void Start()
    {
        if (grid == null)
            Debug.LogError("Grid reference en TrafficManager es NULL!");
        else
            Debug.Log("Grid reference asignada correctamente.");

        // Inicializar timers individuales
        foreach (Lane lane in lanes)
        {
            lane.currentTimer = Random.Range(0f, lane.baseSpawnInterval);
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
                lane.currentTimer = lane.GetSpawnInterval(currentWave);
            }
        }
    }

    private void TrySpawnCarInLane(Lane lane, int laneIndex)
    {
        Vector3Int spawnCell = grid.WorldToCell(lane.spawnPoint.position);

        if (placementSystem.GlobalGridData.HasObjectAt(spawnCell)) return;

        int randomType = Random.Range(0, carPool.CarTypeCount);
        var carData = carPool.GetCarData(randomType);
        GameObject car = carPool.GetCarFromPool(randomType);

        if (car != null)
        {
            Vector3 offset = lane.direction.normalized * Random.Range(0f, 0.8f);
            Vector3 spawnPos = lane.spawnPoint.position + offset;
            spawnPos.y = 0.112f;

            car.transform.position = spawnPos;

            Vector3 dir = new Vector3(lane.direction.x, 0f, lane.direction.z).normalized;
            if (dir != Vector3.zero)
                car.transform.rotation = Quaternion.LookRotation(dir);
            else
                Debug.LogError($"[TrafficManager] Dirección inválida en lane #{laneIndex}");

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
}


