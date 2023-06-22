using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterKnockupState : FighterBaseState
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private float _currentFrame = 0;
    private Vector2 _velocity;
    private float _drag = 0f;
    private float _animationSpeed;
    private bool _isFirstTime = true;

    public FighterKnockupState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (_ctx.IsHurt){
            SwitchState(_factory.Stunned());
        }

        if (_currentFrame >= _action.KnockupStun + _action.Freeze){   
            FighterBaseState state;         
            
            // Knockup always transitions to Knockdown state.
            if (_action.KnockdownStun > 0){
                state = _factory.Knockdown();
            }
            // Take another look here.
            // if(_ctx.IsGrounded){
            //     state = _factory.Knockdown();
            // }
            else{
                state = _factory.Idle();
            }
            SwitchState(state);
        }
    }

    public override void EnterState()
    {
        _currentFrame = 0;
        _collisionData = _ctx.HurtCollisionData;
        _action = _collisionData.action;
        _ctx.IsHurt = false;
        _velocity = Vector2.zero;

        if (_action.Knockup != 0){
            float _time = _action.KnockupStun * Time.fixedDeltaTime;
            _ctx.Gravity = (-2 * _action.Knockup) / Mathf.Pow(_time, 2);
            _velocity.y = (2 * _action.Knockup) / _time;   
        }

        if (_action.Knockback!= 0){
            //_velocity.x = Mathf.Sign(_collisionData.hurtbox.Transform.forward.x) * _action.Knockback;

            float direction = -Mathf.Sign(_collisionData.hurtbox.Transform.right.x);
            float _time = _action.KnockbackStun * Time.fixedDeltaTime;

            _drag = (-2 * _action.Knockback) / (_time * _time);
            _velocity.x = (2 * _action.Knockback) / _time;

            _drag *= direction;
            _velocity.x *= direction;
        }

        _ctx.CurrentMovement = _velocity;
        _ctx.Velocity = _ctx.CurrentMovement;

        if (_action.KnockupStun == 0) return;

        ActionDefault action = _ctx.ActionDictionary["Knockup"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;

        _ctx.AnimOverrideCont["Knockup"] = clip;

        _animationSpeed = AdjustAnimationTime(clip, _action.KnockupStun); 

        if (_action.Freeze != 0){
            _ctx.Animator.SetFloat("SpeedVar", 0f);
        }
        else{
            _ctx.Animator.SetFloat("SpeedVar", _animationSpeed);
        }

        _ctx.Animator.Play("Knockup");
        _ctx.ColBoxAnimator.Play("Idle");
    }

    public override void ExitState()
    {
        _ctx.Gravity = Physics2D.gravity.y;
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Velocity = _ctx.CurrentMovement;
    }

    public override void FixedUpdateState()
    {
        if (_currentFrame > _action.Freeze){

            if (_isFirstTime){
                _ctx.Animator.SetFloat("SpeedVar", _animationSpeed);
                _isFirstTime = false;
            }

            float previousVelocityY = _ctx.CurrentMovement.y;

            // if (_ctx.Velocity.y <= 0){
            //     _ctx.CurrentMovement = new Vector2(_ctx.CurrentMovement.x + _drag * Time.fixedDeltaTime, _ctx.CurrentMovement.y + _ctx.Gravity * _ctx.FallMultiplier * Time.fixedDeltaTime);
            // }
            // else{
            //     _ctx.CurrentMovement = new Vector2(_ctx.CurrentMovement.x + _drag * Time.fixedDeltaTime, _ctx.CurrentMovement.y + _ctx.Gravity * Time.fixedDeltaTime);
            // }    

            _ctx.CurrentMovement = new Vector2(_ctx.CurrentMovement.x + _drag * Time.fixedDeltaTime, _ctx.CurrentMovement.y + _ctx.Gravity * _ctx.FallMultiplier * Time.fixedDeltaTime); // Using this calculation makes the frame timing off.

            _ctx.Velocity = new Vector2(_ctx.CurrentMovement.x , Mathf.Max((previousVelocityY + _ctx.CurrentMovement.y) * .5f, -20f));    
        }
        
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
