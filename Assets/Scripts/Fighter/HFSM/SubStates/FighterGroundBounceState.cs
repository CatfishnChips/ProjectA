using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterGroundBounceState : FighterBaseState
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private Vector2 _velocity;
    private float _animationSpeed;
    private float _colliderAnimationSpeed;
    private bool _isFirstTime = true;
    //private float _groundOffset; // Character's starting distance from the ground (this assumes the ground level is y = 0).
    private float _gravity1, _gravity2;
    private float _drag1, _drag2;
    private float _time1, _time2;
    private float _distancePerTime;
    private float _direction;

    private float _drag;
    private float _time;

    protected int _timeToApex;
    protected int _timeAtApex;
    protected int _timeToFall;
    protected float _apex;
    protected float _range;
    protected int _hitStop;
    protected int _stun;
       

    public FighterGroundBounceState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override bool CheckSwitchState()
    {   
        if (_currentFrame >= _timeToApex + _timeAtApex + _timeToFall){   
            // GroundBounce always transitions to Knockdown state.
            if (_action.HitFlags.HasFlag(HitFlags.KNOCK_DOWN)){
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
        //Debug.Log("FighterKnockupState(EnterState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Root State: " + _ctx.CurrentRootState + " SubState: " + _ctx.CurrentSubState);
        _currentFrame = 0;
        _ctx.CurrentFrame =_currentFrame;
        _isFirstTime = true;
        _collisionData = _ctx.HurtCollisionData;
        _action = _collisionData.action;
        _ctx.IsHurt = false;
        _velocity = Vector2.zero;

        _timeToApex = _ctx.IsGrounded ? _action.Ground.GroundBounce.Arc.timeToApex : _action.Air.GroundBounce.Arc.timeToApex;
        _timeAtApex = _ctx.IsGrounded ? _action.Ground.GroundBounce.Arc.timeAtApex : _action.Air.GroundBounce.Arc.timeAtApex;
        _timeToFall = _ctx.IsGrounded ? _action.Ground.GroundBounce.Arc.timeToFall : _action.Air.GroundBounce.Arc.timeToFall;

        _apex = _ctx.IsGrounded ? _action.Ground.GroundBounce.Arc.apex : _action.Air.GroundBounce.Arc.apex;
        _range = _ctx.IsGrounded ? _action.Ground.GroundBounce.Arc.range : _action.Air.GroundBounce.Arc.range;

        _hitStop = _ctx.IsGrounded ? _action.Ground.GroundBounce.Stun.hitStop : _action.Air.GroundBounce.Stun.hitStop;

        _stun = _ctx.IsGrounded ? _action.Ground.GroundBounce.Stun.stun : _action.Air.GroundBounce.Stun.stun;

        _ctx.HealthManager.UpdateHealth(_ctx.IsGrounded ? _collisionData.action.Ground.GroundBounce.damage : _collisionData.action.Air.GroundBounce.damage);

        //_groundOffset = _ctx.transform.position.y - 0.5f; // y = 0.5f is the centre position of the character.
        _direction = Mathf.Sign(_collisionData.hitbox.Transform.right.x);
    
        _time = (_timeToApex + _timeAtApex + _timeToFall) * Time.fixedDeltaTime;
        // _drag = -2 * _action.GroundBounceVelocity.x / Mathf.Pow(_time, 2);
        // _drag *= _direction;
        _drag = 0f;

        // _velocity.x = 2 * _action.GroundBounceVelocity.x / _time; // Initial horizontal velocity;
        _velocity.x = _range / _time;
        _velocity.x *= _direction;
        
        // Zone 1
        float time1 = _timeToApex * Time.fixedDeltaTime;
        _gravity1 = - 2 * _apex / Mathf.Pow(time1, 2); // g = 2h/t^2
        _velocity.y = 2 * _apex / time1; // Initial vertical velocity. V0y = 2h/t

        // Zone 2
        float time2 = _timeToFall * Time.fixedDeltaTime; 
        // _gravity2 = -2 * (_action.GroundBounceVelocity.y + _groundOffset) / (time2 * time2); // Free Fall h = (1/2)gt^
        _gravity2 = -2 * _apex / (time2 * time2); // Free Fall h = (1/2)gt^2
        
        // Apply Calculated Variables
        _ctx.Drag = _drag;
        _ctx.Gravity = _gravity1;
        _ctx.CurrentMovement = _velocity;
        _ctx.Velocity = _velocity;

        if (_timeToApex == 0 || _timeToFall == 0) return;

        ActionDefault action = _ctx.ActionDictionary["Knockup"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;
        AnimationClip colClip = action.boxAnimation;

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = colClip;

        _animationSpeed = AdjustAnimationTime(clip, _stun); 

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
        // _ctx.Gravity = 0f;
        //_ctx.Drag = 0f;
        // _ctx.CurrentFrame = 0;
        // _ctx.CurrentMovement = Vector2.zero;
        // _ctx.Velocity = _ctx.CurrentMovement;
        // _ctx.FighterController.targetVelocity = _ctx.Velocity;
        Debug.Log("Fighter Ground Bounce State - Exit State");
    }

    public override void FixedUpdateState()
    {
        //Debug.Log("FighterKnockupState(FixedUpdateState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Root State: " + _ctx.CurrentRootState + " SubState: " + _ctx.CurrentSubState);
       _ctx.Drag = _drag;
        _ctx.Gravity = _currentFrame < _timeToApex + _hitStop ? _gravity1 : _gravity2;
        //Debug.Log("FighterKnockupState(FixedUpdateState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Drag: " + _ctx.Drag + " Gravity: " + _ctx.Gravity + " Current Movement: " + _ctx.CurrentMovement + " Velocity: " + _ctx.Velocity);
        //Debug.Log("Fighter Knockup State - Frame: " + _currentFrame + " Velocity Applied: " + (_ctx.CurrentMovement + new Vector2(_ctx.Drag, _ctx.Gravity) * Time.fixedDeltaTime));
    
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
