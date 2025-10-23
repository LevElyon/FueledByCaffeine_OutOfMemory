using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float dodgeSpeed = 10f;
    public float dodgeDuration = 1.0f;
    public float attackDamage;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private PlayerAnimationController animController;
    private PlayerInput playerInputAsset;

    // Dodge tracking
    private bool isDodging = false;
    private float dodgeTimer = 0f;
    private Vector2 dodgeDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<PlayerAnimationController>();
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
        else
        {
            // Normal movement
            if (!animController.IsAttacking() && !animController.IsThrowing())
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
        // Can't attack while dodging
        if (isDodging)
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

        // Can't dodge if already dodging or if no movement input
        if (isDodging || moveInput.magnitude < 0.1f)
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
        // Can't throw while dodging
        if (isDodging)
            return;

        if (!animController.IsAttacking() && !animController.IsThrowing())
        {
            animController.TriggerThrow();
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