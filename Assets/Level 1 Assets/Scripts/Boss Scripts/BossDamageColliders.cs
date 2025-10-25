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
            //Player takes damage
        }
    }
}
