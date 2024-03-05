using UnityEngine;
using UnityEngine.Events;

public class FighterAttackState : FighterBaseState
{
    private ActionFighterAttack _action;
    public int _currentFrame = 0;
    public ActionStates _actionState = default;
    private bool _hadHit = false;

    public bool performedComboMove = false;
    public bool listeningForChainInput = true;
    public InputGestures chainInputGesture = InputGestures.None;

    private bool isReading;

    public bool HadHit { get {return _hadHit;} set{ _hadHit = value;} }
    public ActionFighterAttack Action { get => _action; }

    public FighterAttackState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory):
    base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        Debug.Log("Current Frame: " + _currentFrame + "Cancel Frame: " + _action.CancelFrames);
        if (_actionState == ActionStates.None){

            if(_ctx.Player == Player.P1) EventManager.Instance.FighterAttackEnded?.Invoke();
            else if(_ctx.Player == Player.P2) EventManager.Instance.P2FighterAttackEnded?.Invoke();

            _ctx.ActionState = default;
            _ctx.ValidAttackInputInterval = false;

            if (_ctx.AttackInput.Read()){
                SwitchState(_factory.GetSubState(FighterSubStates.Attack));
            }
            else{
                SwitchState(_factory.GetSubState(FighterSubStates.Idle));
            }
        }
        else if (_currentFrame >= _action.CancelFrames && performedComboMove){
            _ctx.chainInputGesture = chainInputGesture;
            SwitchState(_factory.GetSubState(FighterSubStates.Attack));
        }
    }

    public override void EnterState()
    {   
        //string attackName = _ctx.AttackName;
        if(_ctx.chainInputGesture != InputGestures.None) _action = _ctx.ActionManager.GetAction(_ctx.chainInputGesture) as ActionFighterAttack;
        else _action = _ctx.ActionManager.GetAction(_ctx.AttackInput.ReadContent()) as ActionFighterAttack;
        _currentFrame = 0;
        _ctx.OnAttackStart?.Invoke();
        _hadHit = false;
        _ctx.CurrentFrame =_currentFrame;
        _actionState = ActionStates.Start;
        _ctx.chainInputGesture = InputGestures.None;

        _action.EnterStateFunction(_ctx, this);
        
        _ctx.ClipOverrides["AttackStart"] = _action.MeshAnimationS;
        _ctx.ClipOverrides["AttackActive"] = _action.MeshAnimationA;
        _ctx.ClipOverrides["AttackRecover"] = _action.MeshAnimationR;

        _ctx.ColBoxClipOverrides["Box_AttackStart"] = _action.BoxAnimationS;
        _ctx.ColBoxClipOverrides["Box_AttackActive"] = _action.BoxAnimationA;
        _ctx.ColBoxClipOverrides["Box_AttackRecover"] = _action.BoxAnimationR;

        _ctx.AnimOverrideCont.ApplyOverrides(_ctx.ClipOverrides);
        _ctx.ColBoxOverrideCont.ApplyOverrides(_ctx.ColBoxClipOverrides);

        _ctx.HitResponder.UpdateData(_action);

        if (_ctx.Player == Player.P2) EventManager.Instance.FighterAttackStarted?.Invoke(_action.name);
        else EventManager.Instance.P2FighterAttackStarted?.Invoke(_action.name);
    }

    public override void ExitState()
    {
        Debug.Log("Attack ended at frame: " + _currentFrame);
        _ctx.CurrentFrame = 0;
        _ctx.IsGravityApplied = true;
        _ctx.ActionState = default;
        _ctx.OnAttackEnd?.Invoke();
        _action.ExitStateFunction(_ctx, this);
        performedComboMove = false;
        listeningForChainInput = true;

        if(chainInputGesture != _ctx.chainInputGesture) _ctx.ActionManager.Reset();
        chainInputGesture = InputGestures.None;
        
        _ctx.Drag = 0f;
        _ctx.Gravity = 0f;
        _ctx.CurrentMovement = Vector2.zero;
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState(){
        _action.FixedUpdateFunction(_ctx, this);
        // Debug.Log("Frame L: " + _action.FrameLenght + " Current F: " + _currentFrame + " Buffer: " + _ctx.GetInputBuffer + " Bool: " + _ctx.ValidAttackInputInterval);
        //_ctx.ValidAttackInputInterval = _action.FrameLenght - _currentFrame < _ctx.GetInputBuffer;

        _ctx.CurrentMovement = _action.applyRootMotion ? _ctx.RootMotion : _ctx.CurrentMovement;

        _action.SwitchActionStateFunction(_ctx, this);
        _ctx.ActionState = _actionState;

        _ctx.CurrentFrame =_currentFrame;
        CheckSwitchState();
    }
}
