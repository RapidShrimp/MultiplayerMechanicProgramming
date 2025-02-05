using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[Singleton]
public class SFX_AudioManager : MonoBehaviour
{
    public static SFX_AudioManager Singleton { get; private set; }

    AudioSource CurrentMusic = null;

    SFX_Item[] SoundsNearby = new SFX_Item[0]; 
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Singleton = this;
    }

    public void ToggleMuteAudio()
    {
        AudioListener Listener = GetComponent<AudioListener>();
        Listener.enabled = !Listener.enabled;
    }

    public void SwapPlayer(GameObject ListenTo)
    {
        transform.parent = ListenTo.transform.parent;
        transform.localPosition = Vector3.zero;
        foreach (SFX_Item SFX in SoundsNearby) 
        {
            SFX.OnPlaySFX -= PlaySoundToPlayer;
        }
        SoundsNearby = ListenTo.GetComponentsInChildren<SFX_Item>();
        foreach (SFX_Item SFX in SoundsNearby)
        {
            SFX.OnPlaySFX += PlaySoundToPlayer;
        }
    }

    public void PlaySoundToPlayer(AudioSource Sound,float Volume,float Pitch)
    {
        if(Sound.isPlaying) {Sound.Stop(); }
        Sound.volume = Volume;
        Sound.pitch = Pitch;
        Sound.Play();
    }

    public void SwapToMusic(AudioSource Music1, float Fadeout = 0.5f, float FadeIn = 1.0f, float Volume = 0.5f, float FadeGap = 0.5f)
    {
        if (CurrentMusic == null) 
        {
            CurrentMusic = Music1;
            FadeMusic(CurrentMusic, FadeIn, Volume);
            return;
        }

        StartCoroutine(SwapBetweenMusic(CurrentMusic, Music1, Fadeout, FadeIn, Volume, FadeGap));

    }


    public void FadeMusic(AudioSource Music, float fadeDuration, float DesiredVolume)
    {
        StartCoroutine(OnFadeMusic(Music, fadeDuration, DesiredVolume));
    }

    IEnumerator OnFadeMusic(AudioSource Audio, float fadeTime, float FadeVolume)
    {
        float TimeElapsed = 0;
        float InitialVolume = Audio.volume;
        if (!Audio.isPlaying) 
        {
            InitialVolume = 0;
            Audio.Play();
        }
        while (TimeElapsed < fadeTime)
        {
            Audio.volume = Mathf.Lerp(InitialVolume, FadeVolume, TimeElapsed / fadeTime);
            TimeElapsed += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        if (FadeVolume == 0) 
        {
            Audio.Stop();
        }
    }

    IEnumerator SwapBetweenMusic(AudioSource Music1, AudioSource Music2, float Fadeout = 0.5f, float FadeIn = 1.0f, float Volume = 0.5f, float FadeGap = 0.5f)
    {
        FadeMusic(Music1, Fadeout, 0);
        yield return new WaitForSeconds(Fadeout + FadeGap);
        CurrentMusic = Music2;
        FadeMusic(Music2, FadeIn, Volume);
    }
}


