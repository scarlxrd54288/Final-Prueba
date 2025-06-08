using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    [SerializeField] public PlacementSystem placementSystem;
    [SerializeField] private CarPoolManager carPool;
    [SerializeField] private List<Lane> lanes;
    [SerializeField] private float spawnInterval = 2f;

    [Header("Grid References")]
    [SerializeField] private Grid grid;
    //private GridData carTrafficData;

    private void Awake()
    {
        // Crear la instancia de GridData
        //carTrafficData = new GridData();
        //placementSystem.SetCarTrafficData(carTrafficData);
        //Debug.Log("GridData instancia creada correctamente: " + carTrafficData);
    }

    private void Start()
    {
        if (grid == null)
            Debug.LogError("Grid reference en TrafficManager es NULL!");
        else
            Debug.Log("Grid reference asignada correctamente.");

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            for (int i = 0; i < lanes.Count; i++)
            {
                Lane lane = lanes[i];
                if (!lane.active) continue;

                Vector3Int spawnCell = grid.WorldToCell(lane.spawnPoint.position);

                // Revisa si la celda está libre antes de instanciar
                if (placementSystem.GlobalGridData.HasObjectAt(spawnCell))
                {
                    // Debug.Log("Celda ocupada en lane " + i + ", no se instancia auto.");
                    continue;
                }

                int randomType = Random.Range(0, carPool.CarTypeCount);
                var carData = carPool.GetCarData(randomType);
                GameObject car = carPool.GetCarFromPool(randomType);

                if (car != null)
                {
                    // Aplica offset aleatorio para evitar que todos queden en la misma posición exacta
                    Vector3 offset = lane.direction.normalized * Random.Range(0f, 0.8f);
                    Vector3 spawnPos = lane.spawnPoint.position + offset;

                    // Aquí fija la Y a 0.4 directamente
                    spawnPos.y = 0.112f;

                    car.transform.position = spawnPos;

                    // Validación segura del vector de rotación
                    Vector3 dir = new Vector3(lane.direction.x, 0f, lane.direction.z).normalized;

                    if (dir != Vector3.zero)
                    {
                        car.transform.rotation = Quaternion.LookRotation(dir);
                    }
                    else
                    {
                        Debug.LogError($"[TrafficManager] Dirección inválida en lane #{i}. Revisa el Inspector.");
                    }

                    car.GetComponent<CarController>().Initialize(
                        lane.direction,
                        lane.speed,
                        carData.baseDamage,
                        randomType,
                        carPool,
                        placementSystem.GlobalGridData,
                        grid
                    );
                }

            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }


    public void SetLaneSpeed(int laneIndex, float newSpeed)
    {
        if (laneIndex >= 0 && laneIndex < lanes.Count)
        {
            lanes[laneIndex].speed = newSpeed;
        }
    }

    public void SetLaneDirection(int laneIndex, Vector3 newDirection)
    {
        if (laneIndex >= 0 && laneIndex < lanes.Count)
        {
            lanes[laneIndex].direction = newDirection.normalized;
        }
    }

    private float GetCarDamage(CarDataSO carData)
    {
        return carData.baseDamage;
    }
}

