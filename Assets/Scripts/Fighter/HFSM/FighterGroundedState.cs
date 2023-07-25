using UnityEngine;

public class FighterGroundedState : FighterBaseState
{
    public FighterGroundedState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _isRootState = true;
    }

    public override void CheckSwitchState()
    {
        if (_ctx.IsHurt){
            SwitchState(_factory.Stunned());
        }

        if (!_ctx.IsGrounded || (_ctx.IsJumpPressed && (_ctx.CurrentSubState == FighterStates.Idle || _ctx.CurrentSubState == FighterStates.Walk))){
            SwitchState(_factory.Airborne());
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
    }

    public override void ExitState()
    {    
        _ctx.Gravity = 0f;
        _ctx.Drag = 0f;
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Velocity = Vector2.zero;
        _ctx.Rigidbody2D.velocity = Vector2.zero;
    }

    public override void FixedUpdateState()
    {
        _ctx.CurrentMovement += new Vector2(_ctx.Drag, _ctx.Gravity) * Time.fixedDeltaTime;
        _ctx.Velocity = _ctx.CurrentMovement;

        if(_ctx.Velocity != Vector2.zero)
        _ctx.Rigidbody2D.velocity = _ctx.Velocity;
        CheckSwitchState();
    }

    public override void InitializeSubState()
    {
        FighterBaseState state;
        if(_ctx.MovementInput == 0){
            state = _factory.Idle();
        }
        else{
            state = _factory.Walk();
        }
        SetSubState(state);
        state.EnterState();
    }

    public override void UpdateState()
    {     
    }
}
