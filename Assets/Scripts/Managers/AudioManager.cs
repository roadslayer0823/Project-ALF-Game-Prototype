using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource backgroundMusicAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;

    private AudioDatabase audioDatabase = null;
    private float backgroundMusicVolumeRate = 1.0f;
    private float soundEffectVolumeRate = 1.0f;

    public void PlayBackgroundMusic( AudioClip clip, float volumeScale = 1.0f, bool isLoop = true )
    {
        backgroundMusicAudioSource.clip = clip;
        backgroundMusicAudioSource.volume = volumeScale * backgroundMusicVolumeRate;
        backgroundMusicAudioSource.loop = isLoop;
        backgroundMusicAudioSource.Play();
    }

    public void PlaySoundEffect( AudioClip clip, float volumeScale = 1.0f )
    {
        if (clip != null)
        {
            soundEffectAudioSource.PlayOneShot( clip, volumeScale * soundEffectVolumeRate );
        }
    }

    public void PlaySoundEffect( string audioId, float volumeScale = 1.0f )
    {
        if (this.audioDatabase == null)
        {
            Debug.LogError( "The audio database is not set up yet." );
            return;
        }

        PlaySoundEffect( this.audioDatabase.GetAudioDataById( audioId ).GetClip(), volumeScale );
    }

    public void SetUpAudioDatabase( AudioDatabase audioDatabase )
    {
        this.audioDatabase = audioDatabase;
    }
}
