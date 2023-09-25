using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class WaitForInputListener : ActionNode
{
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        if(!context.selfFSM.ValidAttackInputInterval)
        {
            return State.Running;
        }
        return State.Success;
    }
}
