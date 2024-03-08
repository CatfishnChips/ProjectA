using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Continuous Attack Action", menuName = "ScriptableObject/Action/ContinuousAttack")]
public class ActionContinuousAttack : ActionFighterAttack
{  
    [Header("Custom Variables")]
    [SerializeField] private bool m_startCondition;
    [SerializeField] private bool m_activeCondition;
    [SerializeField] private bool m_recoveryCondition;

    private bool _pauseTime = false;
    private bool _timePaused = false;

    public bool StartCondition { get{return m_startCondition;} }
    public bool ActiveCondition { get{return m_activeCondition;} }
    public bool RecoveryCondition { get{return m_recoveryCondition;} }

    public override void EnterStateFunction(FighterStateMachine ctx, FighterAttackState state)
    {
        base.EnterStateFunction(ctx, state);
        _pauseTime = false;
        _timePaused = false;
    }

    public override void FixedUpdateFunction()
    {
        if (_currentFrame <= StartFrames){
            if(_firstFrameStartup){
                _ctx.Animator.SetFloat("SpeedVar", AnimSpeedS);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedS);
                _ctx.Animator.Play("AttackStart");
                _ctx.ColBoxAnimator.Play("AttackStart");
                _firstFrameStartup = false;
            }
        }
        else if (_currentFrame > StartFrames && _currentFrame <= StartFrames + ActiveFrames){
            if(_firstFrameActive){
                _ctx.Animator.SetFloat("SpeedVar", AnimSpeedA);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedA);
                _ctx.Animator.Play("AttackActive");
                _firstFrameActive = false;
            }

            // REWORK HERE!
            if (!_timePaused){
                _pauseTime = _ctx.HoldBInput.Read();
                _timePaused = !_pauseTime;
            }
        }
        else if(_currentFrame > StartFrames + ActiveFrames && 
        _currentFrame <= StartFrames + ActiveFrames + RecoveryFrames){
            if(_firstFrameRecovery){
                _ctx.Animator.SetFloat("SpeedVar", AnimSpeedR);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedR);
                _ctx.Animator.Play("AttackRecover");
                _firstFrameRecovery = false;
            }
        }

        if (!_pauseTime)
        _currentFrame++;
    }

    public override void ExitStateFunction()
    {
        base.ExitStateFunction();
    }
}