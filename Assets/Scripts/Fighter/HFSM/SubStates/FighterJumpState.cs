using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Jump State", menuName = "FighterStates/Sub/JumpState")]
public class FighterJumpState : ActionDefault
{
    private Rigidbody2D _rb;
    private float _initialJumpVelocity;
    private float _groundOffset; // Character's starting distance from the ground (this assumes the ground level is y = 0).
    private Vector2 _velocity;

    public override void Initialize(IStateMachineRunner ctx, FighterStateFactory factory)
    {
        base.Initialize(ctx, factory);
    }

    public override void CheckSwitchState()
    {   
        if(_currentFrame >= _ctx.JumpTime){
            SwitchState(_factory.GetSubState(FighterStates.Idle));
        }
    }

    public override void EnterState()
    {
        _currentFrame = 0;
        AnimationClip clip = meshAnimation;
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
