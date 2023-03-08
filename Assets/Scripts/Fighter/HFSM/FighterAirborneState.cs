using UnityEngine;

public class FighterAirborneState : FighterBaseState
{   
    private Rigidbody2D _rb;

    public FighterAirborneState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _stateName = "Airborne";
        _isRootState = true;
    }

    public override void CheckSwitchState()
    {
        if(_ctx.IsGrounded && !_ctx.IsJumpPressed){
            SwitchState(_factory.Grounded());
        }

        if (_ctx.IsHurt){
            SwitchState(_factory.Stunned());
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        //_ctx.Gravity = Physics2D.gravity.y;
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
        if (_ctx.IsGravityApplied){
            float previousVelocityY = _ctx.CurrentMovement.y;
            if (_ctx.Velocity.y <= 0){
                _ctx.CurrentMovement = new Vector2(_ctx.CurrentMovement.x, _ctx.CurrentMovement.y + _ctx.Gravity * _ctx.FallMultiplier * Time.fixedDeltaTime);
            }
            else{
                _ctx.CurrentMovement = new Vector2(_ctx.CurrentMovement.x, _ctx.CurrentMovement.y + _ctx.Gravity * Time.fixedDeltaTime);
            }
            _ctx.Velocity = new Vector2(_ctx.CurrentMovement.x , Mathf.Max((previousVelocityY + _ctx.CurrentMovement.y) * .5f, -20f));    
        }
        else 
        {
            _ctx.Velocity = new Vector2(_ctx.CurrentMovement.x , 0);
        }
        _ctx.Rigidbody2D.velocity = _ctx.Velocity;
        Debug.Log("Velocity Applied: " + _ctx.Velocity);
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
