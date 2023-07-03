using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FighterStunnedState : FighterBaseState
{   
    private CollisionData _collisionData;
    private ActionAttack _action;
    private float _currentFrame = 0;

    public FighterStunnedState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _isRootState = true;
    }

    public override void CheckSwitchState()
    {
        if (_ctx.IsHurt){
            SwitchState(_factory.Stunned());
        }
        
        if (_currentFrame >= _action.KnockbackStun + _action.KnockupStun.x + _action.KnockupStun.y + _action.KnockdownStun + _action.Freeze){   
            FighterBaseState state;         

            if (_ctx.IsGrounded){
                state = _factory.Grounded();
            }
            else{
                state = _factory.Airborne();
            }
            SwitchState(state);
        }
    }

    public override void EnterState()
    {
        _currentFrame = 0;
        _collisionData = _ctx.HurtCollisionData;
        _action = _collisionData.action;

        InitializeSubState();

        _ctx.IsHurt = false;

        _ctx.Velocity = _ctx.CurrentMovement;
        _ctx.Rigidbody2D.velocity = _ctx.Velocity;

        _ctx.HealthManager.UpdateHealth(_collisionData);
    }

    public override void ExitState()
    {
        _ctx.Gravity = Physics2D.gravity.y;
    }

    public override void FixedUpdateState()
    {   
        if (_currentFrame > _action.Freeze){  
            _ctx.Rigidbody2D.velocity = _ctx.Velocity;
            //Debug.Log("Velocity Applied: " + _ctx.Velocity);
        }
        
        _currentFrame++;
        CheckSwitchState();

        // If the fighter is hurt again before the stun duration expires, duration is refreshed.
        // if (_ctx.IsHurt)
        // {
        //     _ctx.IsHurt = false;
        //     _collisionData = _ctx.CollisionData;
        //     _currentFrame = 0;
        // }
    }

    public override void InitializeSubState()
    { 
        FighterBaseState state;
        if (_action.KnockupStun.x + _action.KnockupStun.y > 0){
            state = _factory.Knockup();
        }
        else if (_action.KnockdownStun > 0){
            state = _factory.Knockdown();
        }
        else if (_action.KnockbackStun > 0){
            state = _factory.Knockback();
        }
        else{
            state = _factory.Idle();
        }
        SetSubState(state);
        state.EnterState();
    }

    public override void UpdateState()
    {
    }
}
