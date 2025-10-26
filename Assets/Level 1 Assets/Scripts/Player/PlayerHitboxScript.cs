using UnityEngine;

public class PlayerHitboxScript : MonoBehaviour
{
    public PlayerHealthController playerHealthController;
    public PlayerBlockParryController playerBlockParryController;
    public BossHandler BossHandler;

    public void TakeDamage(float damage, Vector2 Dir)
    {
        playerBlockParryController.CheckParry();
        playerHealthController.TakeDamage(damage, Dir * 2f);
    }
}
