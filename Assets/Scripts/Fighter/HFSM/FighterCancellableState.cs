using UnityEngine;

public abstract class FighterCancelleableState : FighterBaseState 
{
    [Tooltip("On which frame the action will be canceled if a chain input is listened.")]
    [SerializeField] private int _cancelFrame;
    [Tooltip("How many frames will the action wait before starting to listen for chain input.")]
    [SerializeField] private int _inputIgnoreFrames;

    private InputGestures _chainActionGesture;
    private bool _listeningForChainInput;
    private bool _isChainSuccessful;

    public override void CheckSwitchState()
    {
        if(_chainActionGesture != InputGestures.None){
            _isChainSuccessful = true;
            SwitchState(_factory.GetChainState(_chainActionGesture));
            return;
        }
    }

    public override void EnterState()
    {
        _chainActionGesture = InputGestures.None;
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
        }
        _chainActionGesture = InputGestures.None;
        _listeningForChainInput = true;
        _isChainSuccessful = false;
    }

    public void CancelCheck(){
        if(CurrentFrame <= _cancelFrame && CurrentFrame > _inputIgnoreFrames && _ctx.AttackInput.Read() && _listeningForChainInput){
            InputGestures chainGesture = _ctx.AttackInput.ReadContent();
            if(_ctx.ActionManager.CheckIfChain(chainGesture)){
                _chainActionGesture = chainGesture;
            }else{
                _listeningForChainInput = false;
            }
        }
    }


}