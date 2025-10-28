using System.Collections;
using UnityEngine;

public class LimbsBreaking : MonoBehaviour
{
    public BossHandler bossHandler;
    public GameObject parentObj;
    public SoundManager SoundManager;
    public float Limb_Max_HP;
    public float Limb_Current_HP;
    public SpriteRenderer[] LimbSprites;
    private Color current;

    private void Start()
    {
        Limb_Current_HP = Limb_Max_HP;
        current = LimbSprites[0].color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            //Add method to get the player's attack damage
            this.TakeDamage(20);
            StartCoroutine(DamageFlash());
        }
    }

    public void TakeDamage(float damage)
    {
        if (Limb_Current_HP - damage <= 0)
        {
            Limb_Current_HP = 0;
            bossHandler.BreakLimb();
            Destroy(parentObj);
        }

        Limb_Current_HP -= damage;
        bossHandler.OnHit(damage);
    }

    public IEnumerator DamageFlash()
    {
        SoundManager.SFXSource.PlayOneShot(SoundManager.SoundEffects[7], 1);
        foreach (SpriteRenderer s in LimbSprites)
        {
            s.color = Color.darkRed;
        }
        yield return new WaitForSecondsRealtime(0.3f);
        foreach (SpriteRenderer s in LimbSprites)
        {
            s.color = current;
        }
    }
}
