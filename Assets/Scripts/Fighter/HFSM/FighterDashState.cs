using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterDashState : FighterBaseState
{
    private ActionDefault _action;
    private int _currentFrame = 0;
    private float _drag = 0f;

    public FighterDashState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _stateName = "Dash";
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= (_ctx.DashTime)){
            SwitchState(_factory.Idle());
        }
    }

    public override void EnterState()
    {
        _currentFrame = 0;
        _ctx.IsGravityApplied = false;
        _ctx.IsInputLocked = true;
        if (_ctx.IsGrounded) 
        {
            _action = _ctx.ActionDictionary["Dash"] as ActionDefault;
        }
        else 
        {
            _action = _ctx.ActionDictionary["Dash"] as ActionDefault;
        }
        AnimationClip clip = _action.meshAnimation;
        _ctx.AnimOverrideCont["Dash"] = clip;

        float speedVar = AdjustAnimationTime(clip, _ctx.DashTime);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);

        _ctx.Animator.Play("Dash");
        _ctx.ColBoxAnimator.Play("Dash");

        float direction = Mathf.Sign(_ctx.SwipeDirection.x);
        float _time = _ctx.DashTime * Time.fixedDeltaTime;

        _drag = (-2 * _ctx.DashDistance) / (_time * _time);
        float _initialDashVelocity = (2 * _ctx.DashDistance) / _time;

        _drag = _drag * direction;
        _initialDashVelocity = _initialDashVelocity * direction;

        _ctx.CurrentMovement = new Vector2(_initialDashVelocity, _ctx.CurrentMovement.y);
        _ctx.Velocity = _ctx.CurrentMovement;
    }

    public override void ExitState()
    {
        _ctx.IsDashPressed = false;
        _ctx.IsInputLocked = false;
        _ctx.IsGravityApplied = true;
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Velocity = _ctx.CurrentMovement;
    }

    public override void FixedUpdateState()
    {
        float previousVelocityY = _ctx.CurrentMovement.y;
        _ctx.CurrentMovement = new Vector2(_ctx.CurrentMovement.x + _drag * Time.fixedDeltaTime, _ctx.CurrentMovement.y);
        _ctx.Velocity = new Vector2((previousVelocityY + _ctx.CurrentMovement.x) * .5f, _ctx.CurrentMovement.y);    

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
