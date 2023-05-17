using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoSingleton<SoundManager> {
    private bool musicOn;
    public bool MusicOn
    {
        get { return musicOn; }
        set
        {
            musicOn = value;
            this.MusicMute(!value);
        }
    }
    private bool soundOn;
    public bool SoundOn {
        get { return soundOn; }
        set 
        { 
            soundOn = value;
            this.SoundMute(!value);
        }
     }
    private int musicVolume;
    public int MusicVolume
    {
        get { return musicVolume; }
        set
        {
            if (musicVolume != value)
            {
                musicVolume = value;
                if (soundOn) this.SetVolume("MusicVolume", musicVolume);
            }
        }
    }
    private int soundVolume;
    public int SoundVolume {
        get { return soundVolume; }
        set 
        { 
            if(soundVolume != value)
            {
                soundVolume = value;
                if (soundOn) this.SetVolume("SoundVolume", soundVolume);
            }
        } 
    }

    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private AudioSource musicAudioSource;
    [SerializeField]
    private AudioSource soundAuidoSource;

    const string musicPath = "Music/";
    const string soundPath = "Sound/";

    void Start()
    {
        this.MusicVolume = Config.MusicVolume;
        this.SoundVolume = Config.SoundVolume;
        this.MusicOn = Config.MusicOn;
        this.SoundOn = Config.SoundOn;
    }
    
    public void Playmusic(string music)
    {
        AudioClip clip = Resloader.Load<AudioClip>(musicPath + music);
        if(clip == null)
        {
            Debug.LogWarningFormat("Playmusic {0} not exist", music);
            return;
        }
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }
        musicAudioSource.clip = clip; 
        musicAudioSource.Play();
    }

    public void PlaySound(string sound)
    {
        AudioClip clip = Resloader.Load<AudioClip>(soundPath + sound);
        if (clip == null)
        {
            Debug.LogWarningFormat("Playsound {0} not exist", sound);
            return;
        }
        soundAuidoSource.PlayOneShot(clip);
    }

    private void MusicMute(bool v)
    {
        this.SetVolume("MusicVolume", v ? 0 : MusicVolume);
    }

    private void SoundMute(bool v)
    {
        this.SetVolume("SoundVolume", v ? 0 : MusicVolume);
    }

    private void SetVolume(string name, int volume)
    {
        float v = volume * 0.5f - 50f;
        this.audioMixer.SetFloat(name, v);
    }
}
