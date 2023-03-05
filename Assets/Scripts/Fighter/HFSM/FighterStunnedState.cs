using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterStunnedState : FighterBaseState
{   
    private CollisionData _collisionData;
    private float _currentFrame = 0;

    public FighterStunnedState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _stateName = "Stunned";
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= _collisionData.stunDuration){            
            SwitchState(_factory.Idle());
        }

        if (_ctx.IsHurt){
            SwitchState(_factory.Stunned());
        }
    }

    public override void EnterState()
    {
        _collisionData = _ctx.CollisionData;
        _ctx.IsHurt = false;
        _ctx.IsInputLocked = true;

        int stunDuration = _collisionData.stunDuration;
        if (stunDuration == 0) return;

        _ctx.Animator.Play("Stunned");
        _ctx.ColBoxAnimator.Play("Idle");

        ActionDefault action = _ctx.ActionDictionary["Stunned"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;

        // Find a way to get the stunned clip.
        //AnimationClip clip = _ctx.Animator.GetCurrentAnimatorClipInfo(0)[0].clip;

        float speedVar = AdjustAnimationTime(clip, stunDuration); 
        _ctx.Animator.SetFloat("SpeedVar", speedVar);
    }

    public override void ExitState()
    {
        _ctx.IsInputLocked = false;
    }

    public override void FixedUpdateState()
    {
        CheckSwitchState();
        _currentFrame++;

        // If the fighter is hurt again before the stun duration expires, duration is refreshed.
        // if (_ctx.IsHurt)
        // {
        //     _ctx.IsHurt = false;
        //     _collisionData = _ctx.CollisionData;
        //     _currentFrame = 0;
        // }
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
       
    }
}
