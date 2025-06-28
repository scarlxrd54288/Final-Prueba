using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPoolManager : MonoBehaviour
{
    public static CoinPoolManager Instance;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int poolSize = 20;

    private Queue<GameObject> coinPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject coin = Instantiate(coinPrefab);
            coin.SetActive(false);
            coinPool.Enqueue(coin);
        }
    }

    public void SpawnCoin(Vector3 position, float lifeTime = 2f)
    {
        if (coinPool.Count > 0)
        {
            GameObject coin = coinPool.Dequeue();
            coin.transform.position = position;
            coin.SetActive(true);
            StartCoroutine(ReturnToPool(coin, lifeTime));
        }
    }

    private IEnumerator ReturnToPool(GameObject coin, float delay)
    {
        yield return new WaitForSeconds(delay);
        coin.SetActive(false);
        coinPool.Enqueue(coin);
    }
}
