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
    public BossStatesChase1(BossHandler bossHandler) : base(bossHandler)
    {
        bossHandler.bossAnims.SetBool("DoAttack", false);
    }
    public override void DoUpdate(float dTime)
    {
        Vector2 targetPos = bossScript.playerPos();

        if (bossScript.CheckStagger())
        {
            bossScript.SetCurrentState(new BossStatesStun1(bossScript));
        }

        if (bossScript.CheckPlayerWithinAttackRange())
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
    public BossStatesStalk1(BossHandler bossHandler) : base(bossHandler)
    {
        stalkingDur = 0;
        checkTracker = 0;
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
            if (bossScript.CheckPlayerWithinAttackRange())
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
    public BossStatesAttack1(BossHandler bossHandler) : base(bossHandler)
    {
        bossHandler.bossAnims.SetBool("DoAttack", true);
    }
    public override void DoUpdate(float dTime)
    {
        if (bossScript.CheckStagger())
        {
            bossScript.SetCurrentState(new BossStatesStun1(bossScript));
        }

        bool canAttack = bossScript.CheckIsAttacking();

        if (canAttack)
        {
            //Add check for whether currently performing an attack animation
            if (bossScript.CheckPlayerWithinAttackRange())
            {
                bossScript.DoAttackPhase1(1);
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
