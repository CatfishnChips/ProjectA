using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterKnockbackState : FighterBaseState
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private float _animationSpeed;
    private bool _isFirstTime = true;
    private float _drag;
    private float _initialVelocity;
    private float _direction;
    private float _time;
    
    private int _stun;
    private int _hitStop;
    private int _slide;
    private float _distance;

    public FighterKnockbackState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override bool CheckSwitchState()
    {
        if (((HitFlags)_action.Ground.HitFlags).HasFlag(HitFlags.BOUNCE_WALL)){
            if(_ctx.FighterController.IsTouchingWall){
                SwitchState(_factory.GetSubState(FighterSubStates.WallBounce));
                return true;
            }
        } 

        if (((HitFlags)_action.Ground.HitFlags).HasFlag(HitFlags.SPLAT_WALL)){
            if(_ctx.FighterController.IsTouchingWall){
                SwitchState(_factory.GetSubState(FighterSubStates.WallSplat));
                return true;
            }
        } 

        if (_currentFrame >= _slide + _hitStop){  
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
        _ctx.CurrentFrame =_currentFrame;
        _isFirstTime = true;
        _collisionData = _ctx.HurtCollisionData;
        _action = _collisionData.action;
        _ctx.IsHurt = false;
        _ctx.HealthManager.UpdateHealth(_collisionData.action.Damage);

        _stun = _action.Ground.Stun.stun;
        _hitStop =  _action.Ground.Stun.hitStop;

        _slide = _action.Ground.Slide.slide;
        _distance = _action.Ground.Slide.distance;

        _direction = Mathf.Sign(_collisionData.hitbox.Transform.right.x);
        _time = _slide * Time.fixedDeltaTime;

        _drag = -2 * _distance / Mathf.Pow(_time, 2);
        _drag *= _direction;

        _initialVelocity = 2 * _distance / _time; // Initial horizontal velocity;
        _initialVelocity *= _direction;

        // Apply Calculated Variables
        _ctx.Drag = _drag;
        //_ctx.Gravity = 0f;

        if (_stun == 0) return;

        ActionDefault action = _ctx.ActionDictionary["Stunned"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;
        AnimationClip boxClip = action.boxAnimation;

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = boxClip;

        _animationSpeed = AdjustAnimationTime(clip, _slide); 

        if (_hitStop != 0){
            _ctx.Animator.SetFloat("SpeedVar", 0f);
            _ctx.ColBoxAnimator.SetFloat("SpeedVar", 0f);
        }
        else{
            _ctx.Animator.SetFloat("SpeedVar", _animationSpeed);
            _ctx.ColBoxAnimator.SetFloat("SpeedVar", _animationSpeed);
        }

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
        if (_currentFrame >= _hitStop){

            if (_isFirstTime){
                _ctx.Animator.SetFloat("SpeedVar", _animationSpeed);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", _animationSpeed);
                _isFirstTime = false;
                
                // Apply Calculated Initial Velocity
                _ctx.CurrentMovement = new Vector2(_initialVelocity, _ctx.CurrentMovement.y);
            }
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