using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    [Header("Hitbox Settings")]
    public Vector2 hitboxOffset = new Vector2(1f, 0f);
    public Vector2 hitboxSize = new Vector2(2f, 2f);
    public bool showHitboxDebug = true;

    private BoxCollider2D hitboxCollider;

    void Start()
    {
        hitboxCollider = GetComponent<BoxCollider2D>();
        if (hitboxCollider == null)
        {
            hitboxCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        hitboxCollider.isTrigger = true;
        hitboxCollider.offset = hitboxOffset;
        hitboxCollider.size = hitboxSize;
        hitboxCollider.enabled = false;
    }

    public void ActivateHitbox()
    {
        hitboxCollider.enabled = true;
    }

    public void DeactivateHitbox()
    {
        hitboxCollider.enabled = false;
    }

    public bool IsHitboxActive()
    {
        return hitboxCollider.enabled;
    }

    public Bounds GetHitboxBounds()
    {
        return hitboxCollider.bounds;
    }

    void OnDrawGizmos()
    {
        if (!showHitboxDebug)
            return;

        Vector3 hitboxWorldPos = transform.position + (Vector3)hitboxOffset;

        // Change color based on whether hitbox is active
        Color wireframeColor = hitboxCollider != null && hitboxCollider.enabled ? Color.red : Color.yellow;

        // Draw wireframe
        Gizmos.color = wireframeColor;
        Gizmos.DrawWireCube(hitboxWorldPos, hitboxSize);

        // Draw semi-transparent filled cube
        Gizmos.color = new Color(wireframeColor.r, wireframeColor.g, wireframeColor.b, 0.15f);
        Gizmos.DrawCube(hitboxWorldPos, hitboxSize);
    }
}