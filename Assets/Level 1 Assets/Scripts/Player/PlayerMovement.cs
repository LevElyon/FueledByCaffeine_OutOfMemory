using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float dodgeSpeed = 10f;
    public float dodgeDuration = 1.0f;

    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.right;  // NEW: Track last move direction for throw projectiles
    private Rigidbody2D rb;
    private PlayerAnimationController animController;
    private PlayerInput playerInputAsset;
    private PlayerBlockParryController blockParryController;
    private PlayerStaminaController staminaController;
    private PlayerRecoveryController recoveryController;
    private PlayerHealthController healthController;

    // Dodge tracking
    private bool isDodging = false;
    private float dodgeTimer = 0f;
    private Vector2 dodgeDirection;

    // Knockback tracking
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private Vector2 knockbackDirection;
    private float knockbackSpeed = 0f;

    // INPUT DEBOUNCING - ADD THIS
    private float lastActionInputTime = 0f;
    private float actionInputCooldown = 0.05f; // 50ms between actions

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<PlayerAnimationController>();
        blockParryController = GetComponent<PlayerBlockParryController>();
        staminaController = GetComponent<PlayerStaminaController>();
        recoveryController = GetComponent<PlayerRecoveryController>();
        healthController = GetComponent<PlayerHealthController>();
        playerInputAsset = new PlayerInput();
        playerInputAsset.Player.Enable();

#if UNITY_EDITOR
        Debug.Log("PlayerMovement initialized with all components");
