using System.Collections;
using TMPro;
using UnityEngine;

public class BossHandler : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxHP = 500;
    private float currentHP;
    private bool isPhase1 = true;

    public float attackRange;
    public float stunDur = 1.5f;
    public float staggerMin = 0f;
    public float staggerMax = 100f;

    private float attackCD;
    private float attackInt = 1f;

    private BossStates currentState;
    public Transform startPoint;
    public Animator bossAnims;
    public Collider2D leftSwing;
    public Collider2D rightSwing;

    public GameObject[] DashPaths;

    public PlayerMovement player;

    private void Start()
    {
        this.transform.position = startPoint.position;

        attackCD = attackInt;
        currentHP = maxHP;

        currentState = new BossStatesChase1(this);
        leftSwing.enabled = false;
        rightSwing.enabled = false;

        DoSlamAttack();
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
        if (bossAnims.GetCurrentAnimatorStateInfo(0).IsName("LSwing"))
        {
            bossAnims.Play("Idle");
        }
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

    public void MoveAwayFromPlayer(float dTime, Vector2 targetPos, float moveSpeed)
    {
        Vector2 direction = targetPos;
        direction.x += this.transform.position.x;
        direction.y += this.transform.position.y;
        Vector2 moveVector = direction.normalized * dTime * moveSpeed;
        Vector2 finalPos = moveVector;
        finalPos.x -= this.transform.position.x;
        finalPos.y -= this.transform.position.y;
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

    public Vector2 currentPos()
    {
        return this.transform.position;
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

    public void ResetAttackCD()
    {
        attackCD = 0;
    }

    public bool CheckPlayerWithinAttackRange()
    {
        float distance = Vector2.Distance(playerPos(), currentPos());
        //check player is within attack range
        if (distance <= attackRange)
        {
            Debug.Log(distance);
            Debug.Log("In range = true");
            return true;
        }

        Debug.Log(distance);
        Debug.Log("In range = false");
        return false;
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
                bossAnims.Play("LSwing");
                break;
            case (2):
                //bossAnims.Play("");
                break;

            default: return;
        }

        ResetAttackCD();
    }

    public void DoAttackPhase2(int number)
    {
        Debug.Log("Phase 2: Attack");
        Vector2 startPos = DashPaths[number].GetComponent<DashPositions>().GetStartPos();
        Vector2 endPos = DashPaths[number].GetComponent<DashPositions>().GetEndPos();
        StartCoroutine(DashAttack(startPos, endPos));
    }

    public void DoSlamAttack()
    {
        Vector2 targetPos = playerPos();
        Debug.Log("Starting attack");
        StartCoroutine(DelayBySeconds(5));
        Debug.Log("End attack");
    }

    public void OnHit(float damage)
    {
        Debug.Log("Got hit by player for " + damage);


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

    public void ToggleLCollider()
    {
        leftSwing.enabled = !leftSwing.enabled;
    }

    public void ToggleRCollider()
    {
        rightSwing.enabled = !rightSwing.enabled;
    }

    public IEnumerator LeftAttackPhase1()
    {
        yield return null;
    }


    public IEnumerator DashAttack(Vector2 start, Vector2 end)
    {
        MoveTowards(Time.deltaTime, start, moveSpeed * 2);
        yield return new WaitForSeconds(2);
        MoveTowards(Time.deltaTime, end, moveSpeed * 4);
    }
    public IEnumerator OccasionallyCheckInRangePlayer()
    {
        yield return new WaitForSeconds(0.5f);

        yield return CheckPlayerWithinAttackRange();
    }

    public IEnumerator DelayBySeconds(float time)
    {
        yield return new WaitForSecondsRealtime(time);
    }
}
