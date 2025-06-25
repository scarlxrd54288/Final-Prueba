using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float maxResistance = 1f;
    private float currentResistance;
    [SerializeField] private int evolutionLevel = 0;

    public int EvolutionLevel => evolutionLevel;
    public float CurrentResistance => currentResistance;

    public delegate void ObstacleDestroyedHandler(Obstacle obstacle);
    public event ObstacleDestroyedHandler OnObstacleDestroyed;

    private int poolID;
    public int PoolID => poolID;

    [SerializeField] private float damage = 0f;
    public float Damage => damage;


    public void SetPoolID(int id)
    {
        poolID = id;
    }

    private void Awake()
    {
        currentResistance = maxResistance;
    }

    private void OnEnable()
    {
        // Reinicia resistencia cada vez que el objeto se active desde el pool
        currentResistance = maxResistance;
    }

    public void TakeDamage(float amount)
    {
        currentResistance -= amount;
        if (currentResistance <= 0)
        {
            currentResistance = 0;
            DestroyObstacle();
        }
    }

    private void DestroyObstacle()
    {
        OnObstacleDestroyed?.Invoke(this);
        ObjectPoolManager.Instance.ReturnObject(gameObject, poolID);
    }

    public void Evolve(int newLevel, float newMaxResistance, float newDamage = 0f)
    {
        if (newLevel < evolutionLevel) return;

        evolutionLevel = newLevel;
        maxResistance = newMaxResistance;
        currentResistance = maxResistance;
        damage = newDamage;
    }


    public bool IsDestroyed() => currentResistance <= 0;
}
