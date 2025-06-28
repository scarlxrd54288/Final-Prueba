using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip placeObjectClip;
    [SerializeField] private AudioClip placeErrorClip;
    [SerializeField] private AudioClip carCrashClip;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip unlockClip;


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
        DontDestroyOnLoad(gameObject);
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
    public void PlayButtonClickSound() => PlaySFX(buttonClickClip);

    private bool canPlayCrash = true;
    public float crashCooldown = 0.5f; // medio segundo de cooldown

    public void PlayCrashSound()
    {
        if (carCrashClip == null || !canPlayCrash) return;

        float randomPitch = UnityEngine.Random.Range(pitchMin, pitchMax);
        float randomVolume = UnityEngine.Random.Range(crashVolumeMin, crashVolumeMax);
        PlaySFX(carCrashClip, randomVolume, randomPitch);

        canPlayCrash = false;
        Invoke(nameof(ResetCrashCooldown), crashCooldown);
    }

    private void ResetCrashCooldown()
    {
        canPlayCrash = true;
    }

    // Nuevo método que crea un AudioSource temporal para pitch independiente
    private void PlaySFX(AudioClip clip)
    {
        PlaySFX(clip, sfxVolume, UnityEngine.Random.Range(pitchMin, pitchMax));
    }

    private void PlaySFX(AudioClip clip, float volume, float pitch)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio");
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume;
        aSource.pitch = pitch;
        aSource.Play();

        Destroy(tempGO, clip.length / pitch);
    }
    public void PlayUnlockSound()
    {
        PlaySFX(unlockClip);
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


