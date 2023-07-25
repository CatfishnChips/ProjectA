using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterDashState : FighterBaseState
{
    private ActionConditional _action;
    private int _currentFrame = 0;

    public FighterDashState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= (_ctx.DashTime)){
            SwitchState(_factory.Idle());
        }
    }

    public override void EnterState()
    {
        _ctx.IsDashPressed = false;
        _currentFrame = 0;
        _ctx.IsGravityApplied = false;
        _ctx.Drag = 0f;

        if (_ctx.IsGrounded) 
        {
            _action = _ctx.ActionDictionary["Dash"] as ActionConditional;
        }
        else 
        {
            _action = _ctx.ActionDictionary["Dash"] as ActionConditional;
        }

        AnimationClip clip;
        if (_ctx.SwipeDirection.x < 0)
        {
            clip = _action.Animations[0].meshAnimation;
        }
        else
        {
            clip = _action.Animations[1].meshAnimation;
        }
        
        _ctx.AnimOverrideCont["Dash"] = clip;
        //_ctx.ColBoxOverrideCont["Dash"]

        // For this action, DashTime variable is used instead of animation's Frame variable.
        float speedVar = AdjustAnimationTime(clip, _ctx.DashTime);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);

        _ctx.Animator.Play("Dash");
        _ctx.ColBoxAnimator.Play("Idle");

        float direction = -Mathf.Sign(_ctx.SwipeDirection.x);
        
        float _time = _ctx.DashTime * Time.fixedDeltaTime;

        _ctx.Drag = (-2 * _ctx.DashDistance) / (_time * _time) * direction;
        float _initialDashVelocity = (2 * _ctx.DashDistance) / _time;

        _initialDashVelocity = _initialDashVelocity * direction;

        _ctx.CurrentMovement = new Vector2(_initialDashVelocity, _ctx.CurrentMovement.y);
    }

    public override void ExitState()
    {
        _ctx.IsGravityApplied = true;
        _ctx.Drag = 0f;
        _ctx.CurrentMovement = Vector2.zero;
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
