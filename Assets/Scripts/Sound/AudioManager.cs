using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private Sound[] bgMusic;
    [Space(15f)]
    [SerializeField] private Sound[] bossBgMusic;
    [Space(15f)]
    [SerializeField] private Sound[] ebgMusic;
    [Space(15f)]
    [SerializeField] private Sound[] sfxSounds;

    //[Space(15f)]
    public AudioSource musicSource;
    public AudioSource sfxSource;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        int randomIndex = UnityEngine.Random.Range(0, bgMusic.Length);
        string bgName = bgMusic[randomIndex].soundName;

        SaveSettings();
        LoadSettings();
        PlayBGMusic(bgName);
    }

    public void PlayBGMusic(string name)
    {
        Sound sound = Array.Find(bgMusic, x => x.soundName == name);
        if(sound == null) { return; }

        musicSource.clip = sound.soundClip;
        musicSource.Play();
    }

    public void PlayBossMusic(string name)
    {
        Sound sound = Array.Find(bossBgMusic, x => x.soundName == name);
        if (sound == null) { return; }

        musicSource.clip = sound.soundClip;
        musicSource.Play();
    }

    public void PlayEBGMusic(string name)
    {
        Sound sound = Array.Find(ebgMusic, x => x.soundName == name);
        if (sound == null) { return; }

        musicSource.clip = sound.soundClip;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        Sound sound = Array.Find(sfxSounds, x => x.soundName == name);
        if (sound == null) { return; }

        sfxSource.PlayOneShot(sound.soundClip);
    }

    public void ToggleMusic() => musicSource.mute = !musicSource.mute;
    public void ToggleSFX() => sfxSource.mute = !sfxSource.mute;

    public void MusicVolume(float volume) => musicSource.volume = volume;
    public void SFXVolume(float volume) => sfxSource.volume = volume;


    private void LoadSettings()
    {
        musicSource.volume = PlayerPrefs.GetFloat("musicVolume");
        sfxSource.volume = PlayerPrefs.GetFloat("sfxVolume");
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("musicVolume", musicSource.volume);
        PlayerPrefs.SetFloat("sfxVolume", sfxSource.volume);

        PlayerPrefs.Save();
    }
}
