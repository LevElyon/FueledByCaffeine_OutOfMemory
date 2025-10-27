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

    private Vector2 travelDirection = Vector2.right;
    private float distanceTraveled = 0f;
    private Rigidbody2D rb;
    private Collider2D projectileCollider;

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

        // Find the CapsuleCollider - try direct child lookup by name first
        projectileCollider = null;

        // Method 1: Find "Collider" child object and get its CapsuleCollider2D
        Transform colliderChild = transform.Find("Collider");
        if (colliderChild != null)
        {
            projectileCollider = colliderChild.GetComponent<CapsuleCollider2D>();
        }

        // Method 2: If not found, try searching for ANY CapsuleCollider2D in children
        if (projectileCollider == null)
        {
            projectileCollider = GetComponentInChildren<CapsuleCollider2D>();
        }

        // Method 3: Fallback - get BoxCollider on this object
        if (projectileCollider == null)
        {
            projectileCollider = GetComponent<BoxCollider2D>();
        }

        // Method 4: Create new BoxCollider if nothing found
        if (projectileCollider == null)
        {
            projectileCollider = gameObject.AddComponent<BoxCollider2D>();
            Debug.LogWarning("Projectile: Created new BoxCollider2D - consider adding CapsuleCollider2D to prefab!");
        }

        // Configure collider as trigger so it doesn't physically collide
        projectileCollider.isTrigger = true;

        // Ignore ALL player colliders so projectile passes through
        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement != null)
        {
            Collider2D[] playerColliders = playerMovement.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D playerCollider in playerColliders)
            {
                Physics2D.IgnoreCollision(projectileCollider, playerCollider);
            }
        }
        else
        {
            Debug.LogWarning("Projectile: Could not find PlayerMovement component!");
        }
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
            Debug.Log($"Projectile reached max distance: {distanceTraveled:F2} / {maxDistance}. Destroying...");
            DestroyProjectile();
        }
    }

    /// <summary>
    /// Initialize the projectile with a direction
    /// </summary>
    public void Initialize(Vector2 direction)
    {
        travelDirection = direction.magnitude > 0 ? direction.normalized : Vector2.right;

        // Calculate angle from throw direction
        // The prefab base rotation of Z=-121 represents the "right" direction
        float angle = Mathf.Atan2(travelDirection.y, travelDirection.x) * Mathf.Rad2Deg;
        float finalRotation = angle - 121f;

        // Rotate the ROOT object (child will automatically inherit this rotation through hierarchy)
        transform.rotation = Quaternion.AngleAxis(finalRotation, Vector3.forward);

        Debug.Log($"Projectile initialized: direction={travelDirection}, angle={angle}°, final rotation={finalRotation}°");
    }

    /// <summary>
    /// Called when projectile hits something via trigger collision
    /// </summary>
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if hit boss or wall - projectile disappears on collision
        if (collision.CompareTag("Boss"))
        {
            Debug.Log($"Projectile hit Boss! Destroying...");
            DestroyProjectile();
        }
        else if (collision.CompareTag("Wall"))
        {
            Debug.Log($"Projectile hit Wall! Destroying...");
            DestroyProjectile();
        }
        else
        {
            Debug.Log($"Projectile hit something: {collision.gameObject.name}, Tag: {collision.tag}");
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
        return projectileCollider.bounds;
    }

    void OnDrawGizmos()
    {
        // Draw collider bounds when selected or in editor
        if (projectileCollider == null)
            projectileCollider = GetComponentInChildren<CapsuleCollider2D>();
        if (projectileCollider == null)
            projectileCollider = GetComponent<BoxCollider2D>();

        if (projectileCollider == null)
            return;

        Gizmos.color = new Color(0, 1, 0, 0.3f); // Semi-transparent green

        // Draw based on collider type
        if (projectileCollider is CapsuleCollider2D capsule)
        {
            // Draw capsule collider
            Vector3 center = transform.position + (Vector3)capsule.offset;
            Vector3 size = capsule.size;

            // Draw as wireframe circles for capsule visualization
            DrawCapsuleGizmo(center, size.x, size.y);
        }
        else if (projectileCollider is BoxCollider2D box)
        {
            // Draw box collider
            Vector3 center = transform.position + (Vector3)box.offset;
            Gizmos.DrawCube(center, new Vector3(box.size.x, box.size.y, 0.1f));
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw more visible collider when projectile is selected
        if (projectileCollider == null)
            projectileCollider = GetComponentInChildren<CapsuleCollider2D>();
        if (projectileCollider == null)
            projectileCollider = GetComponent<BoxCollider2D>();

        if (projectileCollider == null)
            return;

        Gizmos.color = new Color(1, 1, 0, 1f); // Bright yellow when selected

        // Draw based on collider type
        if (projectileCollider is CapsuleCollider2D capsule)
        {
            // Draw capsule collider
            Vector3 center = transform.position + (Vector3)capsule.offset;
            Vector3 size = capsule.size;

            // Draw as solid lines for capsule visualization
            DrawCapsuleGizmo(center, size.x, size.y);
        }
        else if (projectileCollider is BoxCollider2D box)
        {
            // Draw box collider
            Vector3 center = transform.position + (Vector3)box.offset;
            Gizmos.DrawWireCube(center, new Vector3(box.size.x, box.size.y, 0.1f));
        }
    }

    /// <summary>
    /// Draw a capsule shape for gizmos
    /// </summary>
    private void DrawCapsuleGizmo(Vector3 center, float width, float height)
    {
        // Draw capsule as two circles connected by lines
        float radius = width / 2f;
        float halfHeight = (height - width) / 2f;

        // Top circle
        Vector3 topCenter = center + Vector3.up * halfHeight;
        DrawCircleGizmo(topCenter, radius);

        // Bottom circle
        Vector3 bottomCenter = center - Vector3.up * halfHeight;
        DrawCircleGizmo(bottomCenter, radius);

        // Side lines
        Gizmos.DrawLine(topCenter + Vector3.left * radius, bottomCenter + Vector3.left * radius);
        Gizmos.DrawLine(topCenter + Vector3.right * radius, bottomCenter + Vector3.right * radius);
    }

    /// <summary>
    /// Draw a circle for gizmos (for capsule ends)
    /// </summary>
    private void DrawCircleGizmo(Vector3 center, float radius)
    {
        int segments = 16;
        Vector3 previousPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Gizmos.DrawLine(previousPoint, newPoint);
            previousPoint = newPoint;
        }
    }
}