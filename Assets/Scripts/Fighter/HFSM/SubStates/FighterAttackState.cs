using UnityEngine;
using UnityEngine.Events;

public class FighterAttackState : FighterBaseState
{
    private ActionFighterAttack _action;
    private InputGestures _chainActionGesture;
    private bool _listeningForChainInput;

    public InputGestures ChainActionGesture { get => _chainActionGesture; set => _chainActionGesture = value; }

    public FighterAttackState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory):
    base(currentContext, fighterStateFactory){
        _chainActionGesture = InputGestures.None;
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

        if(_chainActionGesture != InputGestures.None) _action = _ctx.ActionManager.GetAction(_chainActionGesture) as ActionFighterAttack;
        else {
            _action = _ctx.ActionManager.GetAction(_ctx.AttackInput.ReadContent()) as ActionFighterAttack;
        }

        if(_action == null) { // Safety check if all the precautions to reset Action Manager somehow failed.
            _ctx.ActionManager.Reset();
            _action = _ctx.ActionManager.GetAction(_ctx.AttackInput.ReadContent()) as ActionFighterAttack;
        }

        _chainActionGesture = InputGestures.None;

        _action.EnterStateFunction(_ctx, this);

    }

    public override void ExitState()
    {
        _action.ExitStateFunction();
    }

    public override void FixedUpdateState(){
        if(_action.CurrentFrame <= _action.CancelFrames && _action.CurrentFrame > _action.InputIgnoreFrames && _ctx.AttackInput.Read() && _listeningForChainInput){
            if(_ctx.ActionManager.CheckIfChain(_ctx.AttackInput.ReadContent())){
                _chainActionGesture = _ctx.AttackInput.ReadContent();
            }else{
                _listeningForChainInput = false;
            }
        }

        _action.FixedUpdateFunction();
        CheckSwitchState();
    }


    private void SetFields(){
        _action = null;
        _listeningForChainInput = true;
    }

    public override void InitializeSubState(){}

    public override void UpdateState(){}

}
