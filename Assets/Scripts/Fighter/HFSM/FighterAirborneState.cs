using UnityEngine;

public class FighterAirborneState : FighterBaseState
{   
    private ActionDefault _action;
    private Rigidbody2D _rb;
    private float _initialJumpVelocity;
    private int _currentFrame = 0;
    private float _groundOffset; // Character's starting distance from the ground (this assumes the ground level is y = 0).
    private Vector2 _velocity;
    private float _gravity1, _gravity2;
    private float _drag1, _drag2;
    private float _distancePerTime;

    public FighterAirborneState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _isRootState = true;
    }

    public override void CheckSwitchState()
    {
        if (_ctx.IsHurt){
            SwitchState(_factory.Stunned());
        }
        
        if(_ctx.IsGrounded && _currentFrame >= _ctx.JumpTime + _ctx.FallTime){
            SwitchState(_factory.Grounded());
        }
    }

    public override void EnterState()
    {
        _ctx.Gravity = Physics2D.gravity.y;
        _ctx.Drag = 0f;
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Velocity = Vector2.zero;
        _ctx.Rigidbody2D.velocity = Vector2.zero;

        _currentFrame = 0;
        
        // Jump Action
        if (_ctx.IsJumpPressed && _ctx.IsGrounded && _ctx.PreviousRootState == FighterStates.Grounded){
            _ctx.IsJumpPressed = false;
        
           _groundOffset = _ctx.transform.position.y - 0.5f; // y = 0.5f is the centre position of the character.
            //Debug.Log(_groundOffset);
            float direction = _ctx.SwipeDirection.x == 0 ? 0f : -Mathf.Sign(_ctx.SwipeDirection.x);
            _distancePerTime = _ctx.JumpDistance / (_ctx.JumpTime + _ctx.FallTime);
            
            // Zone 1
            float time1 = _ctx.JumpTime * Time.fixedDeltaTime;
            _gravity1 = - 2 * (_ctx.JumpHeight) / (time1 * time1);
            _velocity.y = 2 * (_ctx.JumpHeight) / time1; // Initial vertical velocity.

            _drag1 = (-_distancePerTime * time1) / (time1 * time1);
            _drag1 *= direction;

            _velocity.x = (_ctx.JumpDistance) / time1; // Initial horizontal velocity;
            _velocity.x *= direction;

            // Zone 2
            float time2 = _ctx.FallTime * Time.fixedDeltaTime; 
            _gravity2 = -2 * (_ctx.JumpHeight + _groundOffset) / (time2 * time2);

            _drag2 = (-_distancePerTime * time2) / (time2 * time2);
            _drag2 *= direction;
        }
        else{
            _ctx.Gravity = Physics2D.gravity.y;
            _ctx.Drag = 0f;
        }
       
        _ctx.CurrentMovement = _velocity;

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
        _ctx.Drag = _currentFrame <= _ctx.JumpTime ? _drag1 : _drag2;
        _ctx.Gravity = _currentFrame <= _ctx.JumpTime ? _gravity1 : _gravity2;

        _ctx.CurrentMovement += new Vector2(_ctx.IsGravityApplied ? _ctx.Drag : 0f, _ctx.IsGravityApplied ? _ctx.Gravity : 0f) * Time.fixedDeltaTime;
        _ctx.Velocity = _ctx.IsGravityApplied ? _ctx.CurrentMovement : Vector2.zero;
        //_ctx.Velocity = _ctx.CurrentMovement;
        _ctx.Rigidbody2D.velocity = _ctx.Velocity;
        //Debug.Log("Velocity Applied: " + _ctx.Velocity);
        CheckSwitchState();
        if (_ctx.IsGravityApplied) _currentFrame++;
    }

    public override void InitializeSubState()
    {
        FighterBaseState state;
        // if (_ctx.IsJumpPressed){
        //     state = _factory.Jump();
        // }
        // else{
        //     state = _factory.Idle();
        // }
        state = _factory.Idle();

        SetSubState(state);
        state.EnterState();
    }

    public override void UpdateState()
    { 
        
    }
}
