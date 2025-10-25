using UnityEngine;


public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] audios;
    public static SoundManager soundManager;
    public AudioSource MusicSource;
    public AudioSource SFXSource;

    public AudioClip[] SoundEffects;
    public AudioClip[] BGMusic;

    private void Awake()
    {
        soundManager = this;
    }

    private void Start()
    {
        
    }

    public static void PlaySFX(int index, float volume)
    {
        soundManager.SFXSource.PlayOneShot(soundManager.audios[index], volume);
    }

    public static void PlayBGM(int index, float volume)
    {
        soundManager.MusicSource.PlayOneShot(soundManager.BGMusic[index], volume);
    }
}
