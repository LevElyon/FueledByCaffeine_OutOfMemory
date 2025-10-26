using UnityEngine;

public class BossDamageColliders : MonoBehaviour
{
    private Collider2D thisCollider;

    private void Awake()
    {
        thisCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 Dir = this.transform.position - collision.transform.position;
            collision.GetComponent<PlayerHealthController>().TakeDamage(20, Dir.normalized);
            Debug.Log("Hit player for 20 damage");
        }
    }
}
