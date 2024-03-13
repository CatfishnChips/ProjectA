using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter KnockBack State", menuName = "FighterStates/Sub/KnockBackState")]
public class FighterKnockbackState : ActionDefault
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private float _animationSpeed;
    private bool _isFirstTime = true;
    private float _drag;
    private float _initialVelocity;
    private float _direction;
    private float _time;

    public override void Initialize(IStateMachineRunner ctx, FighterStateFactory factory)
    {
        base.Initialize(ctx, factory);
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= _action.KnockbackStun + _action.HitStop){   
            SwitchState(_factory.GetSubState(FighterStates.Idle));
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

        _direction = Mathf.Sign(_collisionData.hitbox.Transform.right.x);
        _time = _action.KnockbackStun * Time.fixedDeltaTime;

        _drag = -2 * _action.Knockback / Mathf.Pow(_time, 2);
        _drag *= _direction;

        _initialVelocity = 2 * _action.Knockback / _time; // Initial horizontal velocity;
        _initialVelocity *= _direction;

        // Apply Calculated Variables
        _ctx.Drag = _drag;
        _ctx.Gravity = 0f;

        if (_action.KnockbackStun == 0) return;

        AnimationClip clip = meshAnimation;
        AnimationClip boxClip = boxAnimation;

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = boxClip;

        _animationSpeed = AdjustAnimationTime(clip, _action.KnockbackStun); 

        if (_action.HitStop != 0){
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
        if (_currentFrame >= _action.HitStop){

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

