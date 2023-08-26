using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterDodgeState : FighterBaseState
{
    protected ActionDefault _action;
    protected int _currentFrame = 0;
    protected bool _isFirstTime = true;

    public FighterDodgeState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= _ctx.DodgeTime.x + _ctx.DodgeTime.y){
            SwitchState(_factory.GetSubState(FighterSubStates.Idle));
        }
    }

    public override void EnterState()
    {
        _isFirstTime = true;
        _ctx.IsDodgePressed = false;
        _currentFrame = 0;
        
        if (_ctx.IsGrounded) 
        {
            _action = _ctx.ActionDictionary["Dodge"] as ActionDefault;
        }
        else 
        {
            _action = _ctx.ActionDictionary["Dodge"] as ActionDefault;
        }
        AnimationClip clip = _action.meshAnimation;
        AnimationClip boxClip = _action.boxAnimation;
        _ctx.AnimOverrideCont["Dodge"] = clip;
        _ctx.ColBoxOverrideCont["Dodge"] = boxClip;

        float speedVar = AdjustAnimationTime(clip, _ctx.DodgeTime.x + _ctx.DodgeTime.y);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);
        float boxSpeedVar = AdjustAnimationTime(boxClip, _ctx.DodgeTime.x + _ctx.DodgeTime.y);
        _ctx.ColBoxAnimator.SetFloat("SpeedVar", boxSpeedVar);

        _ctx.Animator.Play("Dodge");
        _ctx.ColBoxAnimator.Play("Idle");
    }

    public override void ExitState()
    {
        _ctx.IsInvulnerable = false;
    }

    public override void FixedUpdateState()
    {
        if (_currentFrame > _ctx.DodgeTime.x){

            if (_isFirstTime){
                _ctx.IsInvulnerable = true;
                _isFirstTime = false;
            }

            if (_ctx.IsHurt){
                Debug.Log("Script: FighterDodgeState - FixedUpdateState : Attack Dodged");
                //_ctx.Focus = true;
                //_ctx.ResetFocusTimer();
                _ctx.IsHurt = false;
            }
        }
        
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
