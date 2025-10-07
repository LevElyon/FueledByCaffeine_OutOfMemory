using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float dodgeSpeed = 10f;
    public float dodgeDuration = 0.3f;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private PlayerAnimationController animController;

    // Dodge tracking
    private bool isDodging = false;
    private float dodgeTimer = 0f;
    private Vector2 dodgeDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<PlayerAnimationController>();
    }

    void FixedUpdate()
    {
        if (isDodging)
        {
            // Handle dodge movement
            rb.linearVelocity = dodgeDirection * dodgeSpeed;
            dodgeTimer -= Time.fixedDeltaTime;

            if (dodgeTimer <= 0)
            {
                isDodging = false;
            }
        }
        else if (!animController.IsAttacking() && !animController.IsThrowing())
        {
            // Normal movement (only if not attacking or throwing)
            Vector2 movement = moveInput * moveSpeed;
            rb.linearVelocity = movement;

            // Update animation based on movement
            animController.SetMovementAnimation(moveInput);
        }
        else
        {
            // Stop movement when attacking or throwing
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnAttack(InputValue value)
    {
        if (!animController.IsDodging() && !animController.IsAttacking() && !animController.IsThrowing())
        {
            animController.TriggerAttack();
        }
    }

    public void OnDodge(InputValue value)
    {
        if (!animController.IsDodging() && !animController.IsAttacking() && !animController.IsThrowing() && moveInput.magnitude > 0.1f)
        {
            isDodging = true;
            dodgeTimer = dodgeDuration;
            dodgeDirection = moveInput.normalized;
            animController.TriggerDodge();
        }
    }

    // NEW: Add throw function
    public void OnThrow(InputValue value)
    {
        if (!animController.IsDodging() && !animController.IsAttacking() && !animController.IsThrowing())
        {
            animController.TriggerThrow();
        }
    }

    public void TakeDamage()
    {
        animController.TriggerHit();
    }
}