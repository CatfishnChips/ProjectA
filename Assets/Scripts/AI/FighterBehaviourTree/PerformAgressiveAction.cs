using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class PerformAgressiveAction : ActionNode
{
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        Debug.Log("BEN VARIM BEN!");
        string choosenAttack = blackboard.choosenAgressiveAction;
        if(choosenAttack == null) return State.Failure; // This is just a safe check if the AI didn't choose to make an attack the tree should not even execute this node.

        EventManager.Instance.P2AttackMove?.Invoke(choosenAttack);
        return State.Running;
    }
}
