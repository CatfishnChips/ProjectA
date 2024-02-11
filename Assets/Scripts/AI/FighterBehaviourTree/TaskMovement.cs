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
            if(context.selfFSM.FaceDirection == 1) context.inputEvents.OnTap(ScreenSide.Left);
            else context.inputEvents.OnTap(ScreenSide.Right);
        }
        else if (context.distanceToOpponent.x > context.optimalDistance + context.distanceMargin)
        {
            if(context.selfFSM.FaceDirection == -1) context.inputEvents.OnTap(ScreenSide.Left);
            else context.inputEvents.OnTap(ScreenSide.Right);
        }

    }
}
