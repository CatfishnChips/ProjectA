using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FighterStunnedState : FighterBaseState
{   
    private CollisionData _collisionData;
    private ActionAttack _action;
    private float _currentFrame = 0;
    private Vector2 _velocity;
    private float _animationSpeed;
    private bool _isFirstTime = true;

    public FighterStunnedState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _isRootState = true;
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= _action.HitStun + _action.Freeze){   
            FighterBaseState state;         

            if (_ctx.IsGrounded){
                state = _factory.Grounded();
            }
            else{
                state = _factory.Airborne();
            }
            SwitchState(state);
        }

        if (_ctx.IsHurt){
            SwitchState(_factory.Stunned());
        }
    }

    public override void EnterState()
    {
        _currentFrame = 0;
        _collisionData = _ctx.CollisionData;
        _action = _collisionData.action;
        _ctx.IsHurt = false;

        _velocity = Vector2.zero;

        if (_action.Knockup != 0){
            float _time = _ctx.JumpTime * Time.fixedDeltaTime;
            _ctx.Gravity = (-2 * _ctx.JumpHeight) / Mathf.Pow(_time, 2);
            _velocity.y = (2 * _ctx.JumpHeight) / _time;   
        }

        if (_action.Knockback!= 0){
            _velocity.x = Mathf.Sign(_collisionData.hurtbox.Transform.forward.x) * _action.Knockback;
        }

        _ctx.CurrentMovement = _velocity;
        _ctx.Velocity = _ctx.CurrentMovement;
        _ctx.Rigidbody2D.velocity = _ctx.Velocity;

        _ctx.HealthManager.UpdateHealth(_collisionData);

        if (_action.HitStun == 0) return;

        _ctx.Animator.Play("Stunned");
        _ctx.ColBoxAnimator.Play("Idle");

        ActionDefault action = _ctx.ActionDictionary["Stunned"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;

        _animationSpeed = AdjustAnimationTime(clip, _action.HitStun); 

        if (_action.Freeze != 0){
            _ctx.Animator.SetFloat("SpeedVar", 0f);
        }
        else{
            _ctx.Animator.SetFloat("SpeedVar", _animationSpeed);
        }
    }

    public override void ExitState()
    {
        _ctx.Gravity = Physics2D.gravity.y;
    }

    public override void FixedUpdateState()
    {   
        if (_currentFrame > _action.Freeze){

            if (_isFirstTime){
                _ctx.Animator.SetFloat("SpeedVar", _animationSpeed);
                _isFirstTime = false;
            }

            float previousVelocityY = _ctx.CurrentMovement.y;
            if (_ctx.Velocity.y <= 0){    
                _ctx.CurrentMovement = new Vector2(_ctx.CurrentMovement.x, _ctx.CurrentMovement.y + _ctx.Gravity * _ctx.FallMultiplier * Time.fixedDeltaTime);
            }
            else{
                _ctx.CurrentMovement = new Vector2(_ctx.CurrentMovement.x, _ctx.CurrentMovement.y + _ctx.Gravity * Time.fixedDeltaTime);
            }
            _ctx.Velocity = new Vector2(_ctx.Velocity.x, Mathf.Max((previousVelocityY + _ctx.CurrentMovement.y) * .5f, -20f));    
            _ctx.Rigidbody2D.velocity = _ctx.Velocity;

            //Debug.Log("Velocity Applied: " + _ctx.Velocity);
        }
        
        _currentFrame++;
        CheckSwitchState();

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
    }

    public override void UpdateState()
    {
    }
}
