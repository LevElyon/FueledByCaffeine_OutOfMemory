using UnityEngine;

public class BossHandler : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxHP = 500;
    private float currentHP;

    public float attackRange = 2.5f;
    public float stunDur = 1.5f;
    public float staggerMin = 0f;
    public float staggerMax = 100f;

    private float attackCD;
    private float attackInt = 1f;

    private BossStates currentState;
    public Transform startPoint;
    public Animator bossAnims;

    private PlayerMovement player;

    private void FixedUpdate()
    {
        currentState.DoUpdate(Time.fixedDeltaTime);
    }

    public void SetCurrentState(BossStates state)
    {
        currentState = state;
    }

    public string GetCurrentState()
    {
        return currentState.ToString();
    }

    private void Update()
    {
        attackCD += Time.deltaTime;
    }

    public void Initialize(PlayerMovement aplayer)
    {
        player = aplayer;

        this.transform.position = startPoint.position;

        attackCD = attackInt;
        currentHP = maxHP;

        currentState = new BossStatesChase1(this);
    }
    public void MoveTowards(float dTime, Vector2 targetPos, float moveSpeed)
    {

        //move towards target position
        Vector2 direction = targetPos;
        direction.x -= this.transform.position.x;
        direction.y -= this.transform.position.y;
        Vector2 moveVector = direction.normalized * dTime * moveSpeed;
        Vector2 finalPos = moveVector;
        finalPos.x += this.transform.position.x;
        finalPos.y += this.transform.position.y;
        this.GetComponent<Rigidbody2D>().MovePosition(finalPos);

        if (Vector2.Distance(targetPos, finalPos) < 0.2f)
        {
            currentState?.InRangePlayer();
        }
    }

    public Vector2 playerPos()
    {
        return player.transform.position;
    }

    public void ResetStagger()
    {
        staggerMin = 0;
    }

    public void IncreaseStagger(float amt)
    {
        if (currentState.ToString() == "BossStatesStun1")
        {
            return;
        }

        if (staggerMin + amt >= staggerMax)
        {
            staggerMin = staggerMax;
            return;
        }

        staggerMin += amt;
    }

    public bool CheckStagger()
    {
        return (staggerMin >= staggerMax ? true : false);
    }

    public bool CheckPlayerWithinAttackRange()
    {
        //check player is within attack range
        bool isInRange = Vector2.Distance(player.transform.position, this.transform.position) < attackRange;

        return isInRange;
    }

    public bool CheckIsAttacking()
    {
        return (attackCD >= attackInt ? true : false);
    }

    public void DoAttack(float type)
    {
        switch (type)
        {
            case (1):
                bossAnims.Play("");
                break;
            case (2):
                bossAnims.Play("");
                break;

            default: return;
        }

    }

    public void OnHit(float damage)
    {
        if (currentHP - damage <= 0)
        {
            //Boss dies
            return;
        }

        currentHP -= damage;
        currentState = new BossStatesStalk1(this);
    }
}
