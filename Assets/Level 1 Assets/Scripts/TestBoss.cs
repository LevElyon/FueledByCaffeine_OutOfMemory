using UnityEngine;

public class TestBoss : MonoBehaviour
{
    private BlockParryController playerBlockParry;
    private float attackTimer = 0f;
    public float attackCooldown = 3f; // Attack every 3 seconds (to fit countdown)
    private bool countdown3Logged = false;
    private bool countdown2Logged = false;
    private bool countdown1Logged = false;

    void Start()
    {
        playerBlockParry = FindFirstObjectByType<BlockParryController>();
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        // Clear console and start new countdown cycle
        if (attackTimer >= attackCooldown - 3.1f && !countdown3Logged)
        {
            Debug.ClearDeveloperConsole();  // Clear previous logs
            Debug.Log("3...");
            countdown3Logged = true;
        }

        // Countdown: 2 seconds before attack
        if (attackTimer >= attackCooldown - 2f && !countdown2Logged)
        {
            Debug.Log("2...");
            countdown2Logged = true;
        }

        // Countdown: 1 second before attack
        if (attackTimer >= attackCooldown - 1f && !countdown1Logged)
        {
            Debug.Log("1...");
            countdown1Logged = true;
        }

        // Auto-attack
        if (attackTimer >= attackCooldown)
        {
            PerformAttack();
            attackTimer = 0f;
            countdown3Logged = false;
            countdown2Logged = false;
            countdown1Logged = false;
        }
    }

    void PerformAttack()
    {
        Debug.Log(">> BOSS ATTACKING NOW! <<");

        if (playerBlockParry != null)
        {
            // Check if player parried
            if (playerBlockParry.CheckParry())
            {
                Debug.Log("SUCCESS - PARRIED! Player took no damage!");
            }
            else if (playerBlockParry.isBlocking)
            {
                Debug.Log("BLOCKED - Player takes 50% damage!");
            }
            else
            {
                Debug.Log("HIT - Player takes full damage!");
            }
        }
    }
}