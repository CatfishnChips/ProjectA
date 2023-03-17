using UnityEngine;

public class FighterWalkState : FighterBaseState
{
    public FighterWalkState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if(_ctx.MovementInput == 0){
            SwitchState(_factory.Idle());
        }

        if (_ctx.AttackPerformed){
            SwitchState(_factory.Attack());
        }

        if (_ctx.IsDashPressed){
            SwitchState(_factory.Dash());
        }

        if (_ctx.IsDodgePressed){
            SwitchState(_factory.Dodge());
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
        if (_ctx.IsGrounded){
            _ctx.Animator.SetFloat("Blend", _ctx.MovementInput);
        }
        else{
            _ctx.CurrentMovement = new Vector2(_ctx.MovementInput * _ctx.AirMoveSpeed, _ctx.Velocity.y);
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
