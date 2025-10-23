using UnityEngine;


public enum SoundType 
{ 
    PlayerMove,
    PlayerAttack,
    PlayerDash,
    PlayerParry,
    PlayerHurt,
    BossMove,
    BossAttack,
    BossHurt,

}


[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] audios;
    public static SoundManager soundManager;
    private AudioSource audioSource;

    private void Awake()
    {
        soundManager = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume)
    {
        soundManager.audioSource.PlayOneShot(soundManager.audios[(int)sound], volume);
    }
}
