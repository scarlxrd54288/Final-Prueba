using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private Animator animator;
    private float lifetime;

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
}
