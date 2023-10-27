using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    private AudioSource _source;
    [SerializeField] private AudioClip[] audios;

    private void Awake() 
    { 
        if (Instance != null) Destroy(this);
        else Instance = this; 
        _source = GetComponent<AudioSource>();
    }

    public void PlayAudio(int numAudio, float volume=0.5f)
    {
        _source.PlayOneShot(audios[numAudio], volume);
    }
}