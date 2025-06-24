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

    public GameObject GetObject(GameObject prefab, int id)
    {
        if (!poolDictionary.ContainsKey(id))
        {
            poolDictionary[id] = new Queue<GameObject>();
        }

        if (poolDictionary[id].Count > 0)
        {
            GameObject obj = poolDictionary[id].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            return Instantiate(prefab);
        }
    }

    public void ReturnObject(GameObject obj, int id)
    {
        obj.SetActive(false);
        if (!poolDictionary.ContainsKey(id))
        {
            poolDictionary[id] = new Queue<GameObject>();
        }
        poolDictionary[id].Enqueue(obj);
    }
}

