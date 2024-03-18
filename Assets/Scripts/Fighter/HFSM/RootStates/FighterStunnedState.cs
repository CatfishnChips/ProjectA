using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FighterStunnedState : FighterBaseState
{   
    private CollisionData _collisionData;
    private ActionAttack _action;
    private bool _isFirstTime = true;
    private StunnedState _state = default;
    
    public StunnedState State {get{return _state;} set{_state = value;}}

    public FighterStunnedState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _isRootState = true;
    }

    public override bool CheckSwitchState()
    {
        if (_ctx.IsHurt && !_ctx.IsInvulnerable){
            SwitchState(_factory.GetRootState(FighterRootStates.Stunned));
            return true;
        }
        
        // ">" is used instead of ">=" due to Root States' Fixed Update running before the Sub States' Fixed Update.
        // Ex. Calculations in the Sub State's 49th frame are applied to Root State in the 50th frame.
        // This only applies to situations which the fighter's velocity is controled.

        // if (_currentFrame > _action.KnockbackStun + _action.KnockupStun.x + _action.KnockupStun.y + _action.KnockdownStun + _action.HitStop){   
        //     FighterBaseState state;         

        //     if (_ctx.IsGrounded){
        //         state = _factory.GetRootState(FighterRootStates.Grounded);
        //     }
        //     else{
        //         state = _factory.GetRootState(FighterRootStates.Airborne);
        //     }
        //     SwitchState(state);
        // }

        if (_ctx.CurrentSubState == FighterStates.Idle){

            if (_ctx.IsGrounded){
                SwitchState(_factory.GetRootState(FighterRootStates.Grounded));
                return true;
            }
            else{
                SwitchState(_factory.GetRootState(FighterRootStates.Airborne));
                return true;
            }


            // if (_isFirstTime) {
            //     _isFirstTime = false;
            // }      
            // else{
                
            // }
        }
        return false;
    }

    public override void EnterState()
    {
        //Debug.Log("FighterStunnedState(EnterState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Root State: " + _ctx.CurrentRootState + " SubState: " + _ctx.CurrentSubState);
        _currentFrame = 0;
        _collisionData = _ctx.HurtCollisionData;
        _action = _collisionData.action;
        _isFirstTime = true;

        _ctx.HealthManager.UpdateHealth(_ctx.CanBlock ? -_collisionData.action.ChipDamage : -_collisionData.action.Damage);
        InitializeSubState();

        _ctx.IsHurt = false;

        _ctx.Velocity = _ctx.CurrentMovement;
        //_ctx.Rigidbody2D.velocity = _ctx.Velocity;
        _ctx.FighterController.targetVelocity = _ctx.Velocity;
    }

    public override void ExitState()
    {
        _ctx.Gravity = 0f;
        _ctx.Drag = 0f;
        _ctx.CurrentFrame = 0;
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Velocity = Vector2.zero;
        // _ctx.Rigidbody2D.velocity = Vector2.zero;
        _ctx.FighterController.targetVelocity = Vector2.zero;

        switch(_ctx.Player){
            case Player.P1:
            EventManager.Instance.RecoveredFromStun_P1?.Invoke();
            break;

            case Player.P2:
            EventManager.Instance.RecoveredFromStun_P2?.Invoke();
            break;
        }
    }

    public override void FixedUpdateState()
    {   
        //Debug.Log("FighterStunnedState(FixedUpdateState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Root State: " + _ctx.CurrentRootState + " SubState: " + _ctx.CurrentSubState);
        //Debug.Log("FighterStunnedState(FixedUpdateState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Drag: " + _ctx.Drag + " Gravity: " + _ctx.Gravity + " Current Movement: " + _ctx.CurrentMovement + " Velocity: " + _ctx.Velocity);
        if (_currentFrame > _action.HitStop){  
            _ctx.CurrentMovement += new Vector2(_ctx.Drag, _ctx.Gravity) * Time.fixedDeltaTime;
            _ctx.Velocity = _ctx.CurrentMovement;
            //_ctx.Rigidbody2D.velocity = _ctx.Velocity;
            _ctx.FighterController.targetVelocity = _ctx.Velocity;
        }
        CheckSwitchState();
        _currentFrame++;
    }

    public override void InitializeSubState()
    { 
        //Debug.Log("FighterStunnedState(InitializeSubState)");
        FighterBaseState state;
        //Debug.Log("Script: Stunned State " + "Time: " + Time.timeSinceLevelLoad + " Target Can Block?: " + _ctx.CanBlock);
        if(!_action.IgnoreBlock && _ctx.CanBlock){
            state = _factory.GetSubState(FighterSubStates.Block);
        }
        else if (_action.KnockupStun.x + _action.KnockupStun.y > 0){
            if (Mathf.Sign(_action.Knockup) > 0) 
            state = _factory.GetSubState(FighterSubStates.Knockup);
            else
            state = _factory.GetSubState(FighterSubStates.SlamDunk);
        }
        else if (_action.KnockdownStun > 0){
            state = _factory.GetSubState(FighterSubStates.Knockdown);
        }
        else if (_action.KnockbackStun > 0){
            if(_ctx.IsGrounded)
            state = _factory.GetSubState(FighterSubStates.Knockback);
            else
            state = _factory.GetSubState(FighterSubStates.FreeFall);
        }
        else{
            state = _factory.GetSubState(FighterSubStates.Idle);
        }
        SetSubState(state);
        state.EnterState();
    }

    public override void UpdateState()
    {
    }
}
