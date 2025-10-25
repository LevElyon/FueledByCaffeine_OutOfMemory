using UnityEngine;

/// <summary>
/// Manages the player's stamina system.
/// Independent component that tracks stamina and provides simple public methods.
/// External systems query this to determine if actions are affordable.
/// </summary>
public class PlayerStaminaController : MonoBehaviour
{
    [Header("Stamina Pool")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;

    [Header("Regeneration")]
    public float regenerationRate = 15f; // Stamina per second when not acting
    public float regenStartDelay = 0.5f; // Delay before regen starts after consuming stamina

    [Header("Action Costs")]
    public float blockStaminaCost = 10f;
    public float parryStaminaRefund = 5f; // Refund when successfully parrying (less than cost to incentivize timing)
    public float dodgeStaminaCost = 20f;
    public float attackStaminaCost = 15f;
    public float throwStaminaCost = 25f;

    private float timeSinceLastConsumption = 0f;

    void Start()
    {
        // Initialize stamina to max
        currentStamina = maxStamina;
    }

    void Update()
    {
        // Track time since last stamina consumption
        timeSinceLastConsumption += Time.deltaTime;

        // Regenerate stamina if enough time has passed since last consumption
        if (timeSinceLastConsumption >= regenStartDelay)
        {
            RegenerateStamina();
        }
    }

    // ========== PUBLIC METHODS (Called by other systems) ==========

    /// <summary>
    /// Attempts to consume stamina for an action.
    /// Returns true if successful, false if not enough stamina.
    /// </summary>
    public bool TryConsumeStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            timeSinceLastConsumption = 0f; // Reset regen timer
            Debug.Log($"Stamina consumed: {amount}. Current: {currentStamina}");
            return true;
        }
        else
        {
            Debug.Log($"Not enough stamina! Need: {amount}, Have: {currentStamina}");
            return false;
        }
    }

    /// <summary>
    /// Refunds stamina (for successful parries, etc.)
    /// </summary>
    public void RefundStamina(float amount)
    {
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
        Debug.Log($"Stamina refunded: {amount}. Current: {currentStamina}");
    }

    /// <summary>
    /// Check if player can afford an action without consuming stamina
    /// </summary>
    public bool CanAfford(float cost)
    {
        return currentStamina >= cost;
    }

    /// <summary>
    /// Get stamina as a percentage (0 to 1) for UI bars
    /// </summary>
    public float GetStaminaPercent()
    {
        return currentStamina / maxStamina;
    }

    // ========== PRIVATE METHODS ==========

    /// <summary>
    /// Regenerates stamina over time
    /// </summary>
    private void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina = Mathf.Min(currentStamina + regenerationRate * Time.deltaTime, maxStamina);
        }
    }
}