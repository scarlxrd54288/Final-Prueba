using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lane
{
    public Transform spawnPoint;
    public Vector3 direction = Vector3.forward;
    public float speed = 3f;
    public bool active = true;

    [Header("Frecuencia de Spawn")]
    public float baseSpawnInterval = 2f;

    [Tooltip("Multiplicador dinámico ajustado por el TrafficManager")]
    public float spawnMultiplier = 1f; // Nuevo campo para modificar la frecuencia externamente

    [HideInInspector] public float currentTimer = 0f;

    public List<CarController> activeCars = new List<CarController>();

    public float GetSpawnInterval(int wave)
    {
        // Combina progresión por oleada con modificador externo (TrafficManager)
        float dynamicInterval = baseSpawnInterval * Mathf.Pow(0.95f, wave);
        return Mathf.Clamp(dynamicInterval * spawnMultiplier, 0.4f, baseSpawnInterval);
    }

    public int GetBlockedCarsCount()
    {
        int count = 0;
        foreach (var car in activeCars)
        {
            if (car.IsStopped())
            {
                count++;
                //Debug.Log($"Auto bloqueado: {car.name}");
            }
        }

        return count;
    }

    public bool IsFullyBlocked()
    {
        foreach (var car in activeCars)
        {
            if (!car.IsStopped())
                return false;
        }
        return activeCars.Count > 0;
    }
}
