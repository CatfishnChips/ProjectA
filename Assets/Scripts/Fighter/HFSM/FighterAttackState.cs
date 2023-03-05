using UnityEngine;

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
            SwitchState(_factory.Idle());
        }

        if (_ctx.IsHurt){
            SwitchState(_factory.Stunned());
        }
    }

    public override void EnterState()
    {
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
        

        _ctx.ClipOverrides["DirectPunchA"] = _attackMove.meshAnimationA;
        _ctx.ClipOverrides["DirectPunchR"] = _attackMove.meshAnimationR;
        _ctx.ClipOverrides["DirectPunchS"] = _attackMove.meshAnimationS;

        _ctx.ColBoxClipOverrides["Uppercut_Startup"] = _attackMove.boxAnimationS;
        _ctx.ColBoxClipOverrides["Uppercut_Active"] = _attackMove.boxAnimationA;
        _ctx.ColBoxClipOverrides["Uppercut_Recovery"] = _attackMove.boxAnimationR;

        _ctx.AnimOverrideCont.ApplyOverrides(_ctx.ClipOverrides);
        _ctx.ColBoxOverrideCont.ApplyOverrides(_ctx.ColBoxClipOverrides);

        _ctx.HitResponder.UpdateData(_attackMove);

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
        if (_currentFrame <= _attackMove.startUpFrames){
            if(_firstFrameStartup){
                _ctx.Animator.SetFloat("SpeedVar", _attackMove.AnimSpeedS);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", _attackMove.AnimSpeedS);
                _ctx.Animator.Play("AttackStart");
                _ctx.ColBoxAnimator.Play("AttackStart");
                _firstFrameStartup = false;
            }
        }
        else if (_currentFrame > _attackMove.startUpFrames && _currentFrame <= _attackMove.startUpFrames + _attackMove.activeFrames){
            if(_firstFrameActive){
                _ctx.Animator.SetFloat("SpeedVar", _attackMove.AnimSpeedA);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", _attackMove.AnimSpeedA);
                //_ctx.Animator.Play("AttackActive");
                _firstFrameActive = false;
            }
        }
        else if(_currentFrame > _attackMove.startUpFrames + _attackMove.activeFrames && 
        _currentFrame <= _attackMove.startUpFrames + _attackMove.activeFrames + _attackMove.recoveryFrames){
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
