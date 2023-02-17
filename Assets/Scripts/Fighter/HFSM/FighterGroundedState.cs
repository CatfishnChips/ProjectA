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
        if (_ctx.IsJumpPressed){
            SwitchState(_factory.Jump());
        }
        if(_ctx.UppercutPerformed){
            _ctx.UppercutPerformed = false;
            SwitchState(_factory.Attack());
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
        //_ctx.CharController.Move(_ctx.Velocity * _ctx.MoveSpeed);

    }
}
