using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public SoundManager soundManager;
    public void LoadLevel1()
    {
        soundManager.SFXSource.PlayOneShot(soundManager.SoundEffects[3], 0.7f);
        SceneManager.LoadScene("Level 1"); // Ensure the scene name matches exactly
    }

    public void ExitGame()
    {
        soundManager.SFXSource.PlayOneShot(soundManager.SoundEffects[3], 0.7f);
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
#endif
    }

    public void AudioFiller()
    {
        soundManager.SFXSource.PlayOneShot(soundManager.SoundEffects[3], 0.7f);
    }
}
