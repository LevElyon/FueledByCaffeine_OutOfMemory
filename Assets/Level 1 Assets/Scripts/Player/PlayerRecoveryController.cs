using UnityEngine;

/// <summary>
/// Manages the player's recovery/hitstun buffer system.
/// Independent component that tracks action recovery times and prevents action spam.
/// External systems (PlayerMovement, PlayerBlockParryController) query this to check if actions are allowed.
/// </summary>
public class PlayerRecoveryController : MonoBehaviour
{
    [Header("Recovery Times (seconds - time locked after each action)")]
    public float attackRecovery = 0.3f;
    public float throwRecovery = 0.4f;
    public float dodgeRecovery = 0.2f;
    public float blockRecovery = 0.1f;
    public float hitstunDuration = 0.15f; // Stun when taking damage

    private float recoveryTimer = 0f;
    private bool isInRecovery = false;

    void Update()
    {
        // Track recovery timer
        if (isInRecovery)
        {
            recoveryTimer -= Time.deltaTime;
            if (recoveryTimer <= 0f)
            {
                isInRecovery = false;
                recoveryTimer = 0f;
            }
        }
    }

    // ========== PUBLIC METHODS (Called by other systems) ==========

    /// <summary>
    /// Attempt to start recovery for an action.
    /// Returns true if recovery was started, false if already in recovery.
    /// </summary>
    public bool TryStartRecovery(float duration)
    {
        if (isInRecovery)
        {
            Debug.Log($"Already in recovery! {recoveryTimer:F2}s remaining");
            return false;
        }

        recoveryTimer = duration;
        isInRecovery = true;
        Debug.Log($"Recovery started for {duration}s");
        return true;
    }

    /// <summary>
    /// Force start recovery, overriding any existing recovery.
    /// Used for hitstun when taking damage.
    /// </summary>
    public void ForceRecovery(float duration)
    {
        recoveryTimer = duration;
        isInRecovery = true;
        Debug.Log($"Forced recovery for {duration}s");
    }

    /// <summary>
    /// Check if player is currently in recovery
    /// </summary>
    public bool IsInRecovery()
    {
        return isInRecovery;
    }

    /// <summary>
    /// Get recovery progress (0 to 1) for UI visualization
    /// Returns 0 if not in recovery
    /// </summary>
    public float GetRecoveryPercent()
    {
        if (!isInRecovery) return 0f;

        // Find max recovery time for normalization
        float maxRecovery = Mathf.Max(attackRecovery, throwRecovery, dodgeRecovery, blockRecovery, hitstunDuration);

        // Return progress (1.0 = just started, 0.0 = almost done)
        return 1f - Mathf.Clamp01(recoveryTimer / maxRecovery);
    }

    /// <summary>
    /// Get remaining recovery time in seconds
    /// </summary>
    public float GetRecoveryTimeRemaining()
    {
        return isInRecovery ? recoveryTimer : 0f;
    }

    // ========== CONVENIENCE METHODS (Optional shortcuts) ==========

    /// <summary>
    /// Start attack recovery
    /// </summary>
    public bool StartAttackRecovery()
    {
        return TryStartRecovery(attackRecovery);
    }

    /// <summary>
    /// Start throw recovery
    /// </summary>
    public bool StartThrowRecovery()
    {
        return TryStartRecovery(throwRecovery);
    }

    /// <summary>
    /// Start dodge recovery
    /// </summary>
    public bool StartDodgeRecovery()
    {
        return TryStartRecovery(dodgeRecovery);
    }

    /// <summary>
    /// Start block recovery
    /// </summary>
    public bool StartBlockRecovery()
    {
        return TryStartRecovery(blockRecovery);
    }

    /// <summary>
    /// Apply hitstun when taking damage
    /// </summary>
    public void ApplyHitstun()
    {
        ForceRecovery(hitstunDuration);
    }

    // ========== DEBUG ==========

    /// <summary>
    /// Log current recovery state (for debugging)
    /// </summary>
    public void DebugLogRecoveryState()
    {
        if (isInRecovery)
        {
            Debug.Log($"IN RECOVERY: {recoveryTimer:F2}s remaining ({GetRecoveryPercent() * 100:F1}%)");
        }
        else
        {
            Debug.Log("Not in recovery");
        }
    }
}