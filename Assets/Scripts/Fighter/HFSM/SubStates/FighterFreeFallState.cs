using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterFreeFallState : FighterBaseState
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private Vector2 _initialVelocity;
    private float _animationSpeed;
    private bool _isFirstTime = true;
    private float _groundOffset; // Character's starting distance from the ground (this assumes the ground level is y = 0).
    private float _gravity;
    private float _drag;
    private int _hitStop;

    public FighterFreeFallState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

   public override bool CheckSwitchState()
    {
        if (_ctx.IsGrounded){ 
            // Knockup always transitions to Knockdown state.
            if (((HitFlags)_action.Ground.HitFlags).HasFlag(HitFlags.KNOCK_DOWN)){
                SwitchState(_factory.GetSubState(FighterSubStates.Knockdown));
                return true;
            }
            else{
                if(IdleStateSwitchCheck()) return true; 
                
                SwitchState(_factory.GetSubState(FighterSubStates.Idle));
                return true;
            }
        }
        else return false;
    }

    public override void EnterState()
    {
        _currentFrame = 0;
        _ctx.CurrentFrame =_currentFrame;
        _isFirstTime = true;
        _collisionData = _ctx.HurtCollisionData;
        _action = _collisionData.action;
        _ctx.IsHurt = false;
        _initialVelocity = Vector2.zero;

        _hitStop = _action.Air.Stun.hitStop;

        _ctx.HealthManager.UpdateHealth(_action.Damage);

        _groundOffset = _ctx.transform.position.y - 0.5f; // y = 0.5f is the centre position of the character.
        float horizontalDirection = Mathf.Sign(_collisionData.hitbox.Transform.right.x);

        _initialVelocity = _action.Air.Trajectory.Line.velocity;
        _initialVelocity.x *= horizontalDirection;

        _drag = 0;

        // Apply Calculated Variables
        _ctx.CurrentMovement = _initialVelocity;
        _ctx.Drag = _drag;
        _ctx.Gravity = _ctx.GravityConstant;
        
        ActionDefault action = _ctx.ActionDictionary["Knockup"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;
        AnimationClip colClip = action.boxAnimation;

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = colClip;

        //_animationSpeed = AdjustAnimationTime(clip, _action.KnockupStun.y); 

        if (_hitStop != 0){
            _ctx.Animator.SetFloat("SpeedVar", 0f);
            _ctx.ColBoxAnimator.SetFloat("SpeedVar", 0f);
        }
        else{
            _ctx.Animator.SetFloat("SpeedVar", 1f);
            _ctx.ColBoxAnimator.SetFloat("SpeedVar", 1f);
        }

        _ctx.Animator.PlayInFixedTime("Action");
        _ctx.ColBoxAnimator.PlayInFixedTime("Action");
    }

    public override void ExitState()
    {
        // _ctx.Gravity = 0f;
        _ctx.Drag = 0f;
        _ctx.CurrentFrame = 0;
        _ctx.CurrentMovement = Vector2.zero;
        //Debug.Log("Fighter SlamDunk State - Exit State");
    }

    public override void FixedUpdateState()
    {
        if (_currentFrame >= _hitStop){

            if (_isFirstTime){
                _ctx.Animator.SetFloat("SpeedVar", 1f);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", 1f);
                _isFirstTime = false;

                // Apply Calculated Initial Velocity
                _ctx.CurrentMovement = _initialVelocity;
            }

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