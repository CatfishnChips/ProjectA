using UnityEngine;
using UnityEngine.Events;

public class FighterAttackState : FighterBaseState
{
    private ActionAttack _attackMove; 
    private int _currentFrame = 0;

    private bool _firstFrameStartup = true;
    private bool _firstFrameActive = true;
    private bool _firstFrameRecovery = true;

    AnimationState startupAnim;
    AnimationState activeAnim;
    AnimationState recoveryAnim;

    public FighterAttackState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory):
    base(currentContext, fighterStateFactory){
        _stateName = "Attack";
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= (_attackMove.startUpFrames + _attackMove.activeFrames + _attackMove.recoveryFrames)){
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

        _currentFrame = 0;
        _ctx.AttackPerformed = false;
        _ctx.IsInputLocked = true;

        _firstFrameStartup = true;
        _firstFrameActive = true;
        _firstFrameRecovery = true;

        
        if (_ctx.IsGrounded){
            _attackMove = _ctx.AttackMoveDict[_ctx.AttackName];
        }
        else{

        }

        _ctx.IsGravityApplied = false; // Get this value from the attack action!
        

        _ctx.ClipOverrides["DirectPunchA"] = _attackMove.MeshAnimationA;
        _ctx.ClipOverrides["DirectPunchR"] = _attackMove.MeshAnimationR;
        _ctx.ClipOverrides["DirectPunchS"] = _attackMove.MeshAnimationS;

        _ctx.ColBoxClipOverrides["Uppercut_Startup"] = _attackMove.BoxAnimationS;
        _ctx.ColBoxClipOverrides["Uppercut_Active"] = _attackMove.BoxAnimationA;
        _ctx.ColBoxClipOverrides["Uppercut_Recovery"] = _attackMove.BoxAnimationR;

        _ctx.AnimOverrideCont.ApplyOverrides(_ctx.ClipOverrides);
        _ctx.ColBoxOverrideCont.ApplyOverrides(_ctx.ColBoxClipOverrides);

        _ctx.HitResponder.UpdateData(_attackMove);

        EventManager.Instance.FighterAttackStarted?.Invoke(_attackMove.name);

        //_attackMove.AdjustAnimationTimes();
    }

    public override void ExitState()
    {
        _ctx.IsInputLocked = false;
        _ctx.IsGravityApplied = true;
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState(){
        if (_currentFrame <= _attackMove.StartFrames){
            if(_firstFrameStartup){
                _ctx.Animator.SetFloat("SpeedVar", _attackMove.AnimSpeedS);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", _attackMove.AnimSpeedS);
                _ctx.Animator.Play("AttackStart");
                _ctx.ColBoxAnimator.Play("AttackStart");
                _firstFrameStartup = false;
            }
        }
        else if (_currentFrame > _attackMove.StartFrames && _currentFrame <= _attackMove.StartFrames + _attackMove.ActiveFrames){
            if(_firstFrameActive){
                _ctx.Animator.SetFloat("SpeedVar", _attackMove.AnimSpeedA);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", _attackMove.AnimSpeedA);
                //_ctx.Animator.Play("AttackActive");
                _firstFrameActive = false;
            }
        }
        else if(_currentFrame > _attackMove.StartFrames + _attackMove.ActiveFrames && 
        _currentFrame <= _attackMove.StartFrames + _attackMove.ActiveFrames + _attackMove.RecoveryFrames){
            if(_firstFrameRecovery){
                _ctx.Animator.SetFloat("SpeedVar", _attackMove.AnimSpeedR);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", _attackMove.AnimSpeedR);
                //_ctx.Animator.Play("AttackRecover");
                _firstFrameRecovery = false;
            }
        }
        CheckSwitchState();
        _currentFrame++;
    }
}
