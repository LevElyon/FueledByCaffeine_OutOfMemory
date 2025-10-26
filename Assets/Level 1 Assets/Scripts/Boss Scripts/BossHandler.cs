using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BossHandler : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxHP = 100;
    public float currentHP;
    private int limbsBroken;
    public bool isPhase1 = true;

    public float attackRange;
    public float stunDur = 1.5f;
    public float staggerMin = 0f;
    public float staggerMax = 100f;
    public float stunCD;
    public bool canStun;
    public int selectedPath;

    private float attackCD;
    private float attackInt = 1.5f;
    private float attackInt2 = 5f;

    private BossStates currentState;
    public Transform startPoint;
    public Animator bossAnims;
    public Collider2D leftSwing;
    public Collider2D rightSwing;
    public Collider2D ramColliderFront;
    public Collider2D ramColliderBack;
    public Collider2D mainBody;
    public Collider2D bodyLeftCollider;
    public Collider2D bodyRightCollider;

    public GameObject[] DashPaths;

    public PlayerMovement player;
    public Transform playerLeft;
    public Transform playerRight;

    public Transform leftBound;
    public Transform rightBound;
    public SpriteRenderer bossSprite;

    public Sprite defaultSprite;
    public Sprite dashingSprite;

    private void Start()
    {
        this.transform.position = startPoint.position;

        attackCD = attackInt;
        currentHP = maxHP;
        limbsBroken = 0;
        foreach (GameObject s in DashPaths)
        {
            s.GetComponent<SpriteRenderer>().enabled = false;
        }

        currentState = new BossStatesChase1(this);
        leftSwing.enabled = false;
        rightSwing.enabled = false;
        ramColliderFront.enabled = false;
        ramColliderBack.enabled = false;
        bodyLeftCollider.enabled = false;
        bodyRightCollider.enabled = false;

        bossSprite.flipX = false;
        SetBackToIdleAnim();
    }

    private void FixedUpdate()
    {
        currentState.DoUpdate(Time.fixedDeltaTime);

        if (isPhase1 && limbsBroken >= 2)
        {
            isPhase1 = false;
            mainBody.enabled = true;
            SetCurrentState(new BossStateMoveToStart(this));
        }

        canStun = (stunCD <= 0 ? true : false);
        staggerMin -= (Time.deltaTime / 2);
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
        stunCD -= Time.deltaTime;
        Mathf.Clamp(stunCD, 0, 15);

        if (!isPhase1)
        {
            if (bossSprite.flipX)
            {
                bodyRightCollider.enabled = true;
                bodyLeftCollider.enabled = false;
            }
            else
            {
                bodyRightCollider.enabled = false;
                bodyLeftCollider.enabled = true;
            }
        }
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

    public void ResetStunCD()
    {
        stunCD = 15;
    }

    public void ResetAttackCD(int number)
    {
        attackCD = number;
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
    public bool CheckIsAttacking2()
    {
        if (attackCD >= attackInt2)
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

        ResetAttackCD(0);
    }

    public void OnHit(float damage)
    {
        if (isPhase1)
        {
            IncreaseStagger(damage);
            currentHP -= damage;
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
        EndDashAttack();
        Destroy(this.gameObject, 1);
    }

    public void CheckFlip(bool toBool)
    {
        if (!toBool)
        {
            bossSprite.flipX = true;
        }
        else
        {
            bossSprite.flipX = false;
        }
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
        ramColliderFront.enabled = !ramColliderFront.enabled;
    }

    public void ToggleRamAttackBack()
    {
        ramColliderBack.enabled = !ramColliderBack.enabled;
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

        ResetAttackCD(0);
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

    public int CheckWhichLinePlayerIsOn()
    {
        
        float Dist1 = Vector2.Distance(playerPos(), DashPaths[0].GetComponent<DashPositions>().GetMidPos());
        float Dist2 = Vector2.Distance(playerPos(), DashPaths[1].GetComponent<DashPositions>().GetMidPos());
        float Dist3 = Vector2.Distance(playerPos(), DashPaths[2].GetComponent<DashPositions>().GetMidPos());

        if (Dist1 <= Dist2 && Dist1 <= Dist3)
        {
            return 0;
        }

        if (Dist2 <= Dist1 && Dist2 <= Dist3)
        {
            return 1;
        }

        if (Dist3 <= Dist1 && Dist3 <= Dist2)
        {
            return 2;
        }

        return 1;
    }

    public void EndDashAttack()
    {
        foreach (GameObject g in DashPaths)
        {
            g.GetComponent<SpriteRenderer>().enabled = false;
            g.GetComponent<SpriteRenderer>().sprite = defaultSprite;
        }
    }

    public IEnumerator DelayBySeconds(float time)
    {
        yield return new WaitForSecondsRealtime(time);
    }
}
