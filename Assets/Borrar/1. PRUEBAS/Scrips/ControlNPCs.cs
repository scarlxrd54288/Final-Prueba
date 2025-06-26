using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlNPCs : MonoBehaviour
{
    public GameObject objeto; // Lo asignaremos al instanciar
    public Animator animator;

    public void AsignarObjeto(GameObject obj)
    {
        objeto = obj;
        animator = GetComponent<Animator>();
        animator.Play("Levantar"); // cambia "Levantar" por el nombre exacto de tu animación
    }

    // Evento en animación: cuando agarra el objeto
    public void MostrarObjeto()
    {
        if (objeto != null)
        {
            objeto.SetActive(true);
            objeto.transform.position = transform.position + Vector3.forward * 0.5f; // frente al NPC
        }
    }

    // Evento en animación: al final
    public void DesaparecerNPC()
    {
        Destroy(gameObject);
    }
}
