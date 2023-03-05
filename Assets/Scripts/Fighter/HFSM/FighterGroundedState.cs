using UnityEngine;

public class FighterGroundedState : FighterBaseState
{
    private Vector2 currentMovement;

    public FighterGroundedState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _stateName = "Grounded";
        _isRootState = true;
    }

    public override void CheckSwitchState()
    {
        if (!_ctx.IsGrounded || _ctx.IsJumpPressed){
            SwitchState(_factory.Airborne());
        }
    }

    public override void EnterState()
    {
        InitializeSubState();

        _ctx.Gravity = Physics2D.gravity.y;
        _ctx.Rigidbody2D.velocity = new Vector2(_ctx.Rigidbody2D.velocity.x, _ctx.Gravity);
    }

    public override void ExitState()
    {
        
    }

    public override void FixedUpdateState()
    {
        //_ctx.Rigidbody2D.velocity = new Vector2(_ctx.Rigidbody2D.velocity.x, _ctx.Gravity);
    }

    public override void InitializeSubState()
    {
        FighterBaseState state;
        if(_ctx.DeltaTarget == 0){
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
        CheckSwitchState();
    }
}
