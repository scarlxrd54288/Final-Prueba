using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ObjectDatabaseSO : ScriptableObject
{
    public List<ObjectData> objectsData;
}

[Serializable]
public class ObjectData
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field: SerializeField] public GameObject Prefab { get; private set; }

    [field: SerializeField] public int BaseResistance { get; private set; } = 1;
    [field: SerializeField] public int[] ResistancePerLevel;
    [field: SerializeField] public float EvolvedResistance { get; private set; }
    [field: SerializeField] public float EvolvedDamage { get; private set; }

    [field: SerializeField] public int UnlockCost { get; private set; }
    [field: SerializeField] public bool Unlocked { get; set; }

    [field: SerializeField] public float CooldownTime { get; private set; }
    [HideInInspector] public float CooldownTimer;

    //[field: SerializeField] public bool BlocksCars { get; private set; }
    [field: SerializeField] public bool IsOffensive { get; private set; }
    [field: SerializeField] public int Durability { get; private set; }

    [SerializeField] public bool Evolved = false;
    public int EvolutionPoints;
    public GameObject EvolvedPrefab;
    public int EvolutionLevel;


}
