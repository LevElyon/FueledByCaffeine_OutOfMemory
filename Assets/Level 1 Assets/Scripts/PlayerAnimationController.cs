using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private static readonly string PARAM_SPEED = "Speed";
    private static readonly string PARAM_IS_ATTACKING = "IsAttacking";
    private static readonly string PARAM_IS_DODGING = "IsDodging";
    private static readonly string PARAM_IS_THROWING = "IsThrowing";  // NEW
    private static readonly string PARAM_HIT = "Hit";

    private bool isAttacking = false;
    private bool isDodging = false;
    private bool isThrowing = false;  // NEW

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetMovementAnimation(Vector2 movement)
    {
        if (isAttacking || isDodging || isThrowing)  // UPDATED
            return;

        float speed = movement.magnitude;
        animator.SetFloat(PARAM_SPEED, speed);

        if (movement.x != 0)
        {
            spriteRenderer.flipX = movement.x < 0;
        }
    }

    public void TriggerAttack()
    {
        if (isAttacking || isDodging || isThrowing)  // UPDATED
            return;

        animator.SetTrigger(PARAM_IS_ATTACKING);
        isAttacking = true;
    }

    public void TriggerDodge()
    {
        if (isAttacking || isDodging || isThrowing)  // UPDATED
            return;

        animator.SetTrigger(PARAM_IS_DODGING);
        isDodging = true;
    }

    // NEW: Throw function
    public void TriggerThrow()
    {
        if (isAttacking || isDodging || isThrowing)
            return;

        animator.SetTrigger(PARAM_IS_THROWING);
        isThrowing = true;
    }

    public void TriggerHit()
    {
        animator.SetTrigger(PARAM_HIT);
    }

    // Animation Events
    public void OnAttackComplete()
    {
        isAttacking = false;
    }

    public void OnDodgeComplete()
    {
        isDodging = false;
    }

    // NEW: Animation Event for throw
    public void OnThrowComplete()
    {
        isThrowing = false;
    }

    // Getters
    public bool IsAttacking() { return isAttacking; }
    public bool IsDodging() { return isDodging; }
    public bool IsThrowing() { return isThrowing; }  // NEW
}