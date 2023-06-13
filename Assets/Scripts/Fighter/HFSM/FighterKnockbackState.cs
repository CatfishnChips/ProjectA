using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterKnockbackState : FighterBaseState
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private float _currentFrame = 0;
    private Vector2 _velocity;
    private float _animationSpeed;
    private bool _isFirstTime = true;

    public FighterKnockbackState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (_ctx.IsHurt){
            SwitchState(_factory.Stunned());
        }

        if (_currentFrame >= _action.KnockbackStun){   
            SwitchState(_factory.Idle());
        }
    }

    public override void EnterState()
    {
        _currentFrame = 0;
        _collisionData = _ctx.CollisionData;
        _action = _collisionData.action;

        if (_action.KnockbackStun == 0) return;

        ActionDefault action = _ctx.ActionDictionary["Block"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;

        _ctx.AnimOverrideCont["Block"] = clip;

        float speedVar = AdjustAnimationTime(clip, _action.HitStun);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);

        _ctx.Animator.Play("Block");
        _ctx.ColBoxAnimator.Play("Block");
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

