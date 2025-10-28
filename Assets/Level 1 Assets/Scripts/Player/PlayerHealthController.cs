using UnityEngine;

/// <summary>
/// Manages the player's health system.
/// Independent component that tracks health, invulnerability, and knockback.
/// Handles knockback internally based on blocking state.
/// </summary>
public class PlayerHealthController : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Knockback Settings")]
    public float fullKnockbackDistance = 8f; // Renamed from knockbackDistance
    public float blockKnockbackDistance = 2f; // NEW: Small knockback when blocking normally
    public float knockbackDuration = 0.3f;

    [Header("Invulnerability Settings")]
    public float invulnerabilityDuration = 1.0f; // Duration of i-frames after hit

    private float invulnerabilityTimer = 0f;
    private bool isInvulnerable = false;

    private PlayerMovement playerMovement;
    private PlayerBlockParryController blockParryController;
    private PlayerAnimationController animController;

    void Start()
    {
        // Initialize health to max
        currentHealth = maxHealth;

        // Get references
        playerMovement = GetComponent<PlayerMovement>();
        blockParryController = GetComponent<PlayerBlockParryController>();
        animController = GetComponent<PlayerAnimationController>();
    }

    void Update()
    {
        // Track invulnerability timer
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            if (invulnerabilityTimer <= 0f)
            {
                isInvulnerable = false;
                invulnerabilityTimer = 0f;

#if UNITY_EDITOR
                Debug.Log("Invulnerability ended");
#endif
            }
        }

        // Check if player should die
        if (currentHealth <= 0f && !animController.IsDead())
        {
            animController.TriggerDeath();

#if UNITY_EDITOR
            Debug.Log("Player is dead!");
#endif
        }
    }

    // ========== PUBLIC METHODS (Called by other systems) ==========

    /// <summary>
    /// Apply damage to the player.
    /// Automatically handles knockback based on blocking state.
    /// Returns true if damage was taken, false if invulnerable.
    /// Boss should call this with knockback direction.
    /// </summary>
    public bool TakeDamage(float damageAmount, Vector2 knockbackDirection)
    {
        // PREVENT DAMAGE WHEN ALREADY DEAD
        if (animController.IsDead())
        {
#if UNITY_EDITOR
            Debug.Log("Damage blocked - player is already dead!");
#endif
            return false;
        }

        // Can't take damage if invulnerable
        if (isInvulnerable)
        {
#if UNITY_EDITOR
            Debug.Log("Damage blocked - player is invulnerable!");
#endif
            return false;
        }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0f); // Don't go below 0

        // Trigger invulnerability
        TriggerInvulnerability();

#if UNITY_EDITOR
        Debug.Log($"Player took {damageAmount} damage! Health: {currentHealth}/{maxHealth}");
#endif

        // Handle knockback internally based on blocking state
        HandleKnockback(knockbackDirection);

        return true;
    }

    /// <summary>
    /// Check if player is currently invulnerable
    /// </summary>
    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }

    /// <summary>
    /// Get health as a percentage (0 to 1) for UI bars
    /// </summary>
    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }

    /// <summary>
    /// Get remaining invulnerability time in seconds
    /// </summary>
    public float GetInvulnerabilityTimeRemaining()
    {
        return isInvulnerable ? invulnerabilityTimer : 0f;
    }

    /// <summary>
    /// Check if player is dead
    /// </summary>
    public bool IsDead()
    {
        return currentHealth <= 0f;
    }

    /// <summary>
    /// Heal the player
    /// </summary>
    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);

#if UNITY_EDITOR
        Debug.Log($"Player healed for {healAmount}! Health: {currentHealth}/{maxHealth}");
#endif
    }

    // ========== PRIVATE METHODS ==========

    /// <summary>
    /// Trigger invulnerability frames after taking damage
    /// </summary>
    public void TriggerInvulnerability()
    {
        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityDuration;

#if UNITY_EDITOR
        Debug.Log($"Invulnerability started for {invulnerabilityDuration}s");
#endif
    }

    /// <summary>
    /// Handle knockback based on blocking state.
    /// Blocking prevents knockback but doesn't prevent damage.
    /// </summary>
    private void HandleKnockback(Vector2 knockbackDirection)
    {
        // Check if player is blocking
        if (blockParryController.isBlocking)
        {
            // Blocking: Apply small knockback, stay in block pose
            if (playerMovement != null)
            {
                playerMovement.ApplyBlockKnockback(knockbackDirection, blockKnockbackDistance);
#if UNITY_EDITOR
                Debug.Log($"Block knockback applied! Direction: {knockbackDirection}");
#endif
            }
            return;
        }

        // Not blocking: Apply full knockback with hit animation
        if (playerMovement != null)
        {
            playerMovement.ApplyKnockback(knockbackDirection, fullKnockbackDistance);
#if UNITY_EDITOR
            Debug.Log($"Full knockback applied! Direction: {knockbackDirection}");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning("PlayerMovement reference not found for knockback!");
#endif
        }
    }

    // ========== DEBUG ==========

    /// <summary>
    /// Log current health state (for debugging)
    /// </summary>
    public void DebugLogHealthState()
    {
        Debug.Log($"Health: {currentHealth}/{maxHealth} ({GetHealthPercent() * 100:F1}%)");
        Debug.Log($"Invulnerable: {isInvulnerable} ({GetInvulnerabilityTimeRemaining():F2}s remaining)");
        Debug.Log($"Dead: {IsDead()}");
    }
}