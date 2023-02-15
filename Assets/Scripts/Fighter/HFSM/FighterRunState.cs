using UnityEngine;

public class FighterRunState : FighterBaseState
{
    public FighterRunState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _stateName = "Run";
    }

    public override void CheckSwitchState()
    {
        if (!(_ctx.Velocity.x < -0.5f || _ctx.Velocity.x > 0.5f)){ // if fighter is not running
            if (_ctx.Velocity.x >= -0.5f && _ctx.Velocity.x <= 0.5f){ // if we just slowed sown
                SwitchState(_factory.Walk());
            }
            else{ // if we directly stopped
                SwitchState(_factory.Idle());
            }
        }
    }

    public override void EnterState()
    {
        _ctx.Animator.SetBool("Moving", true);
    }

    public override void ExitState()
    {
        _ctx.Animator.SetBool("Moving", false);
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
        _ctx.Animator.SetFloat("Blend", _ctx.Velocity.x);
        Debug.Log("RUNNING!");
    }
}
