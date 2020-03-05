using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;
    public AudioSource UiAudioSource;
    public AudioSource FxAudioSource;
    public AudioSource BackgroundAudioSource;
    public AudioSource StingerAudioSource;
    public AudioSource MusicAudioSource;

    public enum UISound
    {
        GuidePopup
    }

    void Awake()
    {
        if (!Instance)
            Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayButtonClick()
    {
        UiAudioSource.PlayOneShot(SoundBank.GetSound(SoundBank.UiSound.ButtonClick));

    }

    public static void PlayShopBell()
    {
        Instance.FxAudioSource.PlayOneShot(SoundBank.GetSound(SoundBank.FXSound.DoorBell));
    }

    public void PlayMapCLick()
    {
        UiAudioSource.PlayOneShot(SoundBank.GetSound(SoundBank.UiSound.MapClick));
    }

    public static void PlayLevelup()
    {
        Instance.UiAudioSource.PlayOneShot(SoundBank.GetSound(SoundBank.UiSound.LevelUp));
    }

    public static void PlayEvent()
    {
        Instance.UiAudioSource.PlayOneShot(SoundBank.GetSound(SoundBank.UiSound.Event));
    }

    public static void PlayMenuPopup()
    {
        Instance.UiAudioSource.PlayOneShot(SoundBank.GetSound(SoundBank.UiSound.PopUpOption));
    }

    public static void PlayGameStart()
    {
        Instance.StingerAudioSource.PlayOneShot(SoundBank.GetSound(SoundBank.Stinger.GameStart));
    }
    public static void PlayGameLoss()
    {
        Instance.UiAudioSource.PlayOneShot(SoundBank.GetSound(SoundBank.Stinger.GameLoss));
    }

    public static void PlayStinger(SoundBank.Stinger type)
    {
        Instance.StingerAudioSource.PlayOneShot(SoundBank.GetSound(type));

    }
    
    //TODO: create fade
    public static void ChangeMusic(SoundBank.Music type)
    {
        if (type == SoundBank.Music.NoMusic)
        {
            Instance.MusicAudioSource.clip = null;
            Instance.MusicAudioSource.Stop();
        }

        var f = SoundBank.GetSound(type);
        
        if (Instance.MusicAudioSource.clip != f)
        {
            Instance.MusicAudioSource.clip = f;
            Instance.MusicAudioSource.Play();
        }
    }
    public static void ChangeBackground(SoundBank.Background type)
    {
        var f = SoundBank.GetSound(type);


        if (Instance.BackgroundAudioSource.clip != f)
        {
            Instance.BackgroundAudioSource.clip = f;
            Instance.BackgroundAudioSource.Play();
        }
    }

    internal static void PlaySound(UISound guidePopup)
    {
        throw new NotImplementedException();
    }

    internal static void PlaySpeech(TextWithSpeech textWithSpeech)
    {
        throw new NotImplementedException();
    }

    internal static bool SpeechPlaying()
    {
        throw new NotImplementedException();
    }
}
