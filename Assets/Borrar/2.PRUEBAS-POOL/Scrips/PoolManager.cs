using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string nombre;
        public GameObject prefab;
        public int cantidad;
        public Queue<GameObject> objetos;
    }

    public List<Pool> pools;
    private Dictionary<string, Pool> poolDiccionario;

    void Awake()
    {
        Instance = this;
        poolDiccionario = new Dictionary<string, Pool>();

        foreach (var pool in pools)
        {
            Queue<GameObject> cola = new Queue<GameObject>();
            for (int i = 0; i < pool.cantidad; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                cola.Enqueue(obj);
            }
            pool.objetos = cola;
            poolDiccionario[pool.nombre] = pool;
        }
    }

    public GameObject ObtenerDelPool(string nombre)
    {
        if (!poolDiccionario.ContainsKey(nombre)) return null;

        var pool = poolDiccionario[nombre];
        GameObject obj = pool.objetos.Dequeue();
        pool.objetos.Enqueue(obj);
        return obj;
    }
}
