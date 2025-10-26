using UnityEngine;

public class PlayerHitboxScript : MonoBehaviour
{
    public PlayerHealthController playerHealthController;
    public PlayerBlockParryController playerBlockParryController;

    public void TakeDamage(float damage, Vector2 Dir)
    {
        playerHealthController.TakeDamage(damage, Dir * 2f);
        playerBlockParryController.CheckParry();
    }
}
