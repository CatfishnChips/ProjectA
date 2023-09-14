using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterKnockbackState : FighterBaseState
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private int _currentFrame = 0;
    private float _animationSpeed;
    private bool _isFirstTime = true;
    private float _drag;
    private float _initialVelocity;
    private float _direction;
    private float _time;

    public FighterKnockbackState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= _action.KnockbackStun + _action.HitStop){   
            SwitchState(_factory.GetSubState(FighterSubStates.Idle));
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


        if (_action.Knockback!= 0){
            _direction = Mathf.Sign(_collisionData.hitbox.Transform.right.x);
            _time = _action.KnockbackStun * Time.fixedDeltaTime;

            _drag = -2 * _action.Knockback / Mathf.Pow(_time, 2);
            _drag *= _direction;

            _initialVelocity = 2 * _action.Knockback / _time; // Initial horizontal velocity;
            _initialVelocity *= _direction;

            _ctx.Drag = _drag;
            _ctx.CurrentMovement = new Vector2(_initialVelocity, _ctx.CurrentMovement.y);
        }

        if (_action.KnockbackStun == 0) return;

        ActionDefault action = _ctx.ActionDictionary["Stunned"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;

        _ctx.AnimOverrideCont["Stunned"] = clip;

        _animationSpeed = AdjustAnimationTime(clip, _action.KnockbackStun); 

        if (_action.HitStop != 0){
            _ctx.Animator.SetFloat("SpeedVar", 0f);
        }
        else{
            _ctx.Animator.SetFloat("SpeedVar", _animationSpeed);
        }

        _ctx.Animator.Play("Stunned");
        _ctx.ColBoxAnimator.Play("Idle");
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
                _isFirstTime = false;
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

