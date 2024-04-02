using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterJumpState : FighterBaseState
{
    private ActionDefault _action;
    private Rigidbody2D _rb;
    private float _initialJumpVelocity;
    private int _currentFrame = 0;
    private float _groundOffset; // Character's starting distance from the ground (this assumes the ground level is y = 0).
    private Vector2 _velocity;
    private float _gravity1, _gravity2;
    private float _drag;
    private float _time;
    private float _direction;

    public FighterJumpState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {   
        if(_currentFrame >= _ctx.JumpTime){
            SwitchState(_factory.GetSubState(FighterSubStates.Idle));
        }
    }

    public override void EnterState()
    {
        _ctx.Gravity = 0f;
        _ctx.Drag = 0f;
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Velocity = Vector2.zero;
        _ctx.FighterController.targetVelocity = Vector2.zero;

        _currentFrame = 0;

        // Free Fall: h = ut + 1/2gt^2
        // Gravity: g = -2(h - ut) / t^2
        // Initial Velocity: u = 2h / t 

        // h = Height (Known)
        // t = Time (Known)
        // g = Gravity (Calculated)
        // u = Initial Velocity (Calculated) 
        
        // Initial Velocity: u = 2x / t
        // Drag: a = -2x / t^2
        // Kinematic Movement: Δx = v + at

        // x = Distance (Known)
        // t = Time (Known)
        // a = Drag (Calculated)    
        // u = Initial Velocity (Calculated) 
        
        // Jump Action
        if (_ctx.JumpInput.Read() && _ctx.IsGrounded && _ctx.PreviousRootState == FighterStates.Grounded){
        
            _groundOffset = _ctx.transform.position.y - 0.5f; // y = 0.5f is the centre position of the character.
            _direction = _ctx.SwipeDirection.x == 0 ? 0f : _ctx.SwipeDirection.x;
                
            _time = _ctx.JumpTime * Time.fixedDeltaTime;

            _drag = -2 * _ctx.JumpDistance / Mathf.Pow(_time, 2);
            _drag *= _direction;

            _velocity.x = 2 * _ctx.JumpDistance / _time; // Initial horizontal velocity;
            _velocity.x *= _direction;
            
            // Zone 1
            float time1 = _ctx.JumpTime * Time.fixedDeltaTime;
            _gravity1 = - 2 * _ctx.JumpHeight / Mathf.Pow(time1, 2); // g = 2h/t^2
            _velocity.y = 2 * _ctx.JumpHeight / time1; // Initial vertical velocity. V0y = 2h/t
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
        float time = _ctx.FallTime * Time.fixedDeltaTime;
        _ctx.Gravity = -2 * (_ctx.JumpHeight + _groundOffset) / (time * time);
    }

    public override void FixedUpdateState()
    {
        CheckSwitchState();
        _currentFrame++;
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    { 
    }
}
