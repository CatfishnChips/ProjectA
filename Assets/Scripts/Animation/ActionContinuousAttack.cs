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

    public override void EnterStateFunction(FighterStateMachine ctx, FighterAttackState state)
    {
        base.EnterStateFunction(ctx, state);
        _pauseTime = false;
        _timePaused = false;
    }

    public override void FixedUpdateFunction(FighterStateMachine ctx, FighterAttackState state)
    {
         if (state.currentFrame <= state.action.StartFrames){
            if(_firstFrameStartup){
                ctx.Animator.SetFloat("SpeedVar", state.action.AnimSpeedS);
                ctx.ColBoxAnimator.SetFloat("SpeedVar", state.action.AnimSpeedS);
                ctx.Animator.Play("AttackStart");
                ctx.ColBoxAnimator.Play("AttackStart");
                _firstFrameStartup = false;
            }
        }
        else if (state.currentFrame > state.action.StartFrames && state.currentFrame <= state.action.StartFrames + state.action.ActiveFrames){
            if(_firstFrameActive){
                ctx.Animator.SetFloat("SpeedVar", state.action.AnimSpeedA);
                ctx.ColBoxAnimator.SetFloat("SpeedVar", state.action.AnimSpeedA);
                ctx.Animator.Play("AttackActive");
                _firstFrameActive = false;
            }

            // REWORK HERE!
            if (!_timePaused){
                _pauseTime = ctx.IsHoldingTouchB;
                _timePaused = !_pauseTime;
            }
        }
        else if(state.currentFrame > state.action.StartFrames + state.action.ActiveFrames && 
        state.currentFrame <= state.action.StartFrames + state.action.ActiveFrames + state.action.RecoveryFrames){
            if(_firstFrameRecovery){
                ctx.Animator.SetFloat("SpeedVar", state.action.AnimSpeedR);
                ctx.ColBoxAnimator.SetFloat("SpeedVar", state.action.AnimSpeedR);
                ctx.Animator.Play("AttackRecover");
                _firstFrameRecovery = false;
            }
        }

        if (!_pauseTime)
        state.currentFrame++;
    }

    public override void ExitStateFunction(FighterStateMachine ctx, FighterAttackState state)
    {
        base.ExitStateFunction(ctx, state);
    }
}