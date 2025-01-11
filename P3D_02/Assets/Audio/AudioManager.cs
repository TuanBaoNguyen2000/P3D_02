using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;
        public AudioMixerGroup mixerGroup;

        [HideInInspector]
        public AudioSource source;
    }

    public AudioMixer audioMixer;
    public List<Sound> sounds = new List<Sound>();

    private const string MASTER_VOLUME = "MasterVolume";
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string SFX_VOLUME = "SFXVolume";

    protected override void Awake()
    {
        base.Awake();

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            if (s.mixerGroup != null)
                s.source.outputAudioMixerGroup = s.mixerGroup;
        }
    }

    public void Play(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Âm thanh {name} không t?n t?i!");
            return;
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Âm thanh {name} không t?n t?i!");
            return;
        }
        s.source.Stop();
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat(MASTER_VOLUME, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(MASTER_VOLUME, volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat(MUSIC_VOLUME, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(MUSIC_VOLUME, volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat(SFX_VOLUME, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(SFX_VOLUME, volume);
    }

    private void LoadSettings()
    {
        float masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME, 1f);
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME, 1f);

        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    public bool IsPlaying(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Âm thanh {name} không t?n t?i!");
            return false;
        }
        return s.source.isPlaying;
    }

    public void PauseAll()
    {
        foreach (Sound s in sounds)
        {
            if (s.source.isPlaying)
                s.source.Pause();
        }
    }

    public void ResumeAll()
    {
        foreach (Sound s in sounds)
        {
            if (!s.source.isPlaying)
                s.source.UnPause();
        }
    }
}
