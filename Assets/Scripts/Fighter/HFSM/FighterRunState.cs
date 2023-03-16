using UnityEngine;

public class FighterRunState : FighterBaseState
{
    public FighterRunState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
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
    }

    public override void ExitState()
    {
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }
}
