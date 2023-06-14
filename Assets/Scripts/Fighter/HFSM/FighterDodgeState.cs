using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterDodgeState : FighterBaseState
{
    private ActionDefault _action;
    private int _currentFrame = 0;

    public FighterDodgeState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= _ctx.DodgeTime){
            SwitchState(_factory.Idle());
        }
    }

    public override void EnterState()
    {
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
        _ctx.AnimOverrideCont["Dodge"] = clip;

        float speedVar = AdjustAnimationTime(clip, _ctx.DodgeTime);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);

        _ctx.Animator.Play("Dodge");
        _ctx.ColBoxAnimator.Play("Dodge");
    }

    public override void ExitState()
    {
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
