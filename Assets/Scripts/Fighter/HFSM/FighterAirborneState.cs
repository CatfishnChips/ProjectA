using UnityEngine;

public class FighterAirborneState : FighterBaseState
{
    public FighterAirborneState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _stateName = "Airborne";
        _isRootState = true;
    }

    public override void CheckSwitchState()
    {
        if(_ctx.IsGrounded){
            SwitchState(_factory.Grounded());
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
        SetSubState(_factory.Idle());
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }
}
