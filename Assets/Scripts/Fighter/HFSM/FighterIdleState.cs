using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterIdleState : FighterBaseState
{
    private ActionDefault _action;

    public FighterIdleState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _stateName = "Idle";
    }

    public override void CheckSwitchState()
    {
        if (_ctx.DeltaTarget >= 0.5f || _ctx.DeltaTarget <= -0.5f){            
            SwitchState(_factory.Walk());
        }

        if (_ctx.AttackPerformed){
            SwitchState(_factory.Attack());
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
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {

    }
}
