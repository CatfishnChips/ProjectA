using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterJumpState : FighterBaseState
{
    private ActionDefault _action;
    private Rigidbody2D _rb;
    private float _initialJumpVelocity;
    private float _timeToApex;
    private int _currentFrame = 0;

    public FighterJumpState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {   
        if(_currentFrame >= _ctx.JumpTime){
            SwitchState(_factory.Idle());
        }
    }

    public override void EnterState()
    {
        _ctx.IsJumpPressed = false;
        _currentFrame = 0;
        _action = _ctx.ActionDictionary["Jump"] as ActionDefault;
        AnimationClip clip = _action.meshAnimation;

        float _time = _ctx.JumpTime * Time.fixedDeltaTime;

        _ctx.Gravity = (-2 * _ctx.JumpHeight) / (_time * _time);
        _initialJumpVelocity = (2 * _ctx.JumpHeight) / _time;

        float speedVar = AdjustAnimationTime(clip, _ctx.JumpTime);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);
        _ctx.Animator.Play("Jump");

        _ctx.CurrentMovement = new Vector2(-_ctx.SwipeDirection.x * _ctx.JumpDistance, _initialJumpVelocity);
        _ctx.Velocity = new Vector2(-_ctx.SwipeDirection.x * _ctx.JumpDistance, _initialJumpVelocity);
        //Debug.Log("Jump Velocity: " + _ctx.Velocity);
    }

    public override void ExitState()
    {
        //_ctx.Gravity = Physics2D.gravity.y;
    }

    public override void FixedUpdateState()
    {
        _currentFrame++;
        CheckSwitchState();
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    { 
    }
}
