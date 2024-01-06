using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterFreeFallState : FighterBaseState
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private int _currentFrame = 0;
    private Vector2 _velocity;
    private float _animationSpeed;
    private bool _isFirstTime = true;
    private float _groundOffset; // Character's starting distance from the ground (this assumes the ground level is y = 0).
    private float _gravity;
    private float _drag;
    private float _time;
    private int _frame;

    public FighterFreeFallState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

   public override void CheckSwitchState()
    {
        if (_currentFrame >= _frame + _action.HitStop){   
            FighterBaseState state;         
            
            // Knockup always transitions to Knockdown state.
            if (_action.KnockdownStun > 0){
                state = _factory.GetSubState(FighterSubStates.Knockdown);
            }
            else{
                state = _factory.GetSubState(FighterSubStates.Idle);
            }
            SwitchState(state);
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
        _velocity = Vector2.zero;

        _groundOffset = _ctx.transform.position.y - 0.5f; // y = 0.5f is the centre position of the character.
        float horizontalDirection = Mathf.Sign(_collisionData.hitbox.Transform.right.x);

        _gravity = Physics2D.gravity.y; // Gravity Constant
        _time = Mathf.Sqrt(2 * _groundOffset / -_gravity); // Free Fall h = (1/2)gt^2
        _frame = Mathf.CeilToInt(_time / Time.fixedDeltaTime);
        Debug.Log("FighterFreeFallState(EnterState) - Time: " + _time + " Frame: " + _frame);

        //_velocity.y = (_action.Knockup - (_gravity * Mathf.Pow(_time, 2) / 2)) / _time; // Initial Velocity
        _velocity.y = 0f;

        _drag = -2 * _action.Knockback / Mathf.Pow(_time, 2);
        _drag *= horizontalDirection;

        _velocity.x = 2 * _action.Knockback / _time; // Initial horizontal velocity;
        _velocity.x *= horizontalDirection;

        // Apply Calculated Variables
        _ctx.Drag = _drag;
        _ctx.Gravity = _gravity;
        _ctx.CurrentMovement = _velocity;

        ActionDefault action = _ctx.ActionDictionary["Knockup"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;
        AnimationClip colClip = action.boxAnimation;

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = colClip;

        _animationSpeed = AdjustAnimationTime(clip, _action.KnockupStun.y); 

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
        _ctx.CurrentFrame = 0;
        _ctx.CurrentMovement = Vector2.zero;
        //Debug.Log("Fighter SlamDunk State - Exit State");
    }

    public override void FixedUpdateState()
    {
        if (_currentFrame >= _action.HitStop){

            if (_isFirstTime){
                _ctx.Animator.SetFloat("SpeedVar", _animationSpeed);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", _animationSpeed);
                _isFirstTime = false;
            }
            _ctx.Drag = _drag;
            _ctx.Gravity = _gravity;
            //Debug.Log("FighterFreeFallState - Frame: " + _currentFrame + " Velocity Applied: " + (_ctx.CurrentMovement + new Vector2(_ctx.Drag, _ctx.Gravity) * Time.fixedDeltaTime));
        }
        
        CheckSwitchState();
        _currentFrame++;
        _ctx.CurrentFrame =_currentFrame;
        //Debug.Log("FighterFreeFallState(FixedUpdateState) - Frame: " + _currentFrame + " Velocity: " + _ctx.Rigidbody2D.velocity);
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }
}
