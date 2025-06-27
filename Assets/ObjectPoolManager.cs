using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    private Dictionary<int, Queue<GameObject>> poolDictionary = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public GameObject GetObject(GameObject prefab, int poolKey)
    {
        if (!poolDictionary.ContainsKey(poolKey))
            poolDictionary[poolKey] = new Queue<GameObject>();

        if (poolDictionary[poolKey].Count > 0)
        {
            GameObject obj = poolDictionary[poolKey].Dequeue();
            obj.SetActive(true);
            return obj;
        }

        return Instantiate(prefab);
    }

    public void ReturnObject(GameObject obj, int poolKey)
    {
        obj.SetActive(false);
        if (!poolDictionary.ContainsKey(poolKey))
            poolDictionary[poolKey] = new Queue<GameObject>();
        poolDictionary[poolKey].Enqueue(obj);
    }
}

