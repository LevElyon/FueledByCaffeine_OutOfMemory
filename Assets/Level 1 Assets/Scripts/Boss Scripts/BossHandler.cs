using System.Collections;
using TMPro;
using UnityEngine;

public class BossHandler : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxHP = 500;
    private float currentHP;
    private int limbsBroken;
    private bool isPhase1 = true;

    public float attackRange;
    public float stunDur = 1.5f;
    public float staggerMin = 0f;
    public float staggerMax = 100f;

    private float attackCD;
    private float attackInt = 1.5f;

    private BossStates currentState;
    public Transform startPoint;
    public Animator bossAnims;
    public Collider2D leftSwing;
    public Collider2D rightSwing;
    public Collider2D ramCollider;

    public GameObject[] DashPaths;

    public PlayerMovement player;
    public Transform playerLeft;
    public Transform playerRight;

    public Transform leftBound;
    public Transform rightBound;
    public SpriteRenderer bossSprite;

    private void Start()
    {
        this.transform.position = startPoint.position;

        attackCD = attackInt;
        currentHP = maxHP;
        limbsBroken = 0;

        currentState = new BossStatesChase1(this);
        leftSwing.enabled = false;
        rightSwing.enabled = false;
        ramCollider.enabled = false;

        bossSprite.flipX = false;
        SetBackToIdleAnim();
    }

    private void FixedUpdate()
    {
        currentState.DoUpdate(Time.fixedDeltaTime);
        if (isPhase1 && limbsBroken >= 2)
        {
            isPhase1 = false;
        }

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
    public void MoveTowards(float dTime, Vector2 targetPos, float moveSpeed)
    {
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

    public void BreakLimb()
    {
        limbsBroken += 1;
    }

    public bool CheckPlayerWithinAttackRange(Vector2 currentTargetPos)
    {
        float distance = Vector2.Distance(currentTargetPos, currentPos());
        //check player is within attack range
        if (distance <= attackRange)
        {
            return true;
        }

        return false;
    }

    public bool CheckIsAttacking()
    {
        if (attackCD >= attackInt)
        {
            return false;
        }
        else return true;
    }

    public bool CheckIsPlayerLeftSide()
    {
        return ((player.transform.position.x <= this.transform.position.x) ? true : false);
    }

    public float SpaceToLeft()
    {
        return (playerLeft.position.x - leftBound.position.x);
    }
    public float SpaceToRight()
    {
        return (playerRight.position.x - rightBound.position.x);
    }

    public void DoAttackPhase1(float type)
    {
        Debug.Log("Phase 1: Attack");
        if (CheckIsAttacking() == true)
        {
            return;
        }

        switch (type)
        {
            case (1):
                bossAnims.SetBool("DoAttackLeft", true);
                //bossAnims.Play("Left Swing");
                break;
            case (2):
                bossAnims.SetBool("DoAttackRight", true);
                //bossAnims.Play("Right Swing");
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

    public void OnHit(float damage)
    {
        Debug.Log("Got hit by player for " + damage);


        if (isPhase1)
        {
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

    public void ToggleRamAttack()
    {
        ramCollider.enabled = !ramCollider.enabled;
    }

    public void FlipBoss()
    {
        if (CheckIsPlayerLeftSide())
        {
            bossSprite.flipX = false;
        }
        else
        {
            bossSprite.flipX = true;
        }
    }

    public bool CanDoSlashAttack(int n)
    {
        switch (n) {
            case (1):
                return CheckLeftLimb();
            case (2):
                return CheckRightLimb();
            default: return false;
        }

    }

    public void RamAttack(float type)
    {
        Debug.Log("Phase 2: Attack");
        if (CheckIsAttacking() == true)
        {
            return;
        }

        switch (type)
        {
            case (1):
                bossAnims.SetBool("DoFrontRam", true);
                break;

            case (2):
                bossAnims.SetBool("DoBackRam", true);
                break;

            default: break;
        }

        ResetAttackCD();
    }

    public bool CheckLeftLimb()
    {
        return (leftSwing != null ? true : false);
    }

    public bool CheckRightLimb()
    {
        return (rightSwing != null ? true : false);
    }

    public void SetBackToIdleAnim()
    {
        bossAnims.SetBool("DoAttackLeft", false);
        bossAnims.SetBool("DoAttackRight", false);
        bossAnims.SetBool("DoFrontRam", false);
    }

    public IEnumerator DashAttack(Vector2 start, Vector2 end)
    {
        MoveTowards(Time.deltaTime, start, moveSpeed * 2);
        yield return new WaitForSeconds(2);
        MoveTowards(Time.deltaTime, end, moveSpeed * 4);
    }

    public IEnumerator DelayBySeconds(float time)
    {
        yield return new WaitForSecondsRealtime(time);
    }
}
