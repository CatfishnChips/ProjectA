using UnityEngine;
using UnityEngine.Events;

public class FighterAttackState : FighterBaseState
{
    public ActionAttack action; 
    public int currentFrame = 0;

    public FighterAttackState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory):
    base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (currentFrame >= (action.StartFrames + action.ActiveFrames + action.RecoveryFrames)){
            EventManager.Instance.FighterAttackEnded?.Invoke();
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
        currentFrame = 0;

        if(!_ctx.ComboListener.isActive){
            _ctx.ComboListener.isActive = true;
        }
        if (_ctx.IsGrounded){
            action = _ctx.AttackMoveDict[attackName];
        }
        else{

        }

        action = _ctx.ComboListener.AttackOverride(action);

        action.EnterStateFunction(_ctx, this);
        
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
