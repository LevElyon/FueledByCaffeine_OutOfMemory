using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackColliders : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected");
        Debug.Log(collision.name);
        if (collision.GetComponent<PlayerHitboxScript>())
        {
            Debug.Log("Hit player");
            collision.GetComponent<PlayerHitboxScript>().TakeDamage(20, (collision.transform.position - this.transform.position).normalized);
        }
    }
}
