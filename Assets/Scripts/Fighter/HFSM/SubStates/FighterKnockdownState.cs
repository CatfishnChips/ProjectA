using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterKnockdownState : FighterBaseState
{
    private CollisionData _collisionData;
    private ActionAttack _action;

    public FighterKnockdownState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override bool CheckSwitchState()
    {
        if (_currentFrame >= _action.Knockdown.Stun.stun){   
            if(IdleStateSwitchCheck()) return true; 
            
            SwitchState(_factory.GetSubState(FighterSubStates.Idle));
            return true;
        }
        else{
            return false;
        }
    }

    public override void EnterState()
    {
        _currentFrame = 0;
        _ctx.CurrentFrame = _currentFrame;
        _collisionData = _ctx.HurtCollisionData;
        _action = _collisionData.action;
        _ctx.HealthManager.UpdateHealth(_collisionData.action.Knockdown.damage);

        if (_action.Knockdown.Stun.stun == 0) return;

        if (_ctx.PreviousSubState == FighterStates.Knockup || _ctx.PreviousSubState == FighterStates.SlamDunk){
            CameraController.Instance.ScreenShake(new Vector3(0f, -0.1f, 0f));
            return; // If transitioned from Knockup state, do not play the animation.
        } 

        ActionDefault action = _ctx.ActionDictionary["Knockdown"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;
        AnimationClip colClip = action.boxAnimation;

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = colClip;
        

        float speedVar = AdjustAnimationTime(clip, _action.KnockdownStun);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);
        _ctx.ColBoxAnimator.SetFloat("SpeedVar", speedVar);

        _ctx.Animator.PlayInFixedTime("Action");
        _ctx.ColBoxAnimator.PlayInFixedTime("Action");
    }

    public override void ExitState()
    {
        _ctx.CurrentFrame = 0;
        //_ctx.Gravity = 0f;
        _ctx.Drag = 0f;
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Velocity = _ctx.CurrentMovement;
        _ctx.FighterController.targetVelocity = _ctx.Velocity;
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
