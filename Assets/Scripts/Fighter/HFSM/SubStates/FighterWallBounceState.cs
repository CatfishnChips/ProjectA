using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterWallBounceState : FighterKnockupState
{
    public FighterWallBounceState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    private Trajectory _trajectory;

    public override bool CheckSwitchState()
    {
        bool condition = false;

        switch(_trajectory){
            case Trajectory.ARC:
                condition =_currentFrame >= _time;
            break;

            case Trajectory.LINE:
                condition = _ctx.IsGrounded;
            break;
        }
        
        if (condition){ 
            if (_ctx.WasGrounded){
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
            else {
                // Knockup always transitions to Knockdown state.
                if (((HitFlags)_action.Air.HitFlags).HasFlag(HitFlags.KNOCK_DOWN)){
                    SwitchState(_factory.GetSubState(FighterSubStates.Knockdown));
                    return true;
                }
                else{
                    if(IdleStateSwitchCheck()) return true; 
                    
                    SwitchState(_factory.GetSubState(FighterSubStates.Idle));
                    return true;
                }
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
        _velocity = Vector2.zero;

        _trajectory = _ctx.WasGrounded ? _action.Ground.WallBounce.Bounce.Trajectory.trajectory : _action.Air.WallBounce.Bounce.Trajectory.trajectory;

        _hitStop = _ctx.WasGrounded ? _action.Ground.WallBounce.Bounce.Stun.hitStop : _action.Air.WallBounce.Bounce.Stun.hitStop;

        _ctx.HealthManager.UpdateHealth(_ctx.WasGrounded ? _collisionData.action.Ground.WallBounce.Bounce.damage : _collisionData.action.Air.WallBounce.Bounce.damage);

        switch(_ctx.FighterController.Side){
            case FighterController.ScreenSide.Left:
                _direction = 1;
            break;

            case FighterController.ScreenSide.Right:
                _direction = -1;
            break;
        }

        switch(_trajectory){
            case Trajectory.ARC:
                // Set Required Variables

                //_groundOffset = _ctx.transform.position.y - 0.5f; // y = 0.5f is the centre position of the character.

                _timeToApex = _ctx.WasGrounded ? _action.Ground.WallBounce.Bounce.Trajectory.Arc.timeToApex : _action.Air.WallBounce.Bounce.Trajectory.Arc.timeToApex;
                _timeAtApex = _ctx.WasGrounded ? _action.Ground.WallBounce.Bounce.Trajectory.Arc.timeAtApex : _action.Air.WallBounce.Bounce.Trajectory.Arc.timeAtApex;
                _timeToFall = _ctx.WasGrounded ? _action.Ground.WallBounce.Bounce.Trajectory.Arc.timeToFall : _action.Air.WallBounce.Bounce.Trajectory.Arc.timeToFall;

                _apex = _ctx.WasGrounded ? _action.Ground.WallBounce.Bounce.Trajectory.Arc.apex : _action.Air.WallBounce.Bounce.Trajectory.Arc.apex;
                _apex = Mathf.Abs(_apex);
                _range = _ctx.WasGrounded ? _action.Ground.WallBounce.Bounce.Trajectory.Arc.range : _action.Air.WallBounce.Bounce.Trajectory.Arc.range;

                // Projectile Motion Calculations
                _time = _timeToApex + _timeAtApex + _timeToFall + _hitStop;

                float totalTime = (_timeToApex + _timeAtApex + _timeToFall) * Time.fixedDeltaTime;
                _drag = 0;

                _velocity.x = _range / totalTime; // Initial horizontal velocity;
                _velocity.x *= _direction;

                // Zone 1
                float time1 = _timeToApex * Time.fixedDeltaTime;
                _gravity1 = - 2 * _apex / Mathf.Pow(time1, 2); // g = 2h/t^2
                _velocity.y = 2 * _apex / time1; // Initial vertical velocity. V0y = 2h/t

                // Zone 2
                float time2 = _timeToFall * Time.fixedDeltaTime; 
                _gravity2 = -2 * _apex / (time2 * time2); // Free Fall h = (1/2)gt^2

                // Apply Calculated Variables
                _ctx.Drag = _drag;
                _ctx.Gravity = _gravity1;
            break;

            case Trajectory.LINE: 
                _velocity = _ctx.WasGrounded ? _action.Ground.WallBounce.Bounce.Trajectory.Line.velocity : _action.Air.WallBounce.Bounce.Trajectory.Line.velocity;

                _drag = 0;

                // Apply Calculated Variables
                _ctx.Drag = _drag;
                _ctx.Gravity = _ctx.GravityConstant;
            break;
        }
        
        //if (_timeToApex == 0 || _timeToFall == 0) return;

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

    public override void FixedUpdateState()
    {   
        if (!_air)
            if (!_ctx.IsGrounded)
                _air = true;

        if (_currentFrame >= _hitStop){

            if (_isFirstTime){
                _ctx.Animator.SetFloat("SpeedVar", _animationSpeed);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", _animationSpeed);
                _isFirstTime = false;
                
                // Apply Calculated Initial Velocity
                _ctx.CurrentMovement = _velocity;
                _ctx.Drag = _drag;
                _ctx.Gravity = _ctx.GravityConstant;
            }

            if (_trajectory == Trajectory.ARC)    
                _ctx.Gravity = _currentFrame < _timeToApex + _hitStop ? _gravity1 : _gravity2;  
        }
        
        CheckSwitchState();
        _currentFrame++;
        _ctx.CurrentFrame =_currentFrame;
    }
}