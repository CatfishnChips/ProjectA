using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterWallSplatState : FighterBaseState
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private float _animationSpeed;

    private int _stun;
    private int _hitstop;

    public FighterWallSplatState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override bool CheckSwitchState()
    {
        if (_currentFrame >= _stun + _hitstop){   
            if (IdleStateSwitchCheck()) return true;

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
        _ctx.CurrentFrame =_currentFrame;
        _collisionData = _ctx.HurtCollisionData;
        _action = _collisionData.action;
        _ctx.IsHurt = false;

        _stun = _ctx.IsGrounded ? _action.Ground.WallSplat.Stun.stun : _action.Air.WallSplat.Stun.stun;
        _hitstop = _ctx.IsGrounded ? _action.Ground.WallSplat.Stun.hitStop : _action.Air.WallSplat.Stun.hitStop;

        _ctx.HealthManager.UpdateHealth(_ctx.IsGrounded ? _collisionData.action.Ground.WallSplat.damage : _collisionData.action.Air.WallSplat.damage);

        // Apply Calculated Variables
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Drag = 0f;
        //_ctx.Gravity = 0f;

        if (_action.WallSplatStun == 0) return;

        ActionDefault action = _ctx.ActionDictionary["Stunned"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;
        AnimationClip boxClip = action.boxAnimation;

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = boxClip;

        _animationSpeed = AdjustAnimationTime(clip, _action.WallSplatStun); 

        _ctx.Animator.SetFloat("SpeedVar", _animationSpeed);
        _ctx.ColBoxAnimator.SetFloat("SpeedVar", _animationSpeed);

        _ctx.Animator.PlayInFixedTime("Action");
        _ctx.ColBoxAnimator.PlayInFixedTime("Action");
    }

    public override void ExitState()
    {
        _ctx.Drag = 0f;
        _ctx.CurrentFrame = 0;
        _ctx.CurrentMovement = Vector2.zero;
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
