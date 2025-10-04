using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Text;

public class ButtonGlitchEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Text Component")]
    public TextMeshProUGUI textComponent;

    [Header("Static Glitch Settings")]
    public float totalGlitchDuration = 0.5f; // Total time glitch effect lasts on hover
    [Range(0f, 1f)]
    public float glitchChance = 0.3f; // Chance per frame to glitch
    [Range(0f, 1f)]
    public float characterCorruptionChance = 0.4f; // % of characters to corrupt
    public float glitchFlickerSpeed = 0.05f; // How long each glitch frame lasts

    [Header("Visual Settings")]
    public bool useColorGlitch = true;
    public Color staticColor1 = new Color(1f, 0f, 0f, 1f); // Red
    public Color staticColor2 = new Color(0f, 1f, 1f, 1f); // Cyan
    public Color staticColor3 = new Color(1f, 1f, 1f, 1f); // White

    [Header("Character Replacement")]
    public string glitchCharacters = "!@#$%^&*(){}[]<>?/\\|~`";

    private string originalText;
    private Color originalColor;
    private bool isGlitching = false;
    private bool hasFinishedGlitching = false;
    private float glitchTimer = 0f;
    private float totalGlitchTimer = 0f;
    private bool isCurrentlyGlitched = false;

    void Start()
    {
        if (textComponent != null)
        {
            originalText = textComponent.text;
            originalColor = textComponent.color;
        }
    }

    void Update()
    {
        if (!isGlitching || hasFinishedGlitching || textComponent == null) return;

        // Count down total glitch duration
        totalGlitchTimer -= Time.deltaTime;

        // Stop glitching after duration ends
        if (totalGlitchTimer <= 0f)
        {
            ResetText();
            hasFinishedGlitching = true;
            return;
        }

        // Handle individual glitch flickers
        glitchTimer -= Time.deltaTime;

        if (glitchTimer <= 0f)
        {
            if (Random.value < glitchChance)
            {
                ApplyStaticGlitch();
                glitchTimer = glitchFlickerSpeed;
                isCurrentlyGlitched = true;
            }
            else if (isCurrentlyGlitched)
            {
                ResetText();
                glitchTimer = Random.Range(0.01f, 0.1f);
                isCurrentlyGlitched = false;
            }
        }
    }

    void ApplyStaticGlitch()
    {
        StringBuilder glitchedText = new StringBuilder(originalText);

        // Randomly corrupt characters
        for (int i = 0; i < originalText.Length; i++)
        {
            if (Random.value < characterCorruptionChance)
            {
                if (originalText[i] != ' ') // Don't replace spaces
                {
                    // Random corruption type
                    float corruptionType = Random.value;

                    if (corruptionType < 0.3f)
                    {
                        // Replace with glitch character
                        glitchedText[i] = glitchCharacters[Random.Range(0, glitchCharacters.Length)];
                    }
                    else if (corruptionType < 0.6f)
                    {
                        // Duplicate original character
                        glitchedText[i] = originalText[Random.Range(0, originalText.Length)];
                    }
                    else
                    {
                        // Use original but will have color change
                        glitchedText[i] = originalText[i];
                    }
                }
            }
        }

        textComponent.text = glitchedText.ToString();

        // Random color static interference
        if (useColorGlitch)
        {
            float colorChoice = Random.value;
            if (colorChoice < 0.33f)
                textComponent.color = staticColor1;
            else if (colorChoice < 0.66f)
                textComponent.color = staticColor2;
            else
                textComponent.color = staticColor3;
        }
    }

    void ResetText()
    {
        if (textComponent != null)
        {
            textComponent.text = originalText;
            textComponent.color = originalColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isGlitching = true;
        hasFinishedGlitching = false; // Reset the flag
        totalGlitchTimer = totalGlitchDuration; // Start the glitch duration timer
        glitchTimer = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isGlitching = false;
        hasFinishedGlitching = false; // Reset for next hover
        ResetText();
    }
}