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

    [HideInInspector] public float currentTimer = 0f;

    public List<CarController> activeCars = new List<CarController>();

    public float GetSpawnInterval(int wave)
    {
        return Mathf.Clamp(baseSpawnInterval * Mathf.Pow(0.95f, wave), 0.4f, baseSpawnInterval);
    }

    public int GetBlockedCarsCount()
    {
        int count = 0;
        foreach (var car in activeCars)
        {
            if (car.IsStopped())
            {
                count++;
                Debug.Log($"Auto bloqueado: {car.name}");
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
