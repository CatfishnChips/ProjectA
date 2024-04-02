using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterGrabbedState : FighterBaseState
{
    private ActionDefault _action;
    private CollisionData _collisionData;
    private ActionAttack _attackAction;

    public FighterGrabbedState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override bool CheckSwitchState()
    {
        return false;
    }

    public override void EnterState()
    { 
        // Take the animation info from the Action as a paramater. 
        // Then let the animation handle all the work (movement).
        // The animation should move the character to the desired location while applying damage at certain times.
        _currentFrame = 0;
        _collisionData = _ctx.HurtCollisionData;
        _attackAction = _collisionData.action;
        _ctx.IsHurt = false;
        
        if (_ctx.IsGrounded) 
        {
            _action = _ctx.ActionDictionary["GroundedIdle"] as ActionDefault;
        }
        else
        {
            _action = _ctx.ActionDictionary["AirborneIdle"] as ActionDefault;
        }

        AnimationClip clip = _action.meshAnimation;
        AnimationClip boxClip = _action.boxAnimation;

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = boxClip;

        float speedVar = AdjustAnimationTime(clip, _action.frames);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);
        _ctx.ColBoxAnimator.SetFloat("SpeedVar", speedVar);

        _ctx.Animator.PlayInFixedTime("Action");
        _ctx.ColBoxAnimator.PlayInFixedTime("Action");
    }

    public override void ExitState()
    {
    }

    public override void FixedUpdateState()
    {
        CheckSwitchState();
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {

    }
}
