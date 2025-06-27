using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private Animator animator;
    private float lifetime;

    private int poolID;

    public void SetPoolID(int id)
    {
        poolID = id;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayEntry(float duration)
    {
        if (animator != null)
            animator.SetTrigger("entry");

        lifetime = duration;
        StartCoroutine(DespawnAfter());
    }

    private IEnumerator DespawnAfter()
    {
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
    }

    public void ForceDespawn()
    {
        // Cancela cualquier coroutine o animación activa
        //StopAllCoroutines();

        // Vuelve al pool inmediatamente
        NPCPoolManager.Instance.ReturnNPC(gameObject, poolID);
    }

}
