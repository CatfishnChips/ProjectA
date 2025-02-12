using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterWallSplatState : FighterBaseState
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private float _animationSpeed;
    private bool _isFirstTime = true;

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

        _stun = _ctx.WasGrounded ? _action.Ground.WallSplat.Stun.stun : _action.Air.WallSplat.Stun.stun;
        _hitstop = _ctx.WasGrounded ? _action.Ground.WallSplat.Stun.hitStop : _action.Air.WallSplat.Stun.hitStop;

        _ctx.HealthManager.UpdateHealth(_ctx.WasGrounded ? _collisionData.action.Ground.WallSplat.damage : _collisionData.action.Air.WallSplat.damage);

        // Apply Calculated Variables
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Drag = 0f;
        //_ctx.Gravity = 0f;

        if (_stun == 0) return;

        ActionDefault action = _ctx.ActionDictionary["Stunned"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;
        AnimationClip boxClip = action.boxAnimation;

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = boxClip;

        _animationSpeed = AdjustAnimationTime(clip, _stun); 

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
        if (_currentFrame >= _hitstop){

            if (_isFirstTime){
                _ctx.Animator.SetFloat("SpeedVar", 1f);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", 1f);
                _isFirstTime = false;
            }

            //Debug.Log("Fighter Wall Splat State - Frame: " + _currentFrame + " Velocity Applied: " + (_ctx.CurrentMovement + new Vector2(_ctx.Drag, _ctx.Gravity) * Time.fixedDeltaTime));
        }

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
