using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[AddComponentMenu("Feedbacks / PlayAudio")]
public class PlayAudio : Feedback
{
    public AudioClip Clip;
    public float Volume = 1f;
    public float Pitch = 1f;
    protected AudioSource _audioSource;
    GameObject temporaryAudioHost;

    public override void Initialization()
    {
        base.Initialization();
        temporaryAudioHost = new GameObject("TempAudio");
        SceneManager.MoveGameObjectToScene(temporaryAudioHost.gameObject, this.gameObject.scene);
        temporaryAudioHost.transform.position = transform.position;
        _audioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
    }


    protected override void ReadyToPlayFeedback()
    {
        base.ReadyToPlayFeedback();
        PlayAudioSource(temporaryAudioHost.GetComponent<AudioSource>(), Clip, Volume, Pitch);
    }

    protected virtual void PlayAudioSource(AudioSource audioSource, AudioClip sfx, float volume, float pitch)
    {
        // we set that audio source clip to the one in paramaters
        audioSource.clip = sfx;
        // we set the audio source volume to the one in parameters
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        // we set our loop setting
        audioSource.loop = false;
        // we start playing the sound
        audioSource.Play();
    }

    public override void StopFeedback()
    {
        base.StopFeedback();
    }

    protected override bool IsCurrentTaskFinished()
    {
        if (Clip && _audioSource) return !_audioSource.isPlaying;
        return true;
    }
}
