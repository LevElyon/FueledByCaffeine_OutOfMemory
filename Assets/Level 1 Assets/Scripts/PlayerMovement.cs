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
    private Rigidbody2D rb;
    private PlayerAnimationController animController;
    private PlayerInput playerInputAsset;
    private BlockParryController blockParryController;

    // Dodge tracking
    private bool isDodging = false;
    private float dodgeTimer = 0f;
    private Vector2 dodgeDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<PlayerAnimationController>();
        blockParryController = GetComponent<BlockParryController>();
        playerInputAsset = new PlayerInput();
        playerInputAsset.Player.Enable();
    }

    void FixedUpdate()
    {
        if (isDodging)
        {
            // Dodge is active - ONLY apply dodge velocity with easing
            dodgeTimer += Time.fixedDeltaTime;
            // Calculate progress (0 to 1)
            float progress = Mathf.Clamp01(dodgeTimer / dodgeDuration);
            // Apply easing: starts FAST (1.0), ends SLOW (0.0)
            float easedValue = EaseOutQuad(progress);
            float currentSpeed = easedValue * dodgeSpeed;
            // Set velocity
            rb.linearVelocity = dodgeDirection * currentSpeed;
            // Check if dodge is finished
            if (dodgeTimer >= dodgeDuration)
            {
                isDodging = false;
                rb.linearVelocity = Vector2.zero;
                // Read the current input state directly
                moveInput = playerInputAsset.Player.Move.ReadValue<Vector2>();
            }
        }
        else if (blockParryController.isBlocking || blockParryController.isParrying)  // ADD isParrying CHECK
        {
            rb.linearVelocity = Vector2.zero;
            animController.SetMovementAnimation(Vector2.zero);  // Reset movement animation
        }
        else if (!animController.IsAttacking() && !animController.IsThrowing() && !blockParryController.isBlocking)
        {
            // Normal movement - BUT NOT while blocking
            Vector2 movement = moveInput * moveSpeed;
            rb.linearVelocity = movement;
            animController.SetMovementAnimation(moveInput);
        }
        else
        {
            // Stop movement when attacking or throwing
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
        // Only apply input if not dodging
        if (!isDodging)
        {
            moveInput = value.Get<Vector2>();
        }
    }

    public void OnAttack(InputValue value)
    {
        // Don't attack while dodging, blocking, or parrying
        if (isDodging || blockParryController.isBlocking || blockParryController.IsParryAnimationPlaying())
            return;

        if (!animController.IsAttacking() && !animController.IsThrowing())
        {
            animController.TriggerAttack();
        }
    }

    public void OnDodge(InputValue value)
    {
        // Only trigger on button press
        if (!value.isPressed)
            return;

        // Can't dodge if already dodging, no movement input, blocking, or parrying
        if (isDodging || moveInput.magnitude < 0.1f || blockParryController.isBlocking || blockParryController.IsParryAnimationPlaying())
            return;

        // Prevent dodge during attack/throw
        if (animController.IsAttacking() || animController.IsThrowing())
            return;

        // Start dodge
        isDodging = true;
        dodgeTimer = 0f;
        dodgeDirection = moveInput.normalized;
        moveInput = Vector2.zero; // Clear input during dodge
        animController.TriggerDodge();
    }

    public void OnThrow(InputValue value)
    {
        // Don't throw while dodging, blocking, or parrying
        if (isDodging || blockParryController.isBlocking || blockParryController.IsParryAnimationPlaying())
            return;

        if (!animController.IsAttacking() && !animController.IsThrowing())
        {
            animController.TriggerThrow();
        }
    }

    public void OnBlock(InputValue value)
    {
        // Only block if not already attacking, throwing, or dodging
        if (animController.IsAttacking() || animController.IsThrowing() || isDodging)
            return;

        Debug.Log("OnBlock called - isPressed: " + value.isPressed);

        if (value.isPressed)
        {
            Debug.Log("Block PRESSED");
            blockParryController.StartBlock();
        }
        else
        {
            Debug.Log("Block RELEASED");
            blockParryController.EndBlock();
        }
    }

    public void TakeDamage()
    {
        animController.TriggerHit();
    }

    void OnDestroy()
    {
        playerInputAsset?.Dispose();
    }
}