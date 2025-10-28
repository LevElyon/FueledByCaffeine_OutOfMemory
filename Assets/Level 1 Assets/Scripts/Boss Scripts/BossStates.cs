using System.Collections;
using UnityEngine;

public abstract class BossStates 
{
    protected BossHandler bossScript;

    public BossStates(BossHandler bossScript)
    {
        this.bossScript = bossScript;
    }

    public abstract void DoUpdate(float dTime);

    public abstract void InRangePlayer();

}

public class BossStatesChase1 : BossStates 
{
    private Vector2 targetPos;
    public BossStatesChase1(BossHandler bossHandler) : base(bossHandler)
    {
        targetPos = bossHandler.playerPos();
        bossHandler.SetBackToIdleAnim();
        Debug.Log("Change to chase state");
    }
    public override void DoUpdate(float dTime)
    {
        if (bossScript.canStun && bossScript.CheckStagger())
        {
            bossScript.SetCurrentState(new BossStatesStun1(bossScript));
        }

        if (bossScript.CheckIsPlayerLeftSide())
        {
            targetPos = bossScript.playerRight.position;
        }
        else
        {
            targetPos = bossScript.playerLeft.position;
        }

        if (bossScript.CheckPlayerWithinAttackRange(targetPos))
        {
            bossScript.SetCurrentState(new BossStatesAttack1(bossScript));
        }
        else
        {
            bossScript.MoveTowards(dTime, targetPos, bossScript.moveSpeed);
        }
    }

    public override void InRangePlayer()
    {
        
    }
}

public class BossStatesIdle : BossStates 
{
    private float idleDur;
    private float maxIdle;
    public BossStatesIdle(BossHandler bossHandler) : base(bossHandler)
    {
        idleDur = 0;
        maxIdle = 2;
    }
    public override void DoUpdate(float dTime)
    {
        if (bossScript.canStun && bossScript.CheckStagger())
        {
            bossScript.SetCurrentState(new BossStatesStun1(bossScript));
        }

        idleDur += Time.deltaTime;

        if (idleDur >= maxIdle)
        {
            bossScript.SetCurrentState(new BossStateMoveToStart(bossScript));
        }
    }

    public override void InRangePlayer()
    {
        
    }
}
    
public class BossStatesAttack1 : BossStates 
{
    private Vector2 targetPos;
    public BossStatesAttack1(BossHandler bossHandler) : base(bossHandler)
    {
        bossHandler.SetBackToIdleAnim();
    }
    public override void DoUpdate(float dTime)
    {
        if (bossScript.canStun && bossScript.CheckStagger())
        {
            bossScript.SetCurrentState(new BossStatesStun1(bossScript));
        }

        bool canAttack = !bossScript.CheckIsAttacking();
        //Debug.Log("Can attack = " + canAttack);

        if (canAttack)
        {
            bossScript.SetBackToIdleAnim();

            if (bossScript.CheckIsPlayerLeftSide())
            {
                targetPos = bossScript.playerRight.position;
                Debug.Log("Player is on left side");
                if (bossScript.CanDoSlashAttack(1))
                {
                    if (bossScript.CheckPlayerWithinAttackRange(targetPos))
                    {
                        bossScript.DoAttackPhase1(1);
                    }
                    else
                    {
                        bossScript.SetCurrentState(new BossStatesChase1(bossScript));
                    }
                }
                else
                {
                    if (bossScript.CheckPlayerWithinAttackRange(targetPos))
                    {
                        bossScript.RamAttack(1);
                    }
                    else
                    {
                        bossScript.SetCurrentState(new BossStatesChase1(bossScript));
                    }
                }
            }
            else if (bossScript.CheckIsPlayerLeftSide() == false)
            {
                targetPos = bossScript.playerLeft.position;
                Debug.Log("Player is on right side");
                if (bossScript.CanDoSlashAttack(2))
                {
                    if (bossScript.CheckPlayerWithinAttackRange(targetPos))
                    {
                        bossScript.DoAttackPhase1(2);
                    }
                    else
                    {
                        bossScript.SetCurrentState(new BossStatesChase1(bossScript));
                    }
                }
                else
                {
                    if (bossScript.CheckPlayerWithinAttackRange(targetPos))
                    {
                        bossScript.RamAttack(2);
                    }
                    else
                    {
                        bossScript.SetCurrentState(new BossStatesChase1(bossScript));
                    }
                }
            }
        }
    }

    public override void InRangePlayer()
    {
        
    }
}

public class BossStatesStun1 : BossStates 
{
    private float stunDur;
    private float stunMax;
    public BossStatesStun1(BossHandler bossHandler) : base(bossHandler)
    {
        //Add stun animation
        stunDur = 0;
        stunMax = 3;
    }
    public override void DoUpdate(float dTime)
    {
        stunDur += Time.fixedDeltaTime;

        if (stunDur >= stunMax)
        {
            bossScript.ResetStagger();
            if (bossScript.isPhase1)
            {
                bossScript.SetCurrentState(new BossStatesChase1(bossScript));
            }
            else
            {
                bossScript.SetCurrentState(new BossStateMoveToStart(bossScript));
            }
        }
    }

    public override void InRangePlayer()
    {

    }
}

public class BossStateMoveToStart : BossStates 
{
    private int targetPos;
    private Vector2 startPos;
    public BossStateMoveToStart(BossHandler bossHandler) : base(bossHandler)
    {
        bossScript.bossAnims.enabled = false;
        targetPos = bossScript.CheckWhichLinePlayerIsOn();
        bossScript.selectedPath = targetPos;
        bossScript.DashPaths[targetPos].GetComponent<SpriteRenderer>().enabled = true;
        startPos = bossScript.DashPaths[targetPos].GetComponent<DashPositions>().GetStartPos();
    }

    public override void DoUpdate(float dTime)
    {
        if (Vector2.Distance(bossScript.transform.position, startPos) <= 0.5f)
        {
            bossScript.SetCurrentState(new BossStateMoveToEnd(bossScript));
        }
        else
        {
            bossScript.MoveTowards(dTime, startPos, bossScript.moveSpeed * 4);
        }
    }
    public override void InRangePlayer()
    {

    }
}

public class BossStateMoveToEnd : BossStates
{
    private int targetPos;
    private Vector2 endPos;
    public BossStateMoveToEnd(BossHandler bossHandler) : base(bossHandler)
    {
        targetPos = bossScript.selectedPath;
        bossScript.bossAnims.enabled = false;
        bossScript.DashPaths[targetPos].GetComponent<SpriteRenderer>().sprite = bossScript.dashingSprite;
        if (!bossScript.CheckIsPlayerLeftSide())
        {
            bossScript.bossSprite.flipX = true;
        }
        else
        {
            bossScript.bossSprite.flipX = false;
        }

        endPos = bossScript.DashPaths[targetPos].GetComponent<DashPositions>().GetEndPos();
    }

    public override void DoUpdate(float dTime)
    {
        if (bossScript.canStun && bossScript.CheckStagger())
        {
            bossScript.SetCurrentState(new BossStatesStun1(bossScript));
        }

        if (Vector2.Distance(bossScript.transform.position, endPos) <= 0.5f)
        {
            bossScript.EndDashAttack();
            bossScript.SetCurrentState(new BossStatesIdle(bossScript));
        }
        else
        {
            bossScript.MoveTowards(dTime, endPos, bossScript.moveSpeed * 6);
        }
    }
    public override void InRangePlayer()
    {

    }
}
