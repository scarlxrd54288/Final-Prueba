using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCspawner : MonoBehaviour
{
    public GameObject npcPrefab;
    public GameObject[] objetos; // Piedra, Llanta, Tronco
    public Transform[] posiciones; // Puntos de spawn

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivarNPC(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActivarNPC(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ActivarNPC(2);
        }
    }

    void ActivarNPC(int index)
    {
        // Apagar todos los objetos primero
        foreach (GameObject obj in objetos)
            obj.SetActive(false);

        // Crear el NPC en la posición
        GameObject npc = Instantiate(npcPrefab, posiciones[index].position, Quaternion.identity);

        // Decirle qué objeto activar
        npc.GetComponent<ControlNPCs>().AsignarObjeto(objetos[index]);
    }
}
