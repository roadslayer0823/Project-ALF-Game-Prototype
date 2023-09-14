using UnityEngine;

[CreateAssetMenu( fileName = "AudioDatabase", menuName = "ScriptableObjects/AudioDatabase", order = 3 )]
public class AudioDatabase: ScriptableObject
{
    [SerializeField] private AudioData[] audioDataArray = new AudioData[ 0 ];

    public AudioData GetAudioDataById( string id )
    {
        for (int i = 0; i < audioDataArray.Length; i++)
        {
            AudioData _audioData = audioDataArray[ i ];
            if (_audioData.GetId() == id)
            {
                return _audioData;
            }
        }

        return null;
    }

    [System.Serializable]
    public class AudioData
    {
        [SerializeField] private string id = "";
        [SerializeField] private AudioClip clip = null;

        public string GetId()
        {
            return this.id;
        }

        public AudioClip GetClip()
        {
            return this.clip;
        }
    }
}
