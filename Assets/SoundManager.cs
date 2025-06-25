using System;
using System.Collections.Generic;
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
        PlayMusicLoop();
        PlayAmbientLoop();
    }

    // === Sound FX Methods ===

    public void PlayPlaceObjectSound()
    {
        PlaySFX(placeObjectClip);
    }

    public void PlayPlaceErrorSound()
    {
        PlaySFX(placeErrorClip);
    }

    public void PlayCrashSound()
    {
        PlaySFX(carCrashClip);
    }

    public void PlayButtonClickSound()
    {
        PlaySFX(buttonClickClip);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
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

    // === Optional Volume Controls ===

    public void SetMusicVolume(float value) => musicSource.volume = value;
    public void SetSFXVolume(float value) => sfxSource.volume = value;
    public void SetAmbientVolume(float value) => ambientSource.volume = value;
}

