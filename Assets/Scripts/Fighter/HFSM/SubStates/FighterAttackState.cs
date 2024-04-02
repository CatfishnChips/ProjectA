using UnityEngine;
using UnityEngine.Events;

public class FighterAttackState : FighterCancellableState
{
    private ActionFighterAttack _action;


    public FighterAttackState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory):
    base(currentContext, fighterStateFactory){
        _ctx.ChainActionGesture = InputGestures.None;
    }

    public override bool CheckSwitchState()
    {
        if(base.CheckSwitchState()) return true;
        if (_action.ActionState == ActionStates.None){
            
            if(IdleStateSwitchCheck()) return true; 
            
            SwitchState(_factory.GetSubState(FighterSubStates.Idle));
            return true;
            
        }
        else{
            return false;
        }
    }

    public override void EnterState()
    {   
        base.EnterState();

        _action = _cancellableAction as ActionFighterAttack;

        _ctx.ChainActionGesture = InputGestures.None;

        _action.EnterStateFunction(_ctx, this);

    }

    public override void ExitState()
    {
        base.ExitState();
        _action.ExitStateFunction();
    }

    public override void FixedUpdateState(){
        base.FixedUpdateState();
        CheckSwitchState(); // Execution order is important CheckSwitchState should be executed before FixedUpdate to prevent buggy transitions.
        _action.FixedUpdateFunction();
    }

    public override void InitializeSubState(){}

    public override void UpdateState(){}

}
