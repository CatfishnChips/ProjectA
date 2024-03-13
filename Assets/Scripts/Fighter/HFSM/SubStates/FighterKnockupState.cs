using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter KnockUp State", menuName = "FighterStates/Sub/KnockUpState")]
public class FighterKnockupState : ActionDefault
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private Vector2 _velocity;
    private float _animationSpeed;
    private float _colliderAnimationSpeed;
    private bool _isFirstTime = true;
    private float _groundOffset; // Character's starting distance from the ground (this assumes the ground level is y = 0).
    private float _gravity1, _gravity2;
    private float _drag1, _drag2;
    private float _time1, _time2;
    private float _distancePerTime;
    private float _direction;

    private float _drag;
    private float _time;

    public override void Initialize(IStateMachineRunner ctx, FighterStateFactory factory)
    {
        base.Initialize(ctx, factory);
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= _action.KnockupStun.x + _action.KnockupStun.y + _action.HitStop){   
            FighterBaseState state;         
            
            // Knockup always transitions to Knockdown state.
            if (_action.KnockdownStun > 0){
                state = _factory.GetSubState(FighterStates.Idle);
            }
            else{
                state = _factory.GetSubState(FighterStates.Idle);
            }
            SwitchState(state);
        }
    }

    public override void EnterState()
    {
        //Debug.Log("FighterKnockupState(EnterState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Root State: " + _ctx.CurrentRootState + " SubState: " + _ctx.CurrentSubState);
        _currentFrame = 0;
        _ctx.CurrentFrame =_currentFrame;
        _isFirstTime = true;
        _collisionData = _ctx.HurtCollisionData;
        _action = _collisionData.action;
        _ctx.IsHurt = false;
        _velocity = Vector2.zero;

        _groundOffset = _ctx.transform.position.y - 0.5f; // y = 0.5f is the centre position of the character.
        _direction = Mathf.Sign(_collisionData.hitbox.Transform.right.x);
    
        _time = (_action.KnockupStun.x + _action.KnockupStun.y) * Time.fixedDeltaTime;
        _drag = -2 * _action.Knockback / Mathf.Pow(_time, 2);
        _drag *= _direction;

        _velocity.x = 2 * _action.Knockback / _time; // Initial horizontal velocity;
        _velocity.x *= _direction;
        
        // Zone 1
        float time1 = _action.KnockupStun.x * Time.fixedDeltaTime;
        _gravity1 = - 2 * _action.Knockup / Mathf.Pow(time1, 2); // g = 2h/t^2
        _velocity.y = 2 * _action.Knockup / time1; // Initial vertical velocity. V0y = 2h/t

        // Zone 2
        float time2 = _action.KnockupStun.y * Time.fixedDeltaTime; 
        _gravity2 = -2 * (_action.Knockup + _groundOffset) / (time2 * time2); // Free Fall h = (1/2)gt^2
        
        // Apply Calculated Variables
        _ctx.Drag = _drag;
        _ctx.Gravity = _gravity1;
        

        if (_action.KnockupStun.x + _action.KnockupStun.y == 0) return;

        AnimationClip clip = meshAnimation;
        AnimationClip colClip = boxAnimation;

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = colClip;

        _animationSpeed = AdjustAnimationTime(clip, _action.KnockupStun.x + _action.KnockupStun.y); 

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
        _ctx.Gravity = 0f;
        _ctx.Drag = 0f;
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.CurrentFrame = 0;
        //Debug.Log("Fighter Knockup State - Exit State");
    }

    public override void FixedUpdateState()
    {
        //Debug.Log("FighterKnockupState(FixedUpdateState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Root State: " + _ctx.CurrentRootState + " SubState: " + _ctx.CurrentSubState);
        if (_currentFrame >= _action.HitStop){

            if (_isFirstTime){
                _ctx.Animator.SetFloat("SpeedVar", _animationSpeed);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", _animationSpeed);
                _isFirstTime = false;
                
                // Apply Calculated Initial Velocity
                _ctx.CurrentMovement = _velocity;
            }
            _ctx.Drag = _drag;
            _ctx.Gravity = _currentFrame < _action.KnockupStun.x + _action.HitStop ? _gravity1 : _gravity2;
            //Debug.Log("FighterKnockupState(FixedUpdateState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Drag: " + _ctx.Drag + " Gravity: " + _ctx.Gravity + " Current Movement: " + _ctx.CurrentMovement + " Velocity: " + _ctx.Velocity);
            //Debug.Log("Fighter Knockup State - Frame: " + _currentFrame + " Velocity Applied: " + (_ctx.CurrentMovement + new Vector2(_ctx.Drag, _ctx.Gravity) * Time.fixedDeltaTime));
        }
        
        CheckSwitchState();
        _currentFrame++;
        _ctx.CurrentFrame =_currentFrame;
        //Debug.Log("Fighter Knockup State - FixedUpdate Complete");
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }
}
