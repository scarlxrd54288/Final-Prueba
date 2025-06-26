using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPoolManager : MonoBehaviour
{
    public static NPCPoolManager Instance;

    [SerializeField] private NPCDatabaseSO npcDatabase;

    private Dictionary<int, Queue<GameObject>> npcPools = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public GameObject GetNPC(int id)
    {
        // Buscar datos del NPC
        var npcData = npcDatabase.npcList.Find(n => n.ID == id);
        if (npcData == null)
        {
            Debug.LogWarning($"NPC con ID {id} no encontrado en la base de datos.");
            return null;
        }

        // Si no existe la cola aún, créala
        if (!npcPools.ContainsKey(id))
        {
            npcPools[id] = new Queue<GameObject>();
        }

        // Reutilizar si hay en la cola
        if (npcPools[id].Count > 0)
        {
            GameObject npc = npcPools[id].Dequeue();
            npc.SetActive(true);
            return npc;
        }

        // Si no hay en cola, instanciar uno nuevo
        return Instantiate(npcData.Prefab);
    }

    public void ReturnNPC(GameObject npc, int id)
    {
        npc.SetActive(false);
        if (!npcPools.ContainsKey(id))
        {
            npcPools[id] = new Queue<GameObject>();
        }
        npcPools[id].Enqueue(npc);
    }
}

