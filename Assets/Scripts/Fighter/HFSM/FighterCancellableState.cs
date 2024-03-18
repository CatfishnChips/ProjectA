using UnityEngine;

public abstract class FighterCancellableState : FighterBaseState
{
    protected CancellableAction _cancellableAction;
    private bool _listeningForChainInput;
    private bool _isChainSuccessful;

    protected FighterCancellableState(IStateMachineRunner ctx, FighterStateFactory factory) : base(ctx, factory){}

    public override bool CheckSwitchState()
    {
        if(_ctx.ChainActionGesture != InputGestures.None && _currentFrame >= _cancellableAction.CancelFrame){
            _isChainSuccessful = true;
            CancellableAction action = _ctx.ActionManager.GetAction(_ctx.ChainActionGesture);
            SwitchState(_factory.GetSubState((FighterSubStates)action.stateName));
            return true;
        }
        return false;
    }

    public override void EnterState()
    {
        if(_ctx.ChainActionGesture == InputGestures.None) _ctx.ChainActionGesture = _ctx.ActionInput.ReadContent();
        //Debug.Log(_ctx.ChainActionGesture);
        _cancellableAction = _ctx.ActionManager.GetAction(_ctx.ChainActionGesture);
        _ctx.ActionManager.ItarateForward(_ctx.ChainActionGesture);
        _ctx.ChainActionGesture = InputGestures.None;
        _listeningForChainInput = true;
        _isChainSuccessful = false;
    }

    public override void FixedUpdateState()
    {
        CancelCheck();
    }

    public override void ExitState()
    {
        if(!_isChainSuccessful){
            _ctx.ActionManager.Reset();
            _cancellableAction = null;
            _ctx.ChainActionGesture = InputGestures.None;
        }
        _listeningForChainInput = true;
        _isChainSuccessful = false;
    }

    public void CancelCheck(){
        if(_currentFrame <= _cancellableAction.CancelFrame && _currentFrame > _cancellableAction.InputIgnoreFrames && _ctx.ActionInput.Read() && _listeningForChainInput){
            InputGestures chainGesture = _ctx.ActionInput.ReadContent();
            if(_ctx.ActionManager.CheckIfChain(chainGesture)){
                _ctx.ChainActionGesture = chainGesture;
            }else{
                _listeningForChainInput = false;
            }
        }
    }
}
