using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class EnemyStartedAttack : ActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
        blackboard.enemyAttackStarted = false;
    }

    protected override State OnUpdate()
    {
        if(blackboard.enemyAttackStarted) return State.Success;
        else return State.Failure;
    }

}
