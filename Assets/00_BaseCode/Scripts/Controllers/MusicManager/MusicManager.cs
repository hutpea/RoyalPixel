﻿using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using MoreMountains.NiceVibrations;

public enum NameMusic
{
    None = 0,
    Do, Re, Mi, Fa, Son
}
public class MusicData
{
    public NameMusic name;
    public float spaceDelayTime;
    [HideInInspector] public bool isActiving;
    public AudioClip audioClip;
}
public class MusicManager : SerializedMonoBehaviour
{
    public float timer;
    public Dictionary<NameMusic, MusicData> clips;
    public List<PianoTileData> pianoDatas;

    public enum SourceAudio { Music, Effect, UI };
    public AudioMixer mixer;
    public AudioSource effectSource;
    public AudioSource musicSource;
    public AudioSource soundUISource;
    public float lowPitchRange = 0.95f;              //The lowest a sound effect will be randomly pitched.
    public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.
    public float delayTime = 0.5f;
    public const string MASTER_KEY = "MASTER_KEY";
    public const string MUSIC_KEY = "MUSIC_KEY";
    public const string SOUND_KEY = "SOUND_KEY";
    public const float MIN_VALUE = -80f;
    public const float MAX_VALUE = 0f;

    [SerializeField] private AudioClip winMusic;
    [SerializeField] private AudioClip BGMusic;
    [SerializeField] private AudioClip BGHomeMusic;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip soundClaim;
    [SerializeField] private AudioClip soundUseItem;
    [SerializeField] private AudioClip soundCompleteColor;
    [SerializeField] private AudioClip progressComplete;
    [SerializeField] private AudioClip soundGem;
    [SerializeField] private AudioClip soundPigGem;
    [SerializeField] private AudioClip eventFarmBuilding_BackgroundMusic;
    [SerializeField] private AudioClip textTypingSound;
    [SerializeField] private AudioClip dialoguePopupSound;
    [SerializeField] private AudioClip soundCompleteColorBtn;

    private AudioClip _currentMusic;

    public float MasterVolume
    {
        get
        {
            return PlayerPrefs.GetFloat(MASTER_KEY, 0f);
        }
        set
        {
            SetMasterVolume(value);
        }
    }
    public float MusicVolume
    {
        get
        {
            return GameController.Instance.useProfile.OnMusic ? 1 : 0;
        }
    }
    public float SoundVolume
    {
        get
        {
            return GameController.Instance.useProfile.OnSound ? 1 : 0;
        }
    }

    public void Init()
    {
        musicSource.volume = GameController.Instance.useProfile.OnMusic ? 0.2f : 0;
        effectSource.volume = GameController.Instance.useProfile.OnSound ? 1 : 0;
    }


    //Used to play single sound clips.
    public void PlaySingle(AudioClip clip, SourceAudio source = SourceAudio.Effect)
    {
        if (clip == null)
            return;
        switch (source)
        {
            case SourceAudio.Music:
                if (MusicVolume == 0) return;
                musicSource.clip = clip;
                musicSource.Play();
                break;
            case SourceAudio.Effect:
                if (SoundVolume == 0) return;

                effectSource.clip = clip;
                effectSource.Play();
                break;
            case SourceAudio.UI:
                if (SoundVolume == 0) return;
                soundUISource.clip = clip;
                soundUISource.Play();
                break;
        }

    }
    public void PlaySoundGem()
    {
        PlayOneShot(soundGem);
    }
    public void PlaySoundPigGem()
    {
        PlayOneShot(soundPigGem);
    }
    public void PauseBGMusic()
    {
        musicSource.Pause();
    }

    public void ResumeBGMusic()
    {
        musicSource.UnPause();
    }

    //Used to play single sound clips.
    public void PlayOneShot(AudioClip clip, SourceAudio source = SourceAudio.Effect)
    {
        if (clip == null)
            return;
        switch (source)
        {
            case SourceAudio.Music:
                if (MusicVolume == 0) return;
                musicSource.PlayOneShot(clip);
                break;
            case SourceAudio.Effect:
                if (SoundVolume == 0) return;
                effectSource.PlayOneShot(clip);
                break;
        }
    }

    //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    public void RandomizeSfx(params AudioClip[] clips)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        effectSource.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        effectSource.clip = clips[randomIndex];

