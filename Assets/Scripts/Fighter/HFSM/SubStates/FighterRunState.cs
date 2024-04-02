using UnityEngine;

public class FighterRunState : FighterBaseState
{
    public FighterRunState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override bool CheckSwitchState()
    {
        if (!(_ctx.Velocity.x < -0.5f || _ctx.Velocity.x > 0.5f)){ // if fighter is not running
            if (_ctx.Velocity.x >= -0.5f && _ctx.Velocity.x <= 0.5f){ // if we just slowed sown
                SwitchState(_factory.GetSubState(FighterSubStates.Walk));
                return true;
            }
            else{ // if we directly stopped
                if(IdleStateSwitchCheck()) return true; 
                
                SwitchState(_factory.GetSubState(FighterSubStates.Idle));
                return true;
            }
        }
        else return false;
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
