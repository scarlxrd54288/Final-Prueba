using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventosNPCs : MonoBehaviour
{
    public GameObject objeto;       
    public Animator animator;       

    // 🔹 Se llama al lanzar el NPC (desde otro script)
    public void AsignarObjeto(GameObject obj)
    {
        objeto = obj;
        animator = GetComponent<Animator>();
        animator.Play("Levantar"); // Cambia por el nombre exacto de tu animación
    }

    // 🔹 Evento desde la animación (cuando debe aparecer el objeto)
    public void MostrarObjeto()
    {
        if (objeto != null)
        {
            objeto.SetActive(true);
        }
    }

    // 🔹 Evento al final de la animación (para desaparecer al NPC)
    public void DesaparecerNPC()
    {
        Destroy(gameObject); // O gameObject.SetActive(false) si haces pooling de NPCs también
    }


}
