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
        string choosenMethod = blackboard.choosenDefenseMethod;
        if(choosenMethod == null) return State.Failure; // This is just a safe check if the AI didn't choose to make an attack the tree should not even execute this node.

        if(choosenMethod == "Dodge")
        {
            if(dodgeWaitStartFrame >= blackboard.enemyAttackAction.StartFrames - 2) 
            {
                context.inputEvents.OnSwipe?.Invoke(ScreenSide.Left, GestureDirections.Left, GestureDirections.None);
                return State.Success;
            }
            else
            {
                dodgeWaitStartFrame++;
                return State.Running;
            } 
        }
        else{
            context.inputEvents.DirectAttackInputByAction?.Invoke(blackboard.choosenCounterAttack);
            return State.Success;
        }
    }
}
