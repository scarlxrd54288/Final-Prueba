using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip placeObjectClip;
    [SerializeField] private AudioClip placeErrorClip;
    [SerializeField] private AudioClip carCrashClip;
    [SerializeField] private AudioClip buttonClickClip;

    [Header("Looping")]
    [SerializeField] private AudioClip musicLoop;
    [SerializeField] private AudioClip ambientLoop;

    [Header("SFX Pitch Settings")]
    [Range(0.7f, 1.3f)] public float pitchMin = 0.9f;
    [Range(0.7f, 1.3f)] public float pitchMax = 1.1f;

    [Header("Car Crash Volume Settings")]
    [Range(0f, 1f)] public float crashVolumeMin = 0.5f;
    [Range(0f, 1f)] public float crashVolumeMax = 1f;

    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float ambientVolume = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        SetAmbientVolume(ambientVolume);
        PlayMusicLoop();
        PlayAmbientLoop();
    }

    // === Sound FX Methods ===
    public void PlayPlaceObjectSound() => PlaySFX(placeObjectClip);
    public void PlayPlaceErrorSound() => PlaySFX(placeErrorClip);
    //public void PlayCrashSound() => PlaySFX(carCrashClip);
    public void PlayButtonClickSound() => PlaySFX(buttonClickClip);

    public void PlayCrashSound()
    {
        if (carCrashClip == null) return;

        float randomPitch = UnityEngine.Random.Range(pitchMin, pitchMax);
        float randomVolume = UnityEngine.Random.Range(crashVolumeMin, crashVolumeMax);
        sfxSource.pitch = randomPitch;
        sfxSource.PlayOneShot(carCrashClip, randomVolume * sfxVolume);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        float randomPitch = UnityEngine.Random.Range(pitchMin, pitchMax);
        sfxSource.pitch = randomPitch;
        sfxSource.PlayOneShot(clip);
    }

    // === Music & Ambient ===
    private void PlayMusicLoop()
    {
        if (musicLoop != null)
        {
            musicSource.clip = musicLoop;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private void PlayAmbientLoop()
    {
        if (ambientLoop != null)
        {
            ambientSource.clip = ambientLoop;
            ambientSource.loop = true;
            ambientSource.Play();
        }
    }

    // === Volume Controls (for sliders/UI) ===
    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        sfxSource.volume = sfxVolume;
    }

    public void SetAmbientVolume(float value)
    {
        ambientVolume = value;
        ambientSource.volume = ambientVolume;
    }

    // === Pitch controls for UI ===
    public void SetSFXPitchMin(float value)
    {
        pitchMin = value;
    }

    public void SetSFXPitchMax(float value)
    {
        pitchMax = value;
    }
}

