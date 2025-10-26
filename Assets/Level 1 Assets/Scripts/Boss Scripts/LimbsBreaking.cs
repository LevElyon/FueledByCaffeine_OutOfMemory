using UnityEngine;

public class LimbsBreaking : MonoBehaviour
{
    public BossHandler bossHandler;
    public GameObject parentObj;
    public float Limb_Max_HP;
    public float Limb_Current_HP;

    private void Start()
    {
        Limb_Current_HP = Limb_Max_HP;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            Debug.Log(parentObj.name);
            //Add method to get the player's attack damage
            this.TakeDamage(20);
        }
    }

    public void TakeDamage(float damage)
    {
        if (Limb_Current_HP - damage <= 0)
        {
            Limb_Current_HP = 0;
            Destroy(parentObj);
        }

        Limb_Current_HP -= damage;
        bossHandler.OnHit(damage);
    }
}