#endif
    }

    void FixedUpdate()
    {
        // ADD THIS AT THE START
        if (animController.IsDead())
        {
            rb.linearVelocity = Vector2.zero;
            return; // Stop all movement
        }

        if (isKnockedBack)
        {
            // Knockback is active - apply knockback velocity with easing
            knockbackTimer += Time.fixedDeltaTime;
            float progress = Mathf.Clamp01(knockbackTimer / healthController.knockbackDuration);
            float easedValue = EaseOutQuad(progress);
            float currentSpeed = easedValue * knockbackSpeed;
            rb.linearVelocity = knockbackDirection * currentSpeed;

            if (knockbackTimer >= healthController.knockbackDuration)
            {
                isKnockedBack = false;
                rb.linearVelocity = Vector2.zero;
                moveInput = playerInputAsset.Player.Move.ReadValue<Vector2>();

#if UNITY_EDITOR
                Debug.Log("Knockback completed");
#endif
            }
        }
        else if (isDodging)
        {
            // Dodge is active - ONLY apply dodge velocity with easing
            dodgeTimer += Time.fixedDeltaTime;
            float progress = Mathf.Clamp01(dodgeTimer / dodgeDuration);
            float easedValue = EaseOutQuad(progress);
            float currentSpeed = easedValue * dodgeSpeed;
            rb.linearVelocity = dodgeDirection * currentSpeed;

            if (dodgeTimer >= dodgeDuration)
            {
                isDodging = false;
                rb.linearVelocity = Vector2.zero;
                moveInput = playerInputAsset.Player.Move.ReadValue<Vector2>();

#if UNITY_EDITOR
                Debug.Log("Dodge completed");
#endif
            }
        }
        else if (blockParryController.isBlocking || blockParryController.isParrying)
        {
            rb.linearVelocity = Vector2.zero;
            animController.SetMovementAnimation(Vector2.zero);
        }
        else if (recoveryController.IsInRecovery())
        {
            rb.linearVelocity = Vector2.zero;
            animController.SetMovementAnimation(Vector2.zero);
        }
        else if (!animController.IsAttacking() && !animController.IsThrowing() && !blockParryController.isBlocking && !animController.IsHit())
        {
            Vector2 movement = moveInput * moveSpeed;
            rb.linearVelocity = movement;
            animController.SetMovementAnimation(moveInput);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private float EaseOutQuad(float t)
    {
        t = Mathf.Clamp01(t);
        return 1f - (t * t);
    }

    public void OnMove(InputValue value)
    {
        if (animController.IsDead())
            return; // Ignore movement input

        if (!isDodging && !isKnockedBack)
        {
            moveInput = value.Get<Vector2>();

            // NEW: Track last non-zero direction for throw projectiles
            if (moveInput.magnitude > 0.1f)
            {
                lastMoveDirection = moveInput.normalized;
            }
        }
    }

    // NEW: Getter for last move direction (used by throw projectile system)
    public Vector2 GetLastMoveDirection()
    {
        return lastMoveDirection;
    }

    public void OnAttack(InputValue value)
    {
#if UNITY_EDITOR
        Debug.Log($"OnAttack called - Recovery: {recoveryController.IsInRecovery()}, Blocking: {blockParryController.isBlocking}");
#endif

        if (animController.IsDead())
            return; // Ignore attack input

        // INPUT DEBOUNCING CHECK
        if (Time.time - lastActionInputTime < actionInputCooldown)
        {
#if UNITY_EDITOR
            Debug.Log("Attack blocked - input debounce!");
#endif
            return;
        }

        if (isDodging || isKnockedBack || blockParryController.isBlocking || blockParryController.IsParryAnimationPlaying() || recoveryController.IsInRecovery() || animController.IsHit())
        {
#if UNITY_EDITOR
            if (recoveryController.IsInRecovery())
                Debug.Log("Attack blocked by recovery!");
            if (isKnockedBack)
                Debug.Log("Attack blocked - knocked back!");
            if (animController.IsHit())
                Debug.Log("Attack blocked - hit animation playing!");
#endif
            return;
        }

        if (!animController.IsAttacking() && !animController.IsThrowing())
        {
            if (staminaController.TryConsumeStamina(staminaController.attackStaminaCost))
            {
                lastActionInputTime = Time.time; // RECORD INPUT TIME
                animController.TriggerAttack();
                recoveryController.StartAttackRecovery();

#if UNITY_EDITOR
                Debug.Log($"Attack triggered! Recovery: {recoveryController.GetRecoveryTimeRemaining():F2}s");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("Not enough stamina to attack!");
#endif
            }
        }
    }

    public void OnDodge(InputValue value)
    {
        if (animController.IsDead())
            return; // Ignore dodge input

        if (!value.isPressed)
            return;

#if UNITY_EDITOR
        Debug.Log($"OnDodge called - Recovery: {recoveryController.IsInRecovery()}, Input: {moveInput.magnitude:F2}");
#endif

        // INPUT DEBOUNCING CHECK
        if (Time.time - lastActionInputTime < actionInputCooldown)
        {
#if UNITY_EDITOR
            Debug.Log("Dodge blocked - input debounce!");
#endif
            return;
        }

        if (isDodging || isKnockedBack || moveInput.magnitude < 0.1f || blockParryController.isBlocking || blockParryController.IsParryAnimationPlaying() || recoveryController.IsInRecovery() || animController.IsHit())
        {
#if UNITY_EDITOR
            if (recoveryController.IsInRecovery())
                Debug.Log("Dodge blocked by recovery!");
            else if (isKnockedBack)
                Debug.Log("Dodge blocked - knocked back!");
            else if (moveInput.magnitude < 0.1f)
                Debug.Log("Dodge blocked - no movement input!");
            else if (blockParryController.isBlocking)
                Debug.Log("Dodge blocked - currently blocking!");
            else if (isDodging)
                Debug.Log("Dodge blocked - already dodging!");
            else if (animController.IsHit())
                Debug.Log("Dodge blocked - hit animation playing!");
#endif
            return;
        }

        if (animController.IsAttacking() || animController.IsThrowing())
        {
#if UNITY_EDITOR
            Debug.Log("Dodge blocked - attacking or throwing!");
#endif
            return;
        }

        if (!staminaController.TryConsumeStamina(staminaController.dodgeStaminaCost))
        {
#if UNITY_EDITOR
            Debug.Log("Not enough stamina to dodge!");
#endif
            return;
        }

        isDodging = true;
        dodgeTimer = 0f;
        dodgeDirection = moveInput.normalized;
        moveInput = Vector2.zero;
        lastActionInputTime = Time.time; // RECORD INPUT TIME
        animController.TriggerDodge();
        recoveryController.StartDodgeRecovery();

#if UNITY_EDITOR
        Debug.Log($"Dodge triggered! Direction: {dodgeDirection}, Recovery: {recoveryController.GetRecoveryTimeRemaining():F2}s");
#endif
    }

    public void OnThrow(InputValue value)
    {
#if UNITY_EDITOR
        Debug.Log($"OnThrow called - Recovery: {recoveryController.IsInRecovery()}, Blocking: {blockParryController.isBlocking}");
#endif

        if (animController.IsDead())
            return; // Ignore throw input

        // INPUT DEBOUNCING CHECK
        if (Time.time - lastActionInputTime < actionInputCooldown)
        {
#if UNITY_EDITOR
            Debug.Log("Throw blocked - input debounce!");
#endif
            return;
        }

        if (isDodging || isKnockedBack || blockParryController.isBlocking || blockParryController.IsParryAnimationPlaying() || recoveryController.IsInRecovery() || animController.IsHit())
        {
#if UNITY_EDITOR
            if (recoveryController.IsInRecovery())
                Debug.Log("Throw blocked by recovery!");
            if (isKnockedBack)
                Debug.Log("Throw blocked - knocked back!");
            if (animController.IsHit())
                Debug.Log("Throw blocked - hit animation playing!");
#endif
            return;
        }

        if (!animController.IsAttacking() && !animController.IsThrowing())
        {
            if (staminaController.TryConsumeStamina(staminaController.throwStaminaCost))
            {
                lastActionInputTime = Time.time; // RECORD INPUT TIME
                animController.TriggerThrow();
                recoveryController.StartThrowRecovery();

#if UNITY_EDITOR
                Debug.Log($"Throw triggered! Recovery: {recoveryController.GetRecoveryTimeRemaining():F2}s");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("Not enough stamina to throw!");
#endif
            }
        }
    }

    public void OnBlock(InputValue value)
    {
#if UNITY_EDITOR
        Debug.Log($"OnBlock called - isPressed: {value.isPressed}");
#endif

        if (animController.IsDead())
            return; // Ignore block input

        if (animController.IsAttacking() || animController.IsThrowing() || isDodging)
        {
#if UNITY_EDITOR
            Debug.Log("Block blocked - attacking, throwing, or dodging!");
#endif
            return;
        }

        // ========== SPECIAL HANDLING FOR BLOCK ==========
        // Block RELEASE is always allowed (even during knockback/hit)
        // Block PRESS can be blocked by knockback/hit/recovery

        if (value.isPressed)
        {
            // Block PRESS - check for knockback and hit state
            if (isKnockedBack || animController.IsHit())
            {
#if UNITY_EDITOR
                Debug.Log("Block PRESS blocked - knocked back or hit animation playing!");
#endif
                return;
            }

            if (recoveryController.IsInRecovery())
            {
#if UNITY_EDITOR
                Debug.Log($"Can't press block - in recovery! {recoveryController.GetRecoveryTimeRemaining():F2}s remaining");
#endif
                return;
            }

#if UNITY_EDITOR
            Debug.Log("Block PRESSED");
#endif
            blockParryController.StartBlock();
            recoveryController.StartBlockRecovery();
        }
        else
        {
            // Block RELEASE - always allowed
#if UNITY_EDITOR
            Debug.Log("Block RELEASED");
#endif
            blockParryController.EndBlock();
        }
    }

    // ========== KNOCKBACK SYSTEM ==========

    /// <summary>
    /// Apply knockback while player is blocking (no animation change).
    /// Player stays in block pose but gets pushed back.
    /// </summary>
    public void ApplyBlockKnockback(Vector2 direction, float knockbackDistance)
    {
        // Start knockback WITHOUT cancelling actions
        isKnockedBack = true;
        knockbackTimer = 0f;
        knockbackDirection = direction.normalized;
        knockbackSpeed = knockbackDistance / healthController.knockbackDuration;
        moveInput = Vector2.zero;

#if UNITY_EDITOR
        Debug.Log($"Block knockback applied! Direction: {knockbackDirection}, Distance: {knockbackDistance}, Duration: {healthController.knockbackDuration}s");
#endif
    }

    /// <summary>
    /// Apply knockback to the player in a given direction.
    /// Called from boss or other damage sources.
    /// </summary>
    public void ApplyKnockback(Vector2 direction, float knockbackDistance = -1f)
    {
        // Use provided distance, or default to full knockback distance
        if (knockbackDistance < 0f)
            knockbackDistance = healthController.fullKnockbackDistance;

        // Cancel all current actions (THIS is what makes it different from block knockback)
        CancelAllActions();

        // Start knockback
        isKnockedBack = true;
        knockbackTimer = 0f;
        knockbackDirection = direction.normalized;
        knockbackSpeed = knockbackDistance / healthController.knockbackDuration;
        moveInput = Vector2.zero;

#if UNITY_EDITOR
        Debug.Log($"Full knockback applied! Direction: {knockbackDirection}, Distance: {knockbackDistance}, Duration: {healthController.knockbackDuration}s");
#endif
    }

    /// <summary>
    /// Check if player is currently being knocked back
    /// </summary>
    public bool IsKnockedBack()
    {
        return isKnockedBack;
    }

    /// <summary>
    /// Cancel all current actions (attack, dodge, throw, block)
    /// Called when hit to interrupt actions
    /// </summary>
    private void CancelAllActions()
    {
        isDodging = false;
        animController.ForceResetAllActions();  // ← Reset all flags
        animController.TriggerHit();            // ← Set animator trigger
        blockParryController.EndBlock(); // Force end block
        recoveryController.ApplyHitstun(); // Apply hitstun lock

#if UNITY_EDITOR
        Debug.Log("All actions cancelled - player was hit!");
#endif
    }

    void OnDestroy()
    {
        playerInputAsset?.Dispose();
    }
}