using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterGrabbedState : FighterBaseState
{
    private ActionDefault _action;

    public FighterGrabbedState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {

        if (_ctx.AttackPerformed){
            SwitchState(_factory.Attack());
        }
        else if (_ctx.IsDodgePressed){
            SwitchState(_factory.Dodge());
        }
        else if (_ctx.IsDashPressed){
            SwitchState(_factory.Dash());
        }
        else if (_ctx.MovementInput != 0){            
            SwitchState(_factory.Walk());
        }  
    }

    public override void EnterState()
    { 
        if (_ctx.IsGrounded) 
        {
            _action = _ctx.ActionDictionary["GroundedIdle"] as ActionDefault;
        }
        else
        {
            _action = _ctx.ActionDictionary["AirborneIdle"] as ActionDefault;
        }

        _ctx.AnimOverrideCont["Idle 1"] = _action.meshAnimation;

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
