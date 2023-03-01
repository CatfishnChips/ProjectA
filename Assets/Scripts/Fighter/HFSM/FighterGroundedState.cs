using UnityEngine;

public class FighterGroundedState : FighterBaseState
{
    public FighterGroundedState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _stateName = "Grounded";
        _isRootState = true;
    }

    public override void CheckSwitchState()
    {
        if (!_ctx.IsGrounded){
            SwitchState(_factory.Airborne());
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
    }

    public override void ExitState()
    {
        
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void InitializeSubState()
    {
        if(_ctx.Velocity.x == 0){
            SetSubState(_factory.Idle());
        }
        else{
            SetSubState(_factory.Walk());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }
}
