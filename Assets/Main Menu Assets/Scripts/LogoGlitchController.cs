using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LogoGlitchController : MonoBehaviour
{
    public RawImage logo;
    public Material glitchMaterial;
    public float normalDuration = 2f;
    public float glitchDuration = 0.3f;
    public float maxGlitchIntensity = 0.2f;
    public SoundManager SoundManager;

    private Material logoMaterialInstance;

    void Start()
    {
        if (!logo) logo = GetComponent<RawImage>();
        logoMaterialInstance = Instantiate(glitchMaterial);
        logo.material = logoMaterialInstance;
        logoMaterialInstance.SetFloat("_GlitchIntensity", 0f);

        StartCoroutine(GlitchLoop());
    }

    IEnumerator GlitchLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(normalDuration);
            SoundManager.SFXSource.PlayOneShot(SoundManager.SoundEffects[0], 1);

            float t = 0f;
            while (t < glitchDuration)
            {
                t += Time.deltaTime;
                float intensity = Random.Range(maxGlitchIntensity * 0.5f, maxGlitchIntensity);
                logoMaterialInstance.SetFloat("_GlitchIntensity", intensity);
                yield return null;
            }

            logoMaterialInstance.SetFloat("_GlitchIntensity", 0f);
        }
    }
}
