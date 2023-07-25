using UnityEngine;
using UnityEngine.Events;

public class FighterAttackState : FighterBaseState
{
    public ActionAttack _action; 
    public int _currentFrame = 0;
    public ActionStates _actionState = default;

    public FighterAttackState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory):
    base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= (_action.StartFrames + _action.ActiveFrames + _action.RecoveryFrames)){

            if(_ctx.Player == Player.P1) EventManager.Instance.FighterAttackEnded?.Invoke();
            else if(_ctx.Player == Player.P2) EventManager.Instance.P2FighterAttackEnded?.Invoke();

            _ctx.ActionState = default;
            _ctx.ValidAttackInputInterval = false;

            if (_ctx.AttackPerformed){
                SwitchState(_factory.Attack());
            }
            else{
                SwitchState(_factory.Idle());
            }
        }

        // if (_ctx.IsHurt){
        //     EventManager.Instance.FighterAttackInterrupted?.Invoke();
        //     SwitchState(_factory.Stunned());
        // }
    }

    public override void EnterState()
    {   
        string attackName = _ctx.AttackName;
        _ctx.AttackPerformed = false;
        _currentFrame = 0;

        // Restart Combo Listener.
        if(!_ctx.ComboListener.isActive){
            _ctx.ComboListener.isActive = true;
        }
        
        // Determine Attack depending on context.
        if (_ctx.IsGrounded){
            _action = _ctx.AttackMoveDict[attackName];
        }
        else  _action = _ctx.AttackMoveDict[attackName];

        // if (_ctx.MovementInput == 1){
            
        // }
        // else if (_ctx.MovementInput == -1){

        // }
        // else if (_ctx.IsGrounded){
        //     _action = _ctx.AttackMoveDict[attackName];
        // }
        // else if (!_ctx.IsGrounded){

        // }

        _action = _ctx.ComboListener.AttackOverride(_action);

        _action.EnterStateFunction(_ctx, this);
        
        _ctx.ClipOverrides["DirectPunchA"] = _action.MeshAnimationA;
        _ctx.ClipOverrides["DirectPunchR"] = _action.MeshAnimationR;
        _ctx.ClipOverrides["DirectPunchS"] = _action.MeshAnimationS;

        _ctx.ColBoxClipOverrides["Uppercut_Startup"] = _action.BoxAnimationS;
        _ctx.ColBoxClipOverrides["Uppercut_Active"] = _action.BoxAnimationA;
        _ctx.ColBoxClipOverrides["Uppercut_Recovery"] = _action.BoxAnimationR;

        _ctx.AnimOverrideCont.ApplyOverrides(_ctx.ClipOverrides);
        _ctx.ColBoxOverrideCont.ApplyOverrides(_ctx.ColBoxClipOverrides);

        _ctx.HitResponder.UpdateData(_action);

        if (_ctx.Player == Player.P2) EventManager.Instance.FighterAttackStarted?.Invoke(_action.name);
        else EventManager.Instance.P2FighterAttackStarted?.Invoke(_action.name);
    }

    public override void ExitState()
    {
        _ctx.IsGravityApplied = true;
        _ctx.ActionState = default;
        _action.ExitStateFunction(_ctx, this);
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState(){
        _action.SwitchActionStateFunction(_ctx, this);
        _ctx.ActionState = _actionState;

        _action.FixedUpdateFunction(_ctx, this);
        // Debug.Log("Frame L: " + _action.FrameLenght + " Current F: " + _currentFrame + " Buffer: " + _ctx.GetInputBuffer + " Bool: " + _ctx.ValidAttackInputInterval);
        _ctx.ValidAttackInputInterval = _action.FrameLenght - _currentFrame < _ctx.GetInputBuffer;
        CheckSwitchState();
    }
}
