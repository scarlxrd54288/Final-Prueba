using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarTypePool
{
    public CarDataSO carData;
    public int poolSize;
}

public class CarPoolManager : MonoBehaviour
{
    [SerializeField] private List<CarTypePool> carTypes;

    private List<Queue<GameObject>> carPools = new();

    void Awake()
    {
        foreach (var type in carTypes)
        {
            Queue<GameObject> queue = new();
            for (int i = 0; i < type.poolSize; i++)
            {
                GameObject car = Instantiate(type.carData.prefab);
                car.SetActive(false);
                queue.Enqueue(car);
            }
            carPools.Add(queue);
        }
    }

    public GameObject GetCarFromPool(int index)
    {
        if (index < 0 || index >= carPools.Count) return null;

        var queue = carPools[index];
        if (queue.Count > 0)
        {
            var car = queue.Dequeue();
            car.SetActive(true);
            return car;
        }

        return null;
    }

    public void ReturnCarToPool(GameObject car, int typeIndex)
    {
        car.SetActive(false);
        if (typeIndex >= 0 && typeIndex < carPools.Count)
        {
            carPools[typeIndex].Enqueue(car);
        }
    }

    public int CarTypeCount => carTypes.Count;

    public CarDataSO GetCarData(int index)
    {
        return carTypes[index].carData;
    }
}
