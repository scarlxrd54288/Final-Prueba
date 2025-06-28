using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class VoiceOverSystem : MonoBehaviour
{
    public static VoiceOverSystem Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private AudioClip saludoClip;
    [SerializeField] private AudioClip enojadoClip;

    [SerializeField] private AudioClip[] winClips;
    [SerializeField] private AudioClip[] gameOverClips;

    [Header("Animación")]
    [SerializeField] private Animator characterAnimator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Reproducir saludo automáticamente al inicio
        //PlayVoiceWithEmotion(saludoClip, "Feliz");
        PlaySaludo();
    }

    public void PlaySaludo()
    {
        PlayVoiceWithEmotion(saludoClip, "Feliz");
        StartCoroutine(ResetBoolAfterDelay("Feliz", 7f));
    }

    public void PlayEnojado()
    {
        PlayVoiceWithEmotion(enojadoClip, "Enojado");
        StartCoroutine(ResetBoolAfterDelay("Enojado", 7f));
    }

    public void PlayWinVoice()
    {
        if (winClips.Length == 0) return;
        AudioClip clip = winClips[Random.Range(0, winClips.Length)];
        PlayVoiceWithEmotion(clip, "Feliz");
    }

    public void PlayGameOverVoice()
    {
        if (gameOverClips.Length == 0) return;
        AudioClip clip = gameOverClips[Random.Range(0, gameOverClips.Length)];
        PlayVoiceWithEmotion(clip, "Llorando");
    }



    public void PlayVoiceWithEmotion(AudioClip clip, string boolParameter, float customDuration = -1f)
    {
        if (clip == null || characterAnimator == null || voiceSource == null) return;

        voiceSource.clip = clip;
        voiceSource.Play();

        // Activar parámetro
        characterAnimator.SetBool(boolParameter, true);

        // Usar duración del clip o duración personalizada
        float duration = customDuration > 0 ? customDuration : clip.length;
        StartCoroutine(ResetBoolAfterDelay(boolParameter, duration));
    }

    private IEnumerator ResetBoolAfterDelay(string boolParameter, float delay)
    {
        yield return new WaitForSeconds(delay);
        characterAnimator.SetBool(boolParameter, false);
    }
}
