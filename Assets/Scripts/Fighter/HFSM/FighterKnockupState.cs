using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterKnockupState : FighterBaseState
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private int _currentFrame = 0;
    private Vector2 _velocity;
    private float _animationSpeed;
    private bool _isFirstTime = true;
    private float _groundOffset; // Character's starting distance from the ground (this assumes the ground level is y = 0).
    private float _gravity1, _gravity2;
    private float _drag1, _drag2;
    private float _distancePerTime;
    private Vector2 _arcDirection;

    public FighterKnockupState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (_ctx.IsHurt){
            SwitchState(_factory.Stunned());
        }

        if (_currentFrame >= _action.KnockupStun.x + _action.KnockupStun.y + _action.Freeze){   
            FighterBaseState state;         
            
            // Knockup always transitions to Knockdown state.
            if (_action.KnockdownStun > 0){
                state = _factory.Knockdown();
            }
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

        _groundOffset = _ctx.transform.position.y;
        float horizontalDirection = -Mathf.Sign(_collisionData.hurtbox.Transform.right.x);
        _distancePerTime = _action.Knockback / (_action.KnockupStun.x + _action.KnockupStun.y);
        
        // Zone 1
        float time1 = _action.KnockupStun.x;
        _gravity1 = (-2 * _action.Knockup) / (time1 * time1);
        _velocity.y = (2 * _action.Knockup) / time1; // Initial vertical velocity.

        _drag1 = (-2 * _distancePerTime * time1) / (time1 * time1);
        _drag1 *= horizontalDirection;

        _velocity.x = (2 * _action.Knockback) / time1; // Initial horizontal velocity;
        _velocity.x *= horizontalDirection;

        // Zone 2
        float time2 = _action.KnockupStun.y;
        _gravity2 = (-2 * _action.Knockup + _groundOffset) / (time2 * time2);

        _drag2 = (-2 * _distancePerTime * time2) / (time2 * time2);
        _drag2 *= horizontalDirection;

        _arcDirection = _velocity.normalized;

        _ctx.CurrentMovement = _velocity;
        _ctx.Velocity = _ctx.CurrentMovement;

        if (_action.KnockupStun.x + _action.KnockupStun.y == 0) return;

        ActionDefault action = _ctx.ActionDictionary["Knockup"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;

        _ctx.AnimOverrideCont["Knockup"] = clip;

        _animationSpeed = AdjustAnimationTime(clip, _action.KnockupStun.x + _action.KnockupStun.y); 

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
            
            _ctx.CurrentMovement = new Vector2(_ctx.CurrentMovement.x + 
            (_currentFrame < _action.KnockupStun.x + _action.Freeze ? _drag1 : _drag2), _ctx.CurrentMovement.y + 
            (_currentFrame < _action.KnockupStun.x + _action.Freeze ? _gravity1 : _gravity2));

            _ctx.Velocity = _ctx.CurrentMovement;   

            // if (_ctx.Velocity.y <= 0){
            //     _ctx.CurrentMovement = new Vector2(_ctx.CurrentMovement.x + _drag * Time.fixedDeltaTime, _ctx.CurrentMovement.y + _ctx.Gravity * _ctx.FallMultiplier * Time.fixedDeltaTime);
            // }
            // else{
            //     _ctx.CurrentMovement = new Vector2(_ctx.CurrentMovement.x + _drag * Time.fixedDeltaTime, _ctx.CurrentMovement.y + _ctx.Gravity * Time.fixedDeltaTime);
            // }    
            //_ctx.CurrentMovement = new Vector2(_ctx.CurrentMovement.x + _drag * Time.fixedDeltaTime, _ctx.CurrentMovement.y + _ctx.Gravity * _ctx.FallMultiplier * Time.fixedDeltaTime); // Using this calculation makes the frame timing off due to _ctx.FallMultiplier.
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
