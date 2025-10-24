using UnityEngine;

/// <summary>
/// Manages the player's blocking and parrying mechanics.
/// Exposes simple flags that external systems (boss) can query.
/// Follows the same architecture as PlayerAttackHitbox.cs
/// </summary>
public class BlockParryController : MonoBehaviour
{
    [Header("Block State Flags (Query These From Outside Systems)")]
    public bool isBlocking = false;           // Player is in blocking stance
    public bool isParrying = false;          // Player successfully parried (special state)

    [Header("Block Settings")]
    public float blockDamageReduction = 0.5f; // 50% damage while blocking (not parrying)
    public float parryDamageReduction = 0.0f; // 0% damage on successful parry

    [Header("Parry Window Settings")]
    public float parryWindowDuration = 0.3f;  // Time window to trigger parry after blocking starts

    private float parryActiveTime = 0f;       // How long player has been blocking (tracks window)
    private bool blockActive = false;         // Is the parry window currently open?

    private PlayerAnimationController animController;
    private Animator animator;

    void Start()
    {
        animController = GetComponent<PlayerAnimationController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // If player is blocking, track time for parry window
        if (isBlocking && blockActive)
        {
            parryActiveTime += Time.deltaTime;

            // Parry window expires if held too long
            if (parryActiveTime > parryWindowDuration)
            {
                blockActive = false;
            }
        }

        // If parry animation finishes, return to normal block or idle
        // (Handled by animation events and OnBlockEnd)
    }

    // ========== INPUT METHODS (Called from PlayerMovement input handlers) ==========

    /// <summary>
    /// Called when player presses block button
    /// </summary>
    public void StartBlock()
    {
        if (isBlocking) return; // Already blocking

        isBlocking = true;
        blockActive = true;
        parryActiveTime = 0f;
        isParrying = false;

        animController.TriggerBlock();
    }

    /// <summary>
    /// Called when player releases block button
    /// </summary>
    public void EndBlock()
    {
        isBlocking = false;
        blockActive = false;
        isParrying = false;
        parryActiveTime = 0f;

        animController.StopBlock();
    }

    // ========== PARRY TRIGGER (Called from boss when attack hits) ==========

    /// <summary>
    /// Boss calls this when its attack is about to hit the player.
    /// Returns true if parry was successful, false otherwise.
    /// </summary>
    public bool CheckParry()
    {
        // Can only parry if currently blocking and parry window is open
        if (isBlocking && blockActive && parryActiveTime < parryWindowDuration)
        {
            ExecuteParry();
            return true;
        }

        // Not in parry window, or not blocking
        return false;
    }

    /// <summary>
    /// Executes the parry action - called from CheckParry or directly if needed
    /// </summary>
    private void ExecuteParry()
    {
        isParrying = true;
        blockActive = false; // End parry window after successful parry

        // Trigger parry animation
        animController.TriggerParry();
    }

    // ========== ANIMATION CALLBACKS (Called from animation events) ==========

    /// <summary>
    /// Called from Block_Block animation end event
    /// Returns player to blocking stance or idle
    /// </summary>
    public void OnParryEnd()
    {
        isParrying = false;

        // If still holding block input, return to blocking stance
        if (isBlocking)
        {
            blockActive = true;
            parryActiveTime = 0f;
            animController.TriggerBlock();
        }
        else
        {
            animController.StopBlock();
        }
    }

    // ========== STATE QUERIES (Use these for debugging/UI) ==========

    /// <summary>
    /// Get the current damage reduction multiplier based on block/parry state
    /// </summary>
    public float GetDamageReductionMultiplier()
    {
        if (isParrying)
            return parryDamageReduction;
        else if (isBlocking)
            return blockDamageReduction;
        else
            return 1.0f; // No reduction
    }

    /// <summary>
    /// Get parry window progress (0 to 1) for UI visualization
    /// </summary>
    public float GetParryWindowProgress()
    {
        if (!isBlocking) return 0f;
        return Mathf.Clamp01(parryActiveTime / parryWindowDuration);
    }

    /// <summary>
    /// Check if player can currently perform other actions
    /// </summary>
    public bool CanPerformAction()
    {
        // Can act freely if not blocking/parrying
        return !isBlocking && !isParrying;
    }
}