        //Play the clip.
        effectSource.Play();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || _currentMusic == clip)
            return;
        SetMasterVolume(MasterVolume);
        SetMusicVolume(MusicVolume);
        SetSoundVolume(SoundVolume);
        _currentMusic = clip;
        StopMusic();
        musicSource.clip = clip;
        musicSource.PlayDelayed(delayTime);
    }
    public void RandomizeMusic(params AudioClip[] clips)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);
        var clip = clips[randomIndex];
        if (clip == null || _currentMusic == clip)
            return;
        _currentMusic = clip;
        StopMusic();
        //Set the clip to the clip at our randomly chosen index.
        musicSource.clip = clips[randomIndex];

        //Play the clip.
        musicSource.PlayDelayed(delayTime);
    }

    public void PauseMusic()
    {
        //Play the clip.
        musicSource.Pause();
    }
    public void UnPauseMusic()
    {
        //Play the clip.
        musicSource.UnPause();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
    public bool IsMusicPlaying()
    {
        return musicSource.isPlaying;
    }


    private void SetMasterVolume(float volume)
    {
        // mixer.SetFloat("MasterVolume", volume);
        // PlayerPrefs.SetFloat(MASTER_KEY, volume);
    }
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        //PlayerPrefs.SetFloat(MUSIC_KEY, volume);
    }
    public void SetSoundVolume(float volume)
    {
        effectSource.volume = volume;
        // PlayerPrefs.SetFloat(SOUND_KEY, volume);
    }

    #region === Play Sound ===
    public void PlayWinSound()
    {
        if (!GameController.Instance.useProfile.OnSound)
            return;
        // musicSource.clip = winMusic;
        //musicSource.Play();
        PlaySingle(winMusic, SourceAudio.Effect);
    }
    public void PlayBtnClick()
    {
        if (!GameController.Instance.useProfile.OnSound)
            return;
        PlaySingle(clickSound, SourceAudio.UI);
    }
    public void PlayBtnClaim()
    {
        if (!GameController.Instance.useProfile.OnSound)
            return;
        PlaySingle(soundClaim, SourceAudio.Effect);
    }
    public void PlayUseItem()
    {
        if (!GameController.Instance.useProfile.OnSound)
            return;
        PlaySingle(soundUseItem, SourceAudio.Effect);
    }
    public void PlaySoundCompleteColor()
    {
        if (!GameController.Instance.useProfile.OnSound)
            return;
        PlaySingle(soundCompleteColor, SourceAudio.UI);
    }
    public void PlayBGMusic()
    {
        musicSource.clip = BGMusic;
        musicSource.Play();
    }
    public void PlayBGHomeMusic()
    {
        musicSource.clip = BGHomeMusic;
        musicSource.Play();
    }
    public void PlayProgress()
    {
        if (!GameController.Instance.useProfile.OnSound)
            return;
        PlaySingle(progressComplete, SourceAudio.UI);
    }

    public void PlayClickSound()
    {
        PlaySingle(clickSound, SourceAudio.UI);
    }

    public void PlayEventFarmBuilding_BackgroundMusic()
    {
        musicSource.clip = eventFarmBuilding_BackgroundMusic;
        musicSource.Play();
    }

    public void PlayTextTypingSound()
    {
        PlaySingle(textTypingSound, SourceAudio.UI);
    }

    public void PlayDialoguePopupSound()
    {
        PlaySingle(dialoguePopupSound, SourceAudio.UI);
    }

    #endregion

    private float timmeMusic;
    public void PlayOneShot(NameMusic name)
    {
        if (SoundVolume == 0) return;
        if (clips.ContainsKey(name))
        {
            if (Mathf.Abs(timmeMusic - Time.time) < 0.01f)
            {
                effectSource.clip = clips[name].audioClip;
                effectSource.Play();
                AudioSource.PlayClipAtPoint(clips[name].audioClip, Vector3.zero, 1);
                timmeMusic = Time.time;
            }


        }
    }
    System.DateTime oldTime = System.DateTime.MinValue;
    public void PlayShotPiano(AudioSource audio)
    {
        if (!GameController.Instance.useProfile.OnSound)
            return;
        PianoTileData pianoTile = pianoDatas[0];
        float timer = pianoTile.timer;
        if (TimeManager.CaculateDetalTime(oldTime, System.DateTime.Now) > timer * 1000)
        {
            if (GameData.pianoCurrent >= pianoTile.clips.Length)
                GameData.pianoCurrent = 0;
            audio.clip = pianoTile.clips[GameData.pianoCurrent];
            GameData.pianoCurrent++;
            audio.Play();
            oldTime = System.DateTime.Now;
        }
    }


    public void PlayOneShot(AudioClip clip, AudioSource source, float volume = 1)
    {
        if (SoundVolume == 0) return;

        source.volume = volume;
        source.clip = clip;
        source.Play();
    }

    private void Update()
    {
        ClipDelayHandle();
    }

    private void ClipDelayHandle()
    {

        //foreach (var clip in clips)
        //{
        //    if (clip.Value.isActiving)
        //    {
        //        clip.Value.timmer -= Time.deltaTime;
        //        if (clip.Value.timmer < 0)
        //        {
        //            clip.Value.timmer = 0;
        //            clip.Value.isActiving = false;
        //        }
        //    }
        //}
    }
}
