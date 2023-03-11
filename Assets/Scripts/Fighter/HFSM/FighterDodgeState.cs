using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterDodgeState : FighterBaseState
{
    private ActionDefault _action;
    private int _currentFrame = 0;

    public FighterDodgeState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _stateName = "Dodge";
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= _ctx.DodgeTime){
            SwitchState(_factory.Idle());
        }
    }

    public override void EnterState()
    {
        _currentFrame = 0;
        _ctx.IsInputLocked = true;
        if (_ctx.IsGrounded) 
        {
            _action = _ctx.ActionDictionary["Dodge"] as ActionDefault;
        }
        else 
        {
            _action = _ctx.ActionDictionary["Dodge"] as ActionDefault;
        }
        AnimationClip clip = _action.meshAnimation;
        _ctx.AnimOverrideCont["Dash"] = clip;

        float speedVar = AdjustAnimationTime(clip, _ctx.DodgeTime);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);

        _ctx.Animator.Play("Dodge");
        _ctx.ColBoxAnimator.Play("Dodge");
    }

    public override void ExitState()
    {
        _ctx.IsDodgePressed = false;
        _ctx.IsInputLocked = false;
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
