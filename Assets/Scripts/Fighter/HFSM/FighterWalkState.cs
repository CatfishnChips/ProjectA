using UnityEngine;

public class FighterWalkState : FighterBaseState
{
    public FighterWalkState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (_ctx.AttackPerformed){
            SwitchState(_factory.Attack());
        }
        else if (_ctx.IsDodgePressed){
            SwitchState(_factory.Dodge());
        }
        else if (_ctx.IsDashPressed){
            SwitchState(_factory.Dash());
        }
        else if(_ctx.MovementInput == 0){
            SwitchState(_factory.Idle());
        }
    }

    public override void EnterState()
    {
        if (_ctx.IsGrounded)
        {
            _ctx.Animator.Play("MoveBT");
        }
    }

    public override void ExitState()
    {
        _ctx.Velocity = new Vector2(0, _ctx.Velocity.y);
        _ctx.Animator.SetFloat("Blend", 0f);
    }

    public override void FixedUpdateState()
    {
        if (_ctx.CurrentRootState == FighterStates.Grounded){
            _ctx.Animator.SetFloat("Blend", _ctx.MovementInput);
        }
        else if (_ctx.CurrentRootState == FighterStates.Airborne)
        {
            _ctx.CurrentMovement = new Vector2(_ctx.MovementInput * _ctx.AirMoveSpeed, _ctx.CurrentMovement.y);
        }

        CheckSwitchState();
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }
}
