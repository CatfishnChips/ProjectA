using UnityEngine;

public class FighterGroundedState : FighterBaseState
{
    public FighterGroundedState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _stateName = "Grounded";
    }

    public override void CheckSwitchState()
    {
        if (_ctx.IsJumpPressed){
            SwitchState(_factory.Jump());
        }
    }

    public override void EnterState()
    {
        Debug.Log("ENTERED GROUNDED STATE.");
    }

    public override void ExitState()
    {
        Debug.Log("EXITED GROUNDED STATE");
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
