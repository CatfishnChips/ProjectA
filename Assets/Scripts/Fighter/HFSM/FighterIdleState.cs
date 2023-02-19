using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterIdleState : FighterBaseState
{
    public FighterIdleState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _stateName = "Idle";
    }

    public override void CheckSwitchState()
    {
        if (_ctx.Velocity.x != 0){            
            SwitchState(_factory.Walk());
        }

    }

    public override void EnterState()
    {
        string prev_anim_state = _ctx.Animator.GetCurrentAnimatorStateInfo(0).ToString();
        _ctx.Animator.SetTrigger("ToIdle");
        string current_anim_state = _ctx.Animator.GetCurrentAnimatorStateInfo(0).ToString();
        if(current_anim_state == prev_anim_state){
            _ctx.Animator.Play("Idle");
        }

    }

    public override void ExitState()
    {
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }
}
