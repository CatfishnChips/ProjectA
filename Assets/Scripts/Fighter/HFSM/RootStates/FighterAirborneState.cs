using UnityEngine;

public class FighterAirborneState : FighterBaseState
{   
    // private ActionDefault _action;
    // private Rigidbody2D _rb;
    // private float _initialJumpVelocity;
    private float _groundOffset; // Character's starting distance from the ground (this assumes the ground level is y = 0).
    private Vector2 _velocity;
    private float _gravity1, _gravity2;
    private float _drag;
    private float _time;
    private float _direction;

    public FighterAirborneState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _isRootState = true;
    }

    public override bool CheckSwitchState()
    {
        if (_ctx.IsHurt && !_ctx.IsInvulnerable){
            SwitchState(_factory.GetRootState(FighterRootStates.Stunned));
            return true;
        }
        
        // if(_ctx.IsGrounded && _currentFrame >= _ctx.JumpTime + _ctx.FallTime){
        if(_ctx.IsGrounded){
            SwitchState(_factory.GetRootState(FighterRootStates.Grounded));
            return true;
        }

        return false;
    }

    public override void EnterState()
    {
        // _ctx.Gravity = 0f;
        // _ctx.Drag = 0f;
        // _ctx.CurrentMovement = Vector2.zero;
        // _ctx.Velocity = Vector2.zero;
        //_ctx.Rigidbody2D.velocity = Vector2.zero;
        // _ctx.FighterController.targetVelocity = Vector2.zero;

        _currentFrame = 0;
        
        // // Jump Action
        // if (_ctx.JumpInput.Read() && _ctx.IsGrounded && _ctx.PreviousRootState == FighterStates.Grounded){
        
        //     _groundOffset = _ctx.transform.position.y - 0.5f; // y = 0.5f is the centre position of the character.
        //     _direction = _ctx.SwipeDirection.x == 0 ? 0f : _ctx.SwipeDirection.x;
                
        //     _time = (_ctx.JumpTime + _ctx.FallTime) * Time.fixedDeltaTime;
        //     _drag = -2 * _ctx.JumpDistance / Mathf.Pow(_time, 2);
        //     _drag *= _direction;

        //     _velocity.x = 2 * _ctx.JumpDistance / _time; // Initial horizontal velocity;
        //     _velocity.x *= _direction;
            
        //     // Zone 1
        //     float time1 = _ctx.JumpTime * Time.fixedDeltaTime;
        //     _gravity1 = - 2 * _ctx.JumpHeight / Mathf.Pow(time1, 2); // g = 2h/t^2
        //     _velocity.y = 2 * _ctx.JumpHeight / time1; // Initial vertical velocity. V0y = 2h/t

        //     // Zone 2
        //     float time2 = _ctx.FallTime * Time.fixedDeltaTime; 
        //     _gravity2 = -2 * (_ctx.JumpHeight + _groundOffset) / (time2 * time2); // Free Fall h = (1/2)gt^2
        // }
        // else{
        //     _ctx.Gravity = Physics2D.gravity.y;
        //     _ctx.Drag = 0f;
        // }
       
        // _ctx.CurrentMovement = _velocity;
    }

    public override void ExitState()
    {
        // _ctx.Gravity = 0f;
        // _ctx.Drag = 0f;
        // _ctx.CurrentMovement = Vector2.zero;
        // _ctx.Velocity = Vector2.zero;
        //_ctx.Rigidbody2D.velocity = Vector2.zero;
        // _ctx.FighterController.targetVelocity = Vector2.zero;
    }

    public override void FixedUpdateState()
    {  
        // _ctx.Drag = _drag;
        // _ctx.Gravity = _currentFrame <= _ctx.JumpTime ? _gravity1 : _gravity2;

        _ctx.CurrentMovement += new Vector2(_ctx.IsGravityApplied ? _ctx.Drag : 0f, _ctx.IsGravityApplied ? _ctx.Gravity : 0f) * Time.fixedDeltaTime;
        _ctx.Velocity = _ctx.IsGravityApplied ? _ctx.CurrentMovement : Vector2.zero;
        //_ctx.Velocity = _ctx.CurrentMovement;
        _ctx.FighterController.targetVelocity = _ctx.Velocity;
        //Debug.Log("Velocity Applied: " + _ctx.Velocity);
        CheckSwitchState();
        if (_ctx.IsGravityApplied) _currentFrame++;
    }

    public override void InitializeSubState()
    {  
        FighterBaseState state;
        state = _factory.GetSubState(FighterSubStates.Idle);

        SetSubState(state);
        state.EnterState();
    }

    public override void UpdateState()
    { 
        
    }
}
