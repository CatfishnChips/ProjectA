using UnityEngine;

public class FighterWalkState : FighterBaseState
{
    public FighterWalkState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _stateName = "Walk";
    }

    public override void CheckSwitchState()
    {
        if(_ctx.DeltaTarget == 0){
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
        
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        if (_ctx.IsGrounded){
            _ctx.Animator.SetFloat("Blend", _ctx.DeltaTarget);
        }
        else{
            _ctx.CurrentMovement = new Vector2(_ctx.DeltaTarget * _ctx.AirMoveSpeed, _ctx.Velocity.y);
        }

        CheckSwitchState();
    }
}
