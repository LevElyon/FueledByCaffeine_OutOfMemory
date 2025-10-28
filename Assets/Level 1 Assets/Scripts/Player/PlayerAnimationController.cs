using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerRecoveryController recoveryController;
    public SoundManager SoundManager;
    [SerializeField] private PlayerHealthController playerHealthController;

    private static readonly string PARAM_SPEED = "Speed";
    private static readonly string PARAM_IS_ATTACKING = "IsAttacking";
    private static readonly string PARAM_IS_DODGING = "IsDodging";
    private static readonly string PARAM_IS_THROWING = "IsThrowing";  // NEW
    private static readonly string PARAM_IS_HIT = "IsHit";
    private static readonly string PARAM_IS_DEAD = "IsDead";

    private bool isAttacking = false;
    private bool isDodging = false;
    private bool isThrowing = false;  // NEW
    private bool isHit = false;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        recoveryController = GetComponent<PlayerRecoveryController>();
    }

    public void SetMovementAnimation(Vector2 movement)
    {
        if (isAttacking || isDodging || isThrowing || isHit)  // UPDATED
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
        if (isAttacking || isDodging || isThrowing || isHit)  // UPDATED
            return;

        animator.SetTrigger(PARAM_IS_ATTACKING);
        SoundManager.SFXSource.PlayOneShot(SoundManager.SoundEffects[0], 1);

        isAttacking = true;
    }

    public void TriggerDodge()
    {
        if (isAttacking || isDodging || isThrowing || isHit)  // UPDATED
            return;

        animator.SetTrigger(PARAM_IS_DODGING);
        SoundManager.SFXSource.PlayOneShot(SoundManager.SoundEffects[1], 1);
        playerHealthController.TriggerInvulnerability();
        isDodging = true;
    }

    // NEW: Throw function
    public void TriggerThrow()
    {
        if (isAttacking || isDodging || isThrowing || isHit)
            return;

        animator.SetTrigger(PARAM_IS_THROWING);
        SoundManager.SFXSource.PlayOneShot(SoundManager.SoundEffects[3], 1);
        isThrowing = true;
    }

    public void TriggerHit()
    {
        animator.SetTrigger(PARAM_IS_HIT);
        SoundManager.SFXSource.PlayOneShot(SoundManager.SoundEffects[2], 1);
        isHit = true;
    }

    /// <summary>
    /// Force reset all action states (called when interrupted by knockback)
    /// </summary>
    public void ForceResetAllActions()
    {
        isAttacking = false;
        isThrowing = false;
        isDodging = false;
        isHit = true; // Set to true because we're about to play hit animation

        animator.SetFloat(PARAM_SPEED, 0f);

#if UNITY_EDITOR
        Debug.Log("All animation states reset - ready for hit animation");
#endif
    }

    public void TriggerDeath()
    {
        if (isDead) return; // Already dead

        animator.SetTrigger(PARAM_IS_DEAD);
        SoundManager.SFXSource.PlayOneShot(SoundManager.SoundEffects[4], 1);
        isDead = true;

#if UNITY_EDITOR
        Debug.Log("Death animation triggered");
#endif
    }


    // Animation Events
    public void OnAttackComplete()
    {
        isAttacking = false;
        animator.SetFloat(PARAM_SPEED, 0f);
#if UNITY_EDITOR
        Debug.Log("Attack animation complete");
#endif
    }

    public void OnDodgeComplete()
    {
        isDodging = false;
        animator.SetFloat(PARAM_SPEED, 0f);
#if UNITY_EDITOR
        Debug.Log("Dodge animation complete");
#endif
    }

    public void OnThrowComplete()
    {
        isThrowing = false;
        animator.SetFloat(PARAM_SPEED, 0f);
#if UNITY_EDITOR
        Debug.Log("Throw animation complete");
#endif
    }

    public void OnHitComplete()
    {
        isHit = false;
        recoveryController.ForceRecovery(0f);
        animator.SetFloat(PARAM_SPEED, 0f);
#if UNITY_EDITOR
        Debug.Log("Hit animation complete - recovery cleared");
#endif
    }

    public void OnDeathStart()
    {
#if UNITY_EDITOR
        Debug.Log("Death animation started");
#endif
    }

    public void OnDeathComplete()
    {
        isDead = true; // Stay dead
        animator.SetFloat(PARAM_SPEED, 0f); // Stop any movement animation

#if UNITY_EDITOR
        Debug.Log("Death animation complete - Player is dead");
#endif
    }

    public void OnAttackStart()
    {
        Debug.Log("OnAttackStart called!");
        GetComponent<PlayerAttackHitbox>().ActivateHitbox();
    }

    public void OnAttackEnd()
    {
        Debug.Log("OnAttackEnd called!");
        GetComponent<PlayerAttackHitbox>().DeactivateHitbox();
    }


    public void TriggerBlock()
    {
        animator.SetBool("IsBlocking", true);
    }

    public void StopBlock()
    {
        animator.SetBool("IsBlocking", false);
    }

    public void TriggerParry()
    {
        UnityEngine.Debug.Log("TriggerParry() called - setting IsParrying to true");
        animator.SetBool("IsParrying", true);
    }

    public void OnParryEnd()
    {
        GetComponent<PlayerBlockParryController>().OnParryEnd();
    }

    // Getters
    public bool IsAttacking() { return isAttacking; }
    public bool IsDodging() { return isDodging; }
    public bool IsThrowing() { return isThrowing; }
    public bool IsHit() { return isHit; }

    public bool IsDead() { return isDead; }
}