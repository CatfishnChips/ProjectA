using UnityEngine;
using UnityEngine.Events;

public class FighterAttackState : FighterBaseState
{
    private ActionFighterAttack _action;
    private InputGestures _chainActionGesture;
    private bool _listeningForChainInput;


    public FighterAttackState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory):
    base(currentContext, fighterStateFactory){
        _ctx.ChainActionGesture = InputGestures.None;
    }

    public override void CheckSwitchState()
    {
        FighterSubStates newState;
        newState = _action.SwitchState();
        if(newState != FighterSubStates.None) SwitchState(_factory.GetSubState(newState));
    }

    public override void EnterState()
    {   
        
        SetFields();

        if(_ctx.ChainActionGesture != InputGestures.None) _action = _ctx.ActionManager.GetAction(_ctx.ChainActionGesture) as ActionFighterAttack;
        else _action = _ctx.ActionManager.GetAction(_ctx.AttackInput.ReadContent()) as ActionFighterAttack;

        if(_action == null) { // Safety check if all the precautions to reset Action Manager somehow failed.
            _ctx.ActionManager.Reset();
            _action = _ctx.ActionManager.GetAction(_ctx.AttackInput.ReadContent()) as ActionFighterAttack;
        }

        _ctx.ChainActionGesture = InputGestures.None;

        _action.EnterStateFunction(_ctx, this);

    }

    public override void ExitState()
    {
        _action.ExitStateFunction();
    }

    public override void FixedUpdateState(){
        _action.CancelCheck();
        CheckSwitchState(); // Execution order is important CheckSwitchState should be executed before FixedUpdate to prevent buggy transitions.
        _action.FixedUpdateFunction();
    }


    private void SetFields(){
        _action = null;
        _listeningForChainInput = true;
    }

    public override void InitializeSubState(){}

    public override void UpdateState(){}

}
