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
        if (bossScript.CheckStagger())
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

public class BossStatesStalk1 : BossStates 
{
    private float stalkingDur;
    private float checkTracker;
    private Vector2 targetPos;
    public BossStatesStalk1(BossHandler bossHandler) : base(bossHandler)
    {
        stalkingDur = 0;
        checkTracker = 0;
        targetPos = bossScript.playerPos();
    }
    public override void DoUpdate(float dTime)
    {
        stalkingDur += Time.deltaTime;
        checkTracker += Time.fixedDeltaTime;
        Vector2 targetPos = bossScript.playerPos();

        if (bossScript.CheckStagger())
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

        if (stalkingDur <= 2 && checkTracker >= 0.5f)
        {
            if (Vector2.Distance(bossScript.playerPos(), bossScript.currentPos()) <= 5)
            {
                bossScript.MoveAwayFromPlayer(dTime, targetPos, bossScript.moveSpeed);
            }
            else
            {
                bossScript.MoveTowards(dTime, targetPos, bossScript.moveSpeed);
            }
        }

        if (stalkingDur >= 2)
        {
            if (bossScript.CheckPlayerWithinAttackRange(targetPos))
            {
                bossScript.SetCurrentState(new BossStatesAttack1(bossScript));
            }
            else
            {
                bossScript.SetCurrentState(new BossStatesChase1(bossScript));
            }
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
        if (bossScript.CheckStagger())
        {
            bossScript.SetCurrentState(new BossStatesStun1(bossScript));
        }

        bool canAttack = !bossScript.CheckIsAttacking();
        Debug.Log("Can attack = " + canAttack);

        if (canAttack)
        {
            //if (bossScript.bossAnims.GetCurrentAnimatorStateInfo(0).IsName("Left Swing") || bossScript.bossAnims.GetCurrentAnimatorStateInfo(0).IsName("Right Swing"))
            //{
            //    return;
            //}

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
        stunDur = 0;
        stunMax = 3;
    }
    public override void DoUpdate(float dTime)
    {
        stunDur += Time.fixedDeltaTime;

        if (stunDur >= stunMax)
        {
            bossScript.SetCurrentState(new BossStatesChase1(bossScript));
        }
    }

    public override void InRangePlayer()
    {
        
    }
}

public class BossStatesAttack2 : BossStates 
{
    public BossStatesAttack2(BossHandler bossHandler) : base(bossHandler)
    {

    }

    public override void DoUpdate(float dTime)
    {
        
    }

    public override void InRangePlayer()
    {
        
    }
}
