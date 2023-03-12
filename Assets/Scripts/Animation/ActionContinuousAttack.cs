using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Continuous Attack Action", menuName = "ScriptableObject/Action/ContinuousAttack")]
public class ActionContinuousAttack : ActionAttack
{  
    [SerializeField] private bool m_startCondition;
    [SerializeField] private bool m_activeCondition;
    [SerializeField] private bool m_recoveryCondition;

    private bool _pauseTime = false;
    private bool _timePaused = false;

    public bool StartCondition {get {return m_startCondition;}}
    public bool ActiveCondition {get {return m_activeCondition;}}
    public bool RecoveryCondition {get {return m_recoveryCondition;}}

    public override void EnterStateFunction(FighterStateMachine context, FighterAttackState state)
    {
        base.EnterStateFunction(context, state);
        _pauseTime = false;
        _timePaused = false;
    }

    public override void FixedUpdateFunction(FighterStateMachine context, FighterAttackState state)
    {
         if (state.currentFrame <= state.action.StartFrames){
            if(_firstFrameStartup){
                context.Animator.SetFloat("SpeedVar", state.action.AnimSpeedS);
                context.ColBoxAnimator.SetFloat("SpeedVar", state.action.AnimSpeedS);
                context.Animator.Play("AttackStart");
                context.ColBoxAnimator.Play("AttackStart");
                _firstFrameStartup = false;
            }
        }
        else if (state.currentFrame > state.action.StartFrames && state.currentFrame <= state.action.StartFrames + state.action.ActiveFrames){
            if(_firstFrameActive){
                context.Animator.SetFloat("SpeedVar", state.action.AnimSpeedA);
                context.ColBoxAnimator.SetFloat("SpeedVar", state.action.AnimSpeedA);
                _firstFrameActive = false;
            }

            if (!_timePaused){
                _pauseTime = context.IsHoldingTouchB;
                _timePaused = !_pauseTime;
                context.Animator.SetBool("LockTransition", _pauseTime);
            }
        }
        else if(state.currentFrame > state.action.StartFrames + state.action.ActiveFrames && 
        state.currentFrame <= state.action.StartFrames + state.action.ActiveFrames + state.action.RecoveryFrames){
            if(_firstFrameRecovery){
                context.Animator.SetFloat("SpeedVar", state.action.AnimSpeedR);
                context.ColBoxAnimator.SetFloat("SpeedVar", state.action.AnimSpeedR);
                _firstFrameRecovery = false;
            }
        }

        if (!_pauseTime)
        state.currentFrame++;
    }

    public override void ExitStateFunction(FighterStateMachine context, FighterAttackState state)
    {
        base.ExitStateFunction(context, state);
        context.Animator.SetBool("LockTransition", false);
    }
}