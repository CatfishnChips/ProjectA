using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class PerformDefensiveAction : ActionNode
{

    private int dodgeWaitStartFrame;

    protected override void OnStart()
    {
        dodgeWaitStartFrame = 0;
    }

    protected override void OnStop()
    {
        dodgeWaitStartFrame = 0;
    }

    protected override State OnUpdate()
    {
        string choosenAction = blackboard.choosenDefensiveAction;
        if(choosenAction == null) return State.Failure; // This is just a safe check if the AI didn't choose to make an attack the tree should not even execute this node.

        if(choosenAction == "Dodge")
        {
            if(dodgeWaitStartFrame >= blackboard.enemyAttackAction.StartFrames - 2) 
            {
                context.inputEvents.Swipe?.Invoke(new Vector2(1.0f, 1.0f));
                return State.Success;
            }
            else
            {
                dodgeWaitStartFrame++;
                return State.Running;
            } 
        }
        else{
            context.inputEvents.AttackMove?.Invoke(choosenAction);
            return State.Success;
        }
    }
}
