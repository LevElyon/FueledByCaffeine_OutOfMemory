using UnityEngine;

public class TestBoss : MonoBehaviour
{
    private PlayerBlockParryController playerBlockParry;
    private PlayerHealthController playerHealth;
    private PlayerMovement playerMovement;
    private Transform playerTransform;

    [Header("Attack Settings")]
    public float baseDamage = 20f; // Base damage per attack
    public float attackCooldown = 3f; // Attack every 3 seconds

    private float attackTimer = 0f;
    private bool countdown1Logged = false;

    void Start()
    {
        playerBlockParry = FindFirstObjectByType<PlayerBlockParryController>();
        playerHealth = FindFirstObjectByType<PlayerHealthController>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerTransform = FindFirstObjectByType<PlayerMovement>().transform;

#if UNITY_EDITOR
        if (playerBlockParry == null)
            Debug.LogError("TestBoss: PlayerBlockParryController not found!");
        if (playerHealth == null)
            Debug.LogError("TestBoss: PlayerHealthController not found!");
        if (playerMovement == null)
            Debug.LogError("TestBoss: PlayerMovement not found!");
        if (playerTransform == null)
            Debug.LogError("TestBoss: Player Transform not found!");
#endif
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        // Countdown: 1 second before attack
        if (attackTimer >= attackCooldown - 1f && !countdown1Logged)
        {
#if UNITY_EDITOR
            Debug.Log("1...");
#endif
            countdown1Logged = true;
        }

        // Auto-attack
        if (attackTimer >= attackCooldown)
        {
            PerformAttack();
            attackTimer = 0f;
            countdown1Logged = false;
        }
    }

    void PerformAttack()
    {
#if UNITY_EDITOR
        Debug.Log(">> BOSS ATTACKING NOW! <<");
#endif

        if (playerBlockParry == null || playerHealth == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Boss: Missing player references!");
#endif
            return;
        }

        // Check if player parried
        if (playerBlockParry.CheckParry())
        {
#if UNITY_EDITOR
            Debug.Log("SUCCESS - PARRIED! Player took no damage!");
#endif
            return;
        }

        // Calculate knockback direction (away from boss towards player)
        Vector2 knockbackDirection = (playerTransform.position - transform.position).normalized;

        // Get damage multiplier based on block/parry state
        float damageMultiplier = playerBlockParry.GetDamageReductionMultiplier();
        float finalDamage = baseDamage * damageMultiplier;

        // Deal damage - player handles knockback internally based on blocking state
        if (playerHealth.TakeDamage(finalDamage, knockbackDirection))
        {
            // Damage was dealt (player wasn't invulnerable)
            if (playerBlockParry.isBlocking)
            {
#if UNITY_EDITOR
                Debug.Log($"BLOCKED - Player takes {finalDamage} damage (50% reduction)");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log($"HIT - Player takes {finalDamage} damage (full damage)");
#endif
            }
        }
        else
        {
            // Damage was blocked (player was invulnerable)
#if UNITY_EDITOR
            Debug.Log("HIT - But player is invulnerable!");
#endif
        }

#if UNITY_EDITOR
        Debug.Log($"Player health: {playerHealth.currentHealth}/{playerHealth.maxHealth}");
#endif
    }
}