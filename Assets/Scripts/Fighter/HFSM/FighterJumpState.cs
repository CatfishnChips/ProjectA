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
        _ctx.IsJumpPressed = false;
        _currentFrame = 0;
        _action = _ctx.ActionDictionary["Jump"] as ActionDefault;
        AnimationClip clip = _action.meshAnimation;
        _groundOffset = _ctx.transform.position.y;

        float direction = _ctx.SwipeDirection.x == 0 ? 0f : -Mathf.Sign(_ctx.SwipeDirection.x);

        float time = _ctx.JumpTime * Time.fixedDeltaTime;

        _ctx.Gravity = -_ctx.JumpHeight / (time * time);
        _ctx.Drag = (-_ctx.JumpDistance / (time * time)) * direction;
        _velocity.x = _ctx.JumpDistance / ((_ctx.JumpTime + _ctx.FallTime) * Time.fixedDeltaTime); // Initial horizontal velocity;
        _velocity.y = _ctx.JumpHeight / time; // Initial vertical velocity;
        _velocity.x *= direction;

        float speedVar = AdjustAnimationTime(clip, _ctx.JumpTime);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);
        _ctx.Animator.Play("Jump");

        _ctx.CurrentMovement = _velocity;
        //Debug.Log("Jump Velocity: " + _ctx.Velocity);
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
