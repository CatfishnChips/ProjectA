using UnityEngine;

public class FighterAirborneState : FighterBaseState
{   
    private Rigidbody2D _rb;

    public FighterAirborneState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _isRootState = true;
    }

    public override void CheckSwitchState()
    {
        if (_ctx.IsHurt){
            SwitchState(_factory.Stunned());
        }
        
        if(_ctx.IsGrounded && _ctx.CurrentSubState != FighterStates.Jump){
            SwitchState(_factory.Grounded());
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        _rb = _ctx.Rigidbody2D;
    }

    public override void ExitState()
    {
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Velocity = Vector2.zero;
        _rb.velocity = Vector2.zero;
    }

    public override void FixedUpdateState()
    {  
        //_ctx.CurrentMovement += new Vector2(_ctx.CurrentSubState == FighterStates.Walk ? 0f : _ctx.Drag, _ctx.IsGravityApplied ? _ctx.Gravity : 0f) * Time.fixedDeltaTime;
        _ctx.CurrentMovement += new Vector2(0f, _ctx.IsGravityApplied ? _ctx.Gravity : 0f) * Time.fixedDeltaTime;
        //_ctx.Velocity = _ctx.IsGravityApplied ? _ctx.CurrentMovement : Vector2.zero; 
        _ctx.Velocity = _ctx.CurrentMovement;
        _ctx.Rigidbody2D.velocity = _ctx.Velocity;
        //Debug.Log("Velocity Applied: " + _ctx.Velocity);
        CheckSwitchState();
    }

    public override void InitializeSubState()
    {
        FighterBaseState state;
        if (_ctx.IsJumpPressed){
            state = _factory.Jump();
        }
        else{
            state = _factory.Idle();
        }

        SetSubState(state);
        state.EnterState();
    }

    public override void UpdateState()
    { 
        
    }
}
