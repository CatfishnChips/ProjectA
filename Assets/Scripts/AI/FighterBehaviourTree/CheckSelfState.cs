using System.Diagnostics;
using TheKiwiCoder;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CheckSelfState : ActionNode
{
    public FighterStates fighterState;

    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {   
        FighterAttackState attack;
        if(context.selfFSM.CurrentState.GetCurrentSubState is FighterAttackState attackState)
        {
            attack = attackState;
            Debug.Log("Self State is: " + context.selfFSM.CurrentSubState + "attack is: " + attack.Action.name);
        }        
        if(fighterState.HasFlag(context.selfFSM.CurrentSubState)) return State.Success;
        else return State.Failure;
    }
}
