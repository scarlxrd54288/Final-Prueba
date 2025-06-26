using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCDatabase", menuName = "Game/NPC Database")]
public class NPCDatabaseSO : ScriptableObject
{
    public List<NPCData> npcList;
}

[Serializable]
public class NPCData
{
    public string Name;
    public int ID;
    public GameObject Prefab;
    public float Lifetime = 2f;
}
