using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class DodgeOpportunityAttack : ActionNode
{
    protected override void OnStart(){}

    protected override void OnStop(){}

    protected override State OnUpdate()
    {
        int opportunityCheck = Random.Range(0, 100);

        int quickestAttackFrame = 1000;
        string quickestAttack = "";

        foreach (KeyValuePair<string, ActionAttack> attack in context.selfFSM.AttackMoveDict)
        {
            if(attack.Value.StartFrames < quickestAttackFrame) {
                quickestAttackFrame = attack.Value.StartFrames;
                quickestAttack = attack.Key;
            }

        }

        if(context.difficultySettings.OpportunityAttack(opportunityCheck))
        {
            blackboard.choosenAgressiveAction = quickestAttack;
            return State.Success;
        }
        return State.Failure;
    }
}
