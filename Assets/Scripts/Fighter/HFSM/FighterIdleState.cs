using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterIdleState : FighterBaseState
{
    public FighterIdleState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _stateName = "Idle";
    }

    public override void CheckSwitchState()
    {
        if (_ctx.Velocity.x != 0){
            if (_ctx.Velocity.x >= -0.5f && _ctx.Velocity.x <= 0.5f){
                SwitchState(_factory.Walk());
            }
            else if(_ctx.Velocity.x < -0.5f || _ctx.Velocity.x > 0.5f){
                SwitchState(_factory.Run());
            }
        }

    }

    public override void EnterState()
    {
        Debug.Log("ENTERED IDLE STATE");
    }

    public override void ExitState()
    {
        Debug.Log("EXITED IDLE STATE");
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }
}
