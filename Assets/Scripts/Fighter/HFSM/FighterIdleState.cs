using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterIdleState : FighterBaseState
{
    private ActionDefault _action;

    public FighterIdleState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        // if (_ctx.IsHurt && _ctx.StaminaManager.CanBlock && _ctx.IsGrounded && _ctx.CurrentRootState == FighterStates.Grounded){
        //     //if (_ctx.HitCollisionData.action.IgnoreBlock) 
        //     SwitchState(_factory.Block());
        // }
        
        if (_ctx.AttackPerformed){
            SwitchState(_factory.Attack());
        }
        else if (_ctx.IsDodgePressed && _ctx.IsGrounded && _ctx.CurrentRootState == FighterStates.Grounded){
            SwitchState(_factory.Dodge());
        }
        else if (_ctx.IsDashPressed && _ctx.IsGrounded && _ctx.CurrentRootState == FighterStates.Grounded){
            SwitchState(_factory.Dash());
        }
        else if (_ctx.MovementInput != 0){            
            SwitchState(_factory.Walk());
        }  
    }

    public override void EnterState()
    {
        if (_ctx.CurrentRootState == FighterStates.Grounded) 
        {
            _action = _ctx.ActionDictionary["GroundedIdle"] as ActionDefault;
        }
        else if (_ctx.CurrentRootState == FighterStates.Airborne)
        {
            _action = _ctx.ActionDictionary["AirborneIdle"] as ActionDefault;
        }

        _ctx.AnimOverrideCont["Idle 1"] = _action.meshAnimation;
        _ctx.ColBoxOverrideCont["Idle"] = _action.boxAnimation;

        _ctx.Animator.Play("Idle");
        _ctx.ColBoxAnimator.Play("Idle");
    }

    public override void ExitState()
    {
    }

    public override void FixedUpdateState()
    {
        CheckSwitchState();
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {

    }
}
