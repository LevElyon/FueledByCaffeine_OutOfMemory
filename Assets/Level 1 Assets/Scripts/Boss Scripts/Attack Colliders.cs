using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackColliders : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerHitboxScript>())
        {
            if (collision.GetComponent<PlayerHitboxScript>().playerBlockParryController.CheckParry())
            {
                collision.GetComponent<PlayerHitboxScript>().TakeDamage(0, (collision.transform.position - this.transform.position).normalized);
            }
            else if (collision.GetComponent<PlayerHitboxScript>().playerBlockParryController.GetIsBlocking())
            {
                collision.GetComponent<PlayerHitboxScript>().TakeDamage(10, (collision.transform.position - this.transform.position).normalized);
            }
            else
            {
                collision.GetComponent<PlayerHitboxScript>().TakeDamage(20, (collision.transform.position - this.transform.position).normalized);
            }
        }
    }
}
