using UnityEngine;

/// <summary>
/// Manages projectile spawning for the player's throw attack.
/// Spawns projectiles at a specific time during the throw animation with configurable offset and delay.
/// Supports 8-directional throwing based on player facing direction.
/// </summary>
public class PlayerThrowProjectileManager : MonoBehaviour
{
    [Header("Projectile Prefab")]
    public GameObject projectilePrefab;  // Prefab with PlayerThrowProjectile component

    [Header("Spawn Offset")]
    public Vector2 spawnOffset = new Vector2(1.5f, 0.5f);  // X and Y offset from player position

    [Header("Spawn Timing")]
    public float spawnDelayTime = 0.2f;  // Time into throw animation before projectile spawns

    private PlayerMovement playerMovement;
    private PlayerAnimationController animController;
    private SpriteRenderer spriteRenderer;

    // Spawn state tracking
    private float spawnTimer = 0f;
    private bool isWaitingToSpawn = false;
    private Vector2 throwDirection = Vector2.right;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        animController = GetComponent<PlayerAnimationController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

#if UNITY_EDITOR
        if (projectilePrefab == null)
            Debug.LogError("PlayerThrowProjectileManager: Projectile prefab not assigned!");
        if (playerMovement == null)
            Debug.LogError("PlayerThrowProjectileManager: PlayerMovement not found!");
        if (animController == null)
            Debug.LogError("PlayerThrowProjectileManager: PlayerAnimationController not found!");
        if (spriteRenderer == null)
            Debug.LogError("PlayerThrowProjectileManager: SpriteRenderer not found!");
#endif
    }

    void Update()
    {
        // Count down spawn timer
        if (isWaitingToSpawn)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnProjectile();
                isWaitingToSpawn = false;
            }
        }
    }

    /// <summary>
    /// Called when throw animation starts (from PlayerAnimationController or animation event)
    /// Begins countdown to spawn projectile
    /// </summary>
    public void OnThrowStarted()
    {
        // Determine throw direction from player's current facing
        throwDirection = DetermineThrowDirection();

        // Start spawn timer
        spawnTimer = spawnDelayTime;
        isWaitingToSpawn = true;
    }

    /// <summary>
    /// Determine the throw direction based on player's facing and last movement input
    /// </summary>
    private Vector2 DetermineThrowDirection()
    {
        // Get the player's last movement direction
        Vector2 direction = playerMovement.GetLastMoveDirection();

        // If no movement direction, use sprite facing direction
        if (direction.magnitude < 0.1f)
        {
            // If facing left, throw left; otherwise throw right
            direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        }

        return direction.normalized;
    }

    /// <summary>
    /// Spawns the projectile at the calculated position
    /// </summary>
    private void SpawnProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("PlayerThrowProjectileManager: Cannot spawn projectile, prefab is null!");
            return;
        }

        // Calculate spawn position with offset (considering direction for offset orientation)
        Vector2 actualOffset = spawnOffset;
        if (spriteRenderer.flipX)
        {
            actualOffset.x = -spawnOffset.x;  // Flip X offset if facing left
        }

        Vector3 spawnPos = transform.position + (Vector3)actualOffset;

        // Instantiate projectile
        GameObject projectileObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        // Initialize projectile with throw direction
        PlayerThrowProjectile projectile = projectileObj.GetComponent<PlayerThrowProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(throwDirection);
        }
        else
        {
            Debug.LogError("PlayerThrowProjectileManager: Projectile prefab doesn't have PlayerThrowProjectile component!");
            Destroy(projectileObj);
        }
    }

    /// <summary>
    /// Stops the spawn countdown (called if throw is interrupted)
    /// </summary>
    public void CancelSpawn()
    {
        isWaitingToSpawn = false;
        spawnTimer = 0f;
    }
}