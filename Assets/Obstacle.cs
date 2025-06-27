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
    [SerializeField] private int durability = 0; // -1 = infinito
    private int damageCount = 0;

    private int hitCount = 0;

    private NPC associatedNPC;


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
        currentResistance = maxResistance;

        // Revisa si está evolucionado y aplica solo si debe
        if (PointManager.Instance.IsEvolved(poolID))
        {
            var data = PointManager.Instance.GetObjectData(poolID);
            Evolve(data.EvolutionLevel, data.BaseResistance, data.EvolvedDamage, data.Durability);
        }
    }


    public void TakeDamage(float amount)
    {
        Debug.Log($"{gameObject.name} recibe {amount} de daño. Resistencia actual: {currentResistance} -> {currentResistance - amount}");

        currentResistance -= amount;
        if (currentResistance <= 0)
        {
            currentResistance = 0;
            Debug.Log($"{gameObject.name} fue destruido!");
            DestroyObstacle();
        }
    }

    private void DestroyObstacle()
    {
        // 1. Borra su celda en la grilla
        /*
        GridData globalGrid = PlacementSystem.Instance.GlobalGridData;
        foreach (var pos in globalGrid.CalculatePositions(
                     grid.WorldToCell(transform.position), Vector2Int.one))
        {
            globalGrid.RemoveObjectAt(pos);
        }
        */
        if (associatedNPC != null)
        {
            associatedNPC.ForceDespawn(); // Este método lo agregaremos ahora
            associatedNPC = null;
        }
        //Original desde aqui----
        OnObstacleDestroyed?.Invoke(this);
        ObjectPoolManager.Instance.ReturnObject(gameObject, poolID);
    }

    public void Evolve(int newLevel, float newMaxResistance, float newDamage = 0f, int newDurability = -1)
    {
        if (newLevel < evolutionLevel) return;

        evolutionLevel = newLevel;
        maxResistance = newMaxResistance;
        currentResistance = maxResistance;
        damage = newDamage;
        durability = newDurability;
        hitCount = 0;
        Debug.Log($"{gameObject.name} evolucionado. Damage = {damage}, Durability = {durability}");

    }



    public bool IsDestroyed() => currentResistance <= 0;

    public void SetDurability(int value)
    {
        durability = value;
        damageCount = 0;
    }

    public void ApplyDamageToCar(CarController car)
    {
        if (damage <= 0f) return;
        if (car == null) return;

        car.ReceiveDamage(damage * Time.deltaTime);

        if (durability > 0)
        {
            damageCount++;
            Debug.Log($"{gameObject.name} ha dañado un auto. Golpes recibidos: {damageCount}/{durability}");

            if (damageCount >= durability)
            {
                DestroyObstacle();
            }
        }
        else
        {
            Debug.Log($"{gameObject.name} tiene durabilidad infinita.");
        }
    }



    public bool IsOffensive()
    {
        return gameObject.CompareTag("eObstacle") || damage > 0f;
    }


    public void SetAssociatedNPC(NPC npc)
    {
        associatedNPC = npc;
    }
}
