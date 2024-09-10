using System;
using System.Collections;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource backgroundMusicAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;

    private AudioDatabase audioDatabase = null;
    private float backgroundMusicVolumeRate = 1.0f;
    private float soundEffectVolumeRate = 1.0f;

    private Coroutine backgroundMusicCoroutine = null;

    public void PlayBackgroundMusic( AudioClip clip, float volumeScale = 1.0f, bool isLoop = true, float loopStartTime = 0.0f )
    {
        this.backgroundMusicAudioSource.clip = clip;
        this.backgroundMusicAudioSource.volume = volumeScale * backgroundMusicVolumeRate;
        this.backgroundMusicAudioSource.Play();

        if (isLoop)
        {
            this.backgroundMusicCoroutine = StartCoroutine( RunPlayingBackgroundMusicInLoop( this.backgroundMusicAudioSource, loopStartTime ) );
        }
    }

    public void StopBackgroundMusic()
    {
        if (this.backgroundMusicCoroutine != null)
        {
            StopCoroutine( this.backgroundMusicCoroutine );
            this.backgroundMusicCoroutine = null;
        }

        this.backgroundMusicAudioSource.Stop();
    }

    public void PlaySoundEffect( AudioClip clip, float volumeScale = 1.0f )
    {
        if (clip != null)
        {
            this.soundEffectAudioSource.PlayOneShot( clip, volumeScale * this.soundEffectVolumeRate );
        }
    }

    public void PlaySoundEffect( string audioId, Action onCompleteCallback )
    {
        PlaySoundEffect( audioId, 1.0f, onCompleteCallback );
    }

    public void PlaySoundEffect( string audioId, float volumeScale = 1.0f, Action onCompleteCallback = null )
    {
        if (this.audioDatabase == null)
        {
            Debug.LogError( "The audio database is not set up yet." );
        }

        AudioDatabase.AudioData _audioData = this.audioDatabase.GetAudioDataById( audioId );
        if (_audioData != null)
        {
            AudioClip _clip = _audioData.GetClip();

            if (_clip != null)
            {
                PlaySoundEffect( _clip, volumeScale );
                StartCoroutine( WaitAndCallback( _clip.length, onCompleteCallback ) );
            }
            else
            {
                Debug.LogError( $"The audio clip for the audio ID of \"{ audioId }\" is missing." );
            }
        }
        else
        {
            Debug.LogError( $"The audio ID of \"{ audioId }\" is not found." );
        }
    }

    public void SetUpAudioDatabase( AudioDatabase audioDatabase )
    {
        this.audioDatabase = audioDatabase;
    }

    private IEnumerator WaitAndCallback( float delay, Action callback )
    {
        yield return new WaitForSecondsRealtime( delay );
        callback?.Invoke();
    }

    private IEnumerator RunPlayingBackgroundMusicInLoop( AudioSource source, float loopStartTime )
    {
        while ( true )
        {
            yield return new WaitUntil( () => !source.isPlaying );
            source.time = loopStartTime;
            source.Play();
        }
    }
}
