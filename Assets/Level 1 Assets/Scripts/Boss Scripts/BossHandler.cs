using System.Collections;
using UnityEngine;

public class BossHandler : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxHP = 500;
    private float currentHP;
    private bool isPhase1 = true;

    public float attackRange = 16f;
    public float stunDur = 1.5f;
    public float staggerMin = 0f;
    public float staggerMax = 100f;

    private float attackCD;
    private float attackInt = 1f;

    private BossStates currentState;
    public Transform startPoint;
    //public Animator bossAnims;

    public GameObject Line1;
    public GameObject Line2;
    public GameObject Line3;

    public PlayerMovement player;

    private void Start()
    {
        this.transform.position = startPoint.position;

        attackCD = attackInt;
        currentHP = maxHP;

        currentState = new BossStatesChase1(this);
    }

    private void FixedUpdate()
    {
        currentState.DoUpdate(Time.fixedDeltaTime);
        Debug.Log(GetCurrentState());
    }

    public void SetCurrentState(BossStates state)
    {
        currentState = state;
    }

    public BossStates GetCurrentState()
    {
        return currentState;
    }

    private void Update()
    {
        attackCD += Time.deltaTime;
    }

    //public void Initialize(PlayerMovement aplayer)
    //{
    //    this.transform.position = startPoint.position;

    //    attackCD = attackInt;
    //    currentHP = maxHP;

    //    currentState = new BossStatesChase1(this);
    //}
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
        bool isInRange = (Vector2.Distance(player.gameObject.transform.position, this.transform.position) <= attackRange ? true : false);
        Debug.Log(Vector2.Distance(player.gameObject.transform.position, this.transform.position));
        Debug.Log("In range = " + isInRange);

        return isInRange;
    }

    public bool CheckIsAttacking()
    {
        return (attackCD >= attackInt ? true : false);
    }

    public bool CheckIsPlayerLeftSide()
    {
        return ((player.transform.position.x < this.transform.position.x) ? true : false);
    }

    public void DoAttackPhase1(float type)
    {
        Debug.Log("Phase 1: Attack");
        switch (type)
        {
            case (1):

                //bossAnims.Play("");
                break;
            case (2):
                //bossAnims.Play("");
                break;

            default: return;
        }

    }

    public void DoAttackPhase2(float type)
    {
        Vector2 startPos;
        Debug.Log("Phase 2: Attack");
        switch (type)
        {
            case (1):
                //bossAnims.Play("");
                break;
            case (2):
                //bossAnims.Play("");
                break;

            default: return;
        }

    }

    public void OnHit(float damage)
    {
        Debug.Log("Got hit by player for " + damage);
        SoundManager.PlaySound(SoundType.BossHurt, 1);

        if (isPhase1)
        {
            if (currentHP - damage <= (maxHP / 2))
            {
                isPhase1 = false;
                //Change current state to phase 2 attacks
                currentHP -= damage;
                return;
            }

            currentHP -= damage;
            //currentState = new BossStatesStalk1(this);
            return;
        }

        if (!isPhase1)
        {
            if (currentHP - damage <= 0)
            {
                //Play dying sound
                BossDies();
                return;
            }

            currentHP -= damage;
            //Change current state to phase 2 attacks
            return;
        }
        
    }

    public void BossDies()
    {
        Destroy(this.gameObject, 2);
    }

    public IEnumerator LeftAttackPhase1()
    {
        yield return null;
    }
}
