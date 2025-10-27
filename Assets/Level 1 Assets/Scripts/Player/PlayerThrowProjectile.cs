using UnityEngine;

/// <summary>
/// Represents a single thrown projectile. Travels in a direction until it hits something or reaches max distance.
/// Boss can independently detect collision and take damage.
/// </summary>
public class PlayerThrowProjectile : MonoBehaviour
{
    [Header("Movement Settings")]
    public float projectileSpeed = 10f;          // How fast the projectile travels
    public float maxDistance = 15f;              // Maximum distance before disappearing

    [Header("Collider Settings")]
    public Vector2 colliderSize = new Vector2(1f, 1f);
    public Vector2 colliderOffset = Vector2.zero;

    private Vector2 travelDirection = Vector2.right;
    private float distanceTraveled = 0f;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    void Start()
    {
        // Get or create Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // Configure Rigidbody
        rb.gravityScale = 0f;                    // No gravity for projectile
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Don't rotate

        // Get or create BoxCollider2D
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        // Configure collider
        boxCollider.isTrigger = true;            // Trigger so it doesn't physically collide
        boxCollider.size = colliderSize;
        boxCollider.offset = colliderOffset;
    }

    void FixedUpdate()
    {
        // Move the projectile
        Vector2 movement = travelDirection * projectileSpeed * Time.fixedDeltaTime;
        rb.linearVelocity = movement;

        // Track distance traveled
        distanceTraveled += movement.magnitude;

        // Destroy if max distance reached
        if (distanceTraveled >= maxDistance)
        {
            DestroyProjectile();
        }
    }

    /// <summary>
    /// Initialize the projectile with a direction
    /// </summary>
    public void Initialize(Vector2 direction)
    {
        travelDirection = direction.magnitude > 0 ? direction.normalized : Vector2.right;
    }

    /// <summary>
    /// Called when projectile hits something via trigger collision
    /// </summary>
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if hit boundary or boss - projectile disappears on collision
        if (collision.CompareTag("Boss") || collision.CompareTag("Boundary") || collision.CompareTag("Wall"))
        {
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Get the projectile bounds for external collision detection (for boss)
    /// </summary>
    public Bounds GetProjectileBounds()
    {
        return boxCollider.bounds;
    }
}