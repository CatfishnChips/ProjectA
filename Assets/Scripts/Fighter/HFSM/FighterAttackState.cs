using UnityEngine;
using UnityEngine.Events;

public class FighterAttackState : FighterBaseState
{
    public ActionAttack action; 
    public int currentFrame = 0;

    public FighterAttackState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory):
    base(currentContext, fighterStateFactory){
        _stateName = "Attack";
    }

    public override void CheckSwitchState()
    {
        if (currentFrame >= (action.StartFrames + action.ActiveFrames + action.RecoveryFrames)){
            EventManager.Instance.FighterAttackEnded?.Invoke();
            SwitchState(_factory.Idle());
        }

        if (_ctx.IsHurt){
            EventManager.Instance.FighterAttackInterrupted?.Invoke();
            SwitchState(_factory.Stunned());
        }
    }

    public override void EnterState()
    {
        if(!_ctx.ComboListener.isActive){
            _ctx.ComboListener.isActive = true;
        }

        currentFrame = 0;
        _ctx.IsInputLocked = true;

        string attackName = _ctx.AttackName;
        if (_ctx.IsGrounded){
            action = _ctx.AttackMoveDict[attackName];
        }
        else{

        }

        action = _ctx.ComboListener.AttackOverride(action);

        //Debug.Log(_attackMove.name);

        action.EnterStateFunction(_ctx, this);

        _ctx.IsGravityApplied = false; // Get this value from the attack action!
        
        _ctx.ClipOverrides["DirectPunchA"] = action.MeshAnimationA;
        _ctx.ClipOverrides["DirectPunchR"] = action.MeshAnimationR;
        _ctx.ClipOverrides["DirectPunchS"] = action.MeshAnimationS;

        _ctx.ColBoxClipOverrides["Uppercut_Startup"] = action.BoxAnimationS;
        _ctx.ColBoxClipOverrides["Uppercut_Active"] = action.BoxAnimationA;
        _ctx.ColBoxClipOverrides["Uppercut_Recovery"] = action.BoxAnimationR;

        _ctx.AnimOverrideCont.ApplyOverrides(_ctx.ClipOverrides);
        _ctx.ColBoxOverrideCont.ApplyOverrides(_ctx.ColBoxClipOverrides);

        _ctx.HitResponder.UpdateData(action);

        EventManager.Instance.FighterAttackStarted?.Invoke(action.name);
    }

    public override void ExitState()
    {
        _ctx.AttackPerformed = false;
        _ctx.IsInputLocked = false;
        _ctx.IsGravityApplied = true;
        action.ExitStateFunction(_ctx, this);
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState(){
        
        action.FixedUpdateFunction(_ctx, this);
        CheckSwitchState();
    }
}
