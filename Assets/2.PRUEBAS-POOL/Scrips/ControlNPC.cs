using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlNPC : MonoBehaviour
{
    public GameObject npcPrefab;
    public Transform[] spawnPoints; // 5 posiciones
    private string[] nombresObjetos = { "Tronco", "Bolsa", "Caja", "Piedras", "Llanta" };

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            StartCoroutine(AccionNPC(0));
        if (Input.GetKeyDown(KeyCode.Alpha2))
            StartCoroutine(AccionNPC(1));
        if (Input.GetKeyDown(KeyCode.Alpha3))
            StartCoroutine(AccionNPC(2));
        if (Input.GetKeyDown(KeyCode.Alpha4))
            StartCoroutine(AccionNPC(3));
        if (Input.GetKeyDown(KeyCode.Alpha5))
            StartCoroutine(AccionNPC(4));
    }

    IEnumerator AccionNPC(int index)
    {
    Transform spawn = spawnPoints[index];
    // -Mover un poco hacia atrás al NPC
    Vector3 offset = spawn.forward * -2f;
    //Vector3 offset = spawn.rotation * Vector3.back * 0.5f;

     // -Instanciar NPC temporal
    GameObject npc = Instantiate(npcPrefab, spawn.position, Quaternion.identity);
    npc.SetActive(true);

    // -Esperar animación
    yield return new WaitForSeconds(1f);

    GameObject obj = PoolManager.Instance.ObtenerDelPool(nombresObjetos[index]);
    obj.transform.position = spawn.position;
    obj.SetActive(true);

     //-NPC desaparece por su animación con evento
    }

    //IEnumerator AccionNPC(int index)
    //{
        //Transform spawn = spawnPoints[index];

        //GameObject obj = PoolManager.Instance.ObtenerDelPool(nombresObjetos[index]);
        //obj.SetActive(false); // el objeto aparecerá desde el evento

        //Vector3 offsetZ = spawn.forward * -0.5f; // mueve el NPC un poco hacia atrás
        //GameObject npc = Instantiate(npcPrefab, spawn.position + offsetZ, Quaternion.identity);
        //npc.GetComponent<EventosNPCs>().AsignarObjeto(obj);

        //yield break;
    //}
}
