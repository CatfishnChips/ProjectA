using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterKnockupState : FighterBaseState
{
    protected CollisionData _collisionData;
    protected ActionAttack _action;
    protected Vector2 _velocity;
    protected float _animationSpeed;
    protected float _colliderAnimationSpeed;
    protected bool _isFirstTime = true;
    protected float _groundOffset; // Character's starting distance from the ground (this assumes the ground level is y = 0).
    protected float _gravity1, _gravity2;
    protected float _drag1, _drag2;
    protected float _time1, _time2;
    protected float _distancePerTime;
    protected float _direction;

    protected float _drag;
    protected float _time;

    protected int _timeToApex;
    protected int _timeAtApex;
    protected int _timeToFall;
    protected float _apex;
    protected float _range;
    protected int _hitStop;

    protected bool _air;
    protected bool _isGrounded;
   

    public FighterKnockupState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override bool CheckSwitchState()
    {
        if (_action.HitFlags.HasFlag(HitFlags.BOUNCE_WALL)){
            if(_ctx.FighterController.IsTouchingWall){
                SwitchState(_factory.GetSubState(FighterSubStates.WallBounce));
                return true;
            }
        } 

        if (_action.HitFlags.HasFlag(HitFlags.SPLAT_WALL)){
            if(_ctx.FighterController.IsTouchingWall){
                SwitchState(_factory.GetSubState(FighterSubStates.WallSplat));
                return true;
            }
        } 

        if (_action.HitFlags.HasFlag(HitFlags.BOUNCE_GROUND)){
            if (_air){
                if(_ctx.IsGrounded){
                    SwitchState(_factory.GetSubState(FighterSubStates.GroundBounce));
                    return true;
                }
            }
        } 

        if (_currentFrame >= _timeToApex + _timeAtApex + _timeToFall + _hitStop){
            if (_action.HitFlags.HasFlag(HitFlags.BOUNCE_GROUND)){
                if(_ctx.IsGrounded){
                    SwitchState(_factory.GetSubState(FighterSubStates.GroundBounce));
                    return true;
                }   
            }   

            if (_action.HitFlags.HasFlag(HitFlags.KNOCK_DOWN)){
                if(_ctx.IsGrounded){
                    SwitchState(_factory.GetSubState(FighterSubStates.Knockdown));
                    return true;
                }
            }
           
            if(IdleStateSwitchCheck()) return true; 
            
            SwitchState(_factory.GetSubState(FighterSubStates.Idle));
            return true;
            
        }
        else return false;
    }

    protected virtual void SetInternalVariables(){
        _currentFrame = 0;
        _ctx.CurrentFrame =_currentFrame;
        _isFirstTime = true;
        _collisionData = _ctx.HurtCollisionData;
        _action = _collisionData.action;
        _ctx.IsHurt = false;
        _velocity = Vector2.zero;
        _air = false;
    }

    protected virtual void SetExternalVariables(){
        _timeToApex = _ctx.IsGrounded ? _action.Ground.Trajectory.Arc.timeToApex : _action.Air.Trajectory.Arc.timeToApex;
        _timeAtApex = _ctx.IsGrounded ? _action.Ground.Trajectory.Arc.timeAtApex : _action.Air.Trajectory.Arc.timeAtApex;
        _timeToFall = _ctx.IsGrounded ? _action.Ground.Trajectory.Arc.timeToFall : _action.Air.Trajectory.Arc.timeToFall;

        _apex = _ctx.IsGrounded ? _action.Ground.Trajectory.Arc.apex : _action.Air.Trajectory.Arc.apex;
        _range = _ctx.IsGrounded ? _action.Ground.Trajectory.Arc.range : _action.Air.Trajectory.Arc.range;

        _hitStop = _ctx.IsGrounded ? _action.Ground.Stun.hitStop : _action.Air.Stun.hitStop;
    }

    public override void EnterState()
    {
        //Debug.Log("FighterKnockupState(EnterState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Root State: " + _ctx.CurrentRootState + " SubState: " + _ctx.CurrentSubState);
        SetInternalVariables();

        SetExternalVariables();

        _ctx.HealthManager.UpdateHealth(_collisionData.action.Damage);
        
        // _groundOffset is currently not used.
        //_groundOffset = _ctx.transform.position.y - 0.5f; // y = 0.5f is the centre position of the character.

        _direction = Mathf.Sign(_collisionData.hitbox.Transform.right.x);
    
        // _time = (_action.KnockupStun.x + _action.KnockupStun.y) * Time.fixedDeltaTime;
        // _drag = -2 * _action.Knockback / Mathf.Pow(_time, 2);
        // _drag *= _direction;

        _time = (_timeToApex + _timeAtApex + _timeToFall) * Time.fixedDeltaTime;
        _drag = 0;

        // _velocity.x = 2 * _action.Knockback / _time; // Initial horizontal velocity;
        // _velocity.x *= _direction;

        _velocity.x = _range / _time; // Initial horizontal velocity;
        _velocity.x *= _direction;
        
        // Zone 1
        // float time1 = _action.KnockupStun.x * Time.fixedDeltaTime;
        // _gravity1 = - 2 * _action.Knockup / Mathf.Pow(time1, 2); // g = 2h/t^2
        // _velocity.y = 2 * _action.Knockup / time1; // Initial vertical velocity. V0y = 2h/t

        float time1 = _timeToApex * Time.fixedDeltaTime;
        _gravity1 = - 2 * _apex / Mathf.Pow(time1, 2); // g = 2h/t^2
        _velocity.y = 2 * _apex / time1; // Initial vertical velocity. V0y = 2h/t

        // Zone 2
        // float time2 = _action.KnockupStun.y * Time.fixedDeltaTime; 
        // _gravity2 = -2 * (_action.Knockup + _groundOffset) / (time2 * time2); // Free Fall h = (1/2)gt^2

        float time2 = _timeToFall * Time.fixedDeltaTime; 
        //_gravity2 = -2 * (_apex + _groundOffset) / (time2 * time2); // Free Fall h = (1/2)gt^
        _gravity2 = -2 * _apex / (time2 * time2); // Free Fall h = (1/2)gt^2
        
        // Apply Calculated Variables
        _ctx.Drag = _drag;
        _ctx.Gravity = _gravity1;
        
        if (_timeToApex == 0 || _timeToFall == 0) return;

        ActionDefault action = _ctx.ActionDictionary["Knockup"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;
        AnimationClip colClip = action.boxAnimation;

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = colClip;

        _animationSpeed = AdjustAnimationTime(clip, _timeToApex + _timeAtApex + _timeToFall); 

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
        //_ctx.Gravity = 0f;
        _ctx.Drag = 0f;
        _ctx.CurrentFrame = 0;
        //_ctx.CurrentMovement = Vector2.zero;
        //_ctx.Velocity = _ctx.CurrentMovement;
        //_ctx.FighterController.targetVelocity = _ctx.Velocity;
        Debug.Log("Fighter Knockup State - Exit State");
    }

    public override void FixedUpdateState()
    {   
        if (!_air)
            if (!_ctx.IsGrounded)
                _air = true;

        //Debug.Log("FighterKnockupState(FixedUpdateState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Root State: " + _ctx.CurrentRootState + " SubState: " + _ctx.CurrentSubState);
        if (_currentFrame >= _hitStop){

            if (_isFirstTime){
                _ctx.Animator.SetFloat("SpeedVar", _animationSpeed);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", _animationSpeed);
                _isFirstTime = false;
                
                // Apply Calculated Initial Velocity
                _ctx.CurrentMovement = _velocity;
            }
            _ctx.Drag = _drag;
            _ctx.Gravity = _currentFrame < _timeToApex + _hitStop ? _gravity1 : _gravity2;
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
