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
        _stateName = "Jump";
    }

    public override void CheckSwitchState()
    {   
        if(_currentFrame >= _ctx.JumpTime){
            SwitchState(_factory.Idle());
        }
    }

    public override void EnterState()
    {
        _action = _ctx.ActionDictionary["Jump"] as ActionDefault;
        AnimationClip clip = _action.meshAnimation;

        float _time = _ctx.JumpTime * Time.fixedDeltaTime;

        _ctx.Gravity = (-2 * _ctx.JumpHeight) / Mathf.Pow(_time, 2);
        _initialJumpVelocity = (2 * _ctx.JumpHeight) / _time;

        float speedVar = AdjustAnimationTime(clip, _ctx.JumpTime);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);
        _ctx.Animator.Play("Jump");

        _ctx.CurrentMovement = new Vector2(_ctx.SwipeDirection.x * _ctx.JumpDistance, _initialJumpVelocity);
        _ctx.Velocity = new Vector2(-_ctx.SwipeDirection.x * _ctx.JumpDistance, _initialJumpVelocity);
        //Debug.Log("Jump Velocity: " + _ctx.Velocity);
    }

    public override void ExitState()
    {
        _ctx.Gravity = Physics2D.gravity.y;
        _ctx.IsJumpPressed = false;
    }

    public override void FixedUpdateState()
    {
        CheckSwitchState();
        _currentFrame++;
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        
    }
}
