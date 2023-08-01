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
    private float _colliderAnimationSpeed;
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

        _groundOffset = _ctx.transform.position.y; // y = 0.5f is the centre position of the character.
        //Debug.Log(_groundOffset);
        float horizontalDirection = -Mathf.Sign(_collisionData.hurtbox.Transform.right.x);
        _distancePerTime = _action.Knockback / (_action.KnockupStun.x + _action.KnockupStun.y);
        
        // Zone 1
        float time1 = _action.KnockupStun.x * Time.fixedDeltaTime;
        _gravity1 = -2 * (_action.Knockup) / (time1 * time1);
        _velocity.y = 2 * (_action.Knockup) / time1; // Initial vertical velocity.

        _drag1 = (-_distancePerTime * time1) / (time1 * time1);
        _drag1 *= horizontalDirection;

        _velocity.x = (_action.Knockback) / time1; // Initial horizontal velocity;
        _velocity.x *= horizontalDirection;

        // Zone 2
        float time2 = _action.KnockupStun.y * Time.fixedDeltaTime; 
        _gravity2 = -2 * (_action.Knockup) / (time2 * time2);

        _drag2 = (-_distancePerTime * time2) / (time2 * time2);
        _drag2 *= horizontalDirection;

        _arcDirection = _velocity.normalized;

        _ctx.CurrentMovement = _velocity;

        if (_action.KnockupStun.x + _action.KnockupStun.y == 0) return;

        ActionDefault action = _ctx.ActionDictionary["Knockup"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;
        AnimationClip colClip = action.boxAnimation;

        _ctx.AnimOverrideCont["Knockup"] = clip;
        _ctx.AnimOverrideCont["Idle"] = colClip;

        _animationSpeed = AdjustAnimationTime(clip, _action.KnockupStun.x + _action.KnockupStun.y); 
        _colliderAnimationSpeed = AdjustAnimationTime(colClip, _action.KnockupStun.x + _action.KnockupStun.y); 

        if (_action.Freeze != 0){
            _ctx.Animator.SetFloat("SpeedVar", 0f);
            _ctx.ColBoxAnimator.SetFloat("SpeedVar", 0f);
        }
        else{
            _ctx.Animator.SetFloat("SpeedVar", _animationSpeed);
            _ctx.ColBoxAnimator.SetFloat("SpeedVar", _colliderAnimationSpeed);
        }

        _ctx.Animator.Play("Knockup");
        _ctx.ColBoxAnimator.Play("Idle");
    }

    public override void ExitState()
    {
        _ctx.Gravity = 0f;
        _ctx.Drag = 0f;
        _ctx.CurrentMovement = Vector2.zero;
        //Debug.Log("Fighter Knockup State - Exit State");
    }

    public override void FixedUpdateState()
    {
        if (_currentFrame >= _action.Freeze){

            if (_isFirstTime){
                _ctx.Animator.SetFloat("SpeedVar", _animationSpeed);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", _colliderAnimationSpeed);
                _isFirstTime = false;
            }
            _ctx.Drag = _currentFrame < _action.KnockupStun.x + _action.Freeze ? _drag1 : _drag2;
            _ctx.Gravity = _currentFrame < _action.KnockupStun.x + _action.Freeze ? _gravity1 : _gravity2;
            //Debug.Log("Fighter Knockup State - Frame: " + _currentFrame + " Velocity Applied: " + (_ctx.CurrentMovement + new Vector2(_ctx.Drag, _ctx.Gravity) * Time.fixedDeltaTime));
        }
        
        CheckSwitchState();
        _currentFrame++;
        //Debug.Log("Fighter Knockup State - FixedUpdate Complete");
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }
}
