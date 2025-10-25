using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    [Header("Hitbox Settings")]
    public Vector2 hitboxOffset = new Vector2(1f, 0f);
    public Vector2 hitboxSize = new Vector2(2f, 2f);
    public bool showHitboxDebug = true;

    private BoxCollider2D hitboxCollider;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitboxCollider = GetComponent<BoxCollider2D>();

        if (hitboxCollider == null)
        {
            hitboxCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        hitboxCollider.isTrigger = true;
        hitboxCollider.size = hitboxSize;
        hitboxCollider.enabled = false;
    }

    /// <summary>
    /// Activates the hitbox and adjusts its offset based on player facing direction
    /// </summary>
    public void ActivateHitbox()
    {
        // Mirror the offset based on which direction player is facing
        Vector2 adjustedOffset = hitboxOffset;

        if (spriteRenderer.flipX) // Facing left
        {
            adjustedOffset.x = -hitboxOffset.x; // Flip the X offset
        }

        hitboxCollider.offset = adjustedOffset;
        hitboxCollider.enabled = true;
    }

    /// <summary>
    /// Deactivates the hitbox
    /// </summary>
    public void DeactivateHitbox()
    {
        hitboxCollider.enabled = false;
    }

    /// <summary>
    /// Returns whether the hitbox is currently active
    /// </summary>
    public bool IsHitboxActive()
    {
        return hitboxCollider.enabled;
    }

    /// <summary>
    /// Returns the world space bounds of the hitbox
    /// </summary>
    public Bounds GetHitboxBounds()
    {
        return hitboxCollider.bounds;
    }

    void OnDrawGizmos()
    {
        if (!showHitboxDebug)
            return;

        // Get sprite renderer if not cached
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Calculate adjusted offset based on flip direction
        Vector2 adjustedOffset = hitboxOffset;
        if (spriteRenderer != null && spriteRenderer.flipX)
        {
            adjustedOffset.x = -hitboxOffset.x;
        }

        // Draw hitbox gizmo at adjusted position
        Vector3 hitboxWorldPos = transform.position + (Vector3)adjustedOffset;
        Color wireframeColor = hitboxCollider != null && hitboxCollider.enabled ? Color.red : Color.yellow;

        // Draw wireframe cube
        Gizmos.color = wireframeColor;
        Gizmos.DrawWireCube(hitboxWorldPos, hitboxSize);

        // Draw semi-transparent filled cube
        Gizmos.color = new Color(wireframeColor.r, wireframeColor.g, wireframeColor.b, 0.15f);
        Gizmos.DrawCube(hitboxWorldPos, hitboxSize);
    }
}