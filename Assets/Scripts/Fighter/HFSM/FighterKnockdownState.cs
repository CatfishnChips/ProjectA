using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterKnockdownState : FighterBaseState
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private int _currentFrame = 0;

    public FighterKnockdownState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= _action.KnockdownStun){   
            SwitchState(_factory.GetSubState(FighterSubStates.Idle));
        }
    }

    public override void EnterState()
    {
        _currentFrame = 0;
        _ctx.CurrentFrame = _currentFrame;
        _collisionData = _ctx.HurtCollisionData;
        _action = _collisionData.action;

        if (_action.KnockdownStun == 0) return;

        if (_ctx.PreviousSubState == FighterStates.Knockup || _ctx.PreviousSubState == FighterStates.SlamDunk){
            CameraController.Instance.ScreenShake(new Vector3(0f, -0.1f, 0f));
            return; // If transitioned from Knockup state, do not play the animation.
        } 

        ActionDefault action = _ctx.ActionDictionary["Knockdown"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;

        _ctx.AnimOverrideCont["Knockdown"] = clip;

        float speedVar = AdjustAnimationTime(clip, _action.KnockdownStun);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);

        _ctx.Animator.Play("Knockdown");
        _ctx.ColBoxAnimator.Play("Idle");
    }

    public override void ExitState()
    {
        _ctx.CurrentFrame = 0;
    }

    public override void FixedUpdateState()
    {
        CheckSwitchState();
        _currentFrame++;
        _ctx.CurrentFrame =_currentFrame;
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }
}
