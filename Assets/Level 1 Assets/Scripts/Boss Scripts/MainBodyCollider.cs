using System.Collections;
using UnityEngine;

public class MainBodyCollider : MonoBehaviour
{
    public BossHandler bossHandler;
    public float Body_HP;
    public float current_HP;

    public Collider2D damageCollider;
    public SpriteRenderer thisSprite;
    private Color defaultColor;

    public SoundManager soundManager;
    private void Start()
    {
        damageCollider.enabled = false;
        defaultColor = thisSprite.color;
    }
    public void ToggleHitbox()
    {
        damageCollider.enabled = !damageCollider.enabled;
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
        bossHandler.IncreaseStagger(10);
        if (current_HP - damage <= 0)
        {
            current_HP = 0;
            bossHandler.BreakLimb();
            Destroy(this.gameObject);
        }

        current_HP -= damage;
        bossHandler.OnHit(damage);
    }
    public IEnumerator DamageFlash()
    {
        soundManager.SFXSource.PlayOneShot(soundManager.SoundEffects[8], 1);
        thisSprite.color = Color.darkRed;
        yield return new WaitForSecondsRealtime(0.3f);
        thisSprite.color = defaultColor;
    }
}
