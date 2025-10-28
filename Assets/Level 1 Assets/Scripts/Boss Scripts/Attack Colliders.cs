using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackColliders : MonoBehaviour
{
    public float damage;
    public PlayerBlockParryController PlayerBlockParryController;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerHitboxScript>())
        {
            collision.GetComponent<PlayerHitboxScript>().TakeDamage(20 * PlayerBlockParryController.GetDamageReductionMultiplier(), (collision.transform.position - this.transform.position).normalized);
            
            //if (collision.GetComponent<PlayerHitboxScript>().playerBlockParryController.CheckParry())
            //{
            //    collision.GetComponent<PlayerHitboxScript>().TakeDamage(0, (collision.transform.position - this.transform.position).normalized);
            //}
        }
    }
}
