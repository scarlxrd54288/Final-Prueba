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

    private void Awake()
    {
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
        Destroy(gameObject);
    }

    public void Evolve(int newLevel, float newMaxResistance)
    {
        if (newLevel < evolutionLevel) return;

        evolutionLevel = newLevel;
        maxResistance = newMaxResistance;
        currentResistance = maxResistance;
    }

    public bool IsDestroyed() => currentResistance <= 0;
}

