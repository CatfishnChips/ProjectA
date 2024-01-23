using UnityEngine;
using TheKiwiCoder;

public class TaskMovement : ActionNode
{
    protected override void OnStart(){}

    protected override void OnStop(){}

    protected override State OnUpdate()
    {
        NonAttackingStateMovement();
        return State.Success;
    }

    private void NonAttackingStateMovement()
    {
        // Debug.Log("Distance: " + context.distanceToOpponent);
        // Debug.Log("Optimal: " + context.optimalDistance);
        // Debug.Log("Margin:" + context.distanceMargin);
        if (context.distanceToOpponent.x < context.optimalDistance - context.distanceMargin)
        {
            context.inputEvents.Move(-1.0f * context.selfFSM.FaceDirection);
        }
        else if (context.distanceToOpponent.x > context.optimalDistance + context.distanceMargin)
        {
            context.inputEvents.Move(context.selfFSM.FaceDirection);
        }
        else
        {
            context.inputEvents.Move(0.0f);
        }

    }
}
