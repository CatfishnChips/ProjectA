using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spirit Attack Action", menuName = "ScriptableObject/Action/Attack/SpiritAttack")]
public class ActionSpiritAttack : ActionAttack
{
    private SpiritController m_spiritCtx;

    protected float m_calculatedKnockup;
    protected float m_calculatedKnockback;

    public float CalculatedKnockup {get{return m_calculatedKnockup;} set{m_calculatedKnockup = value;}}
    public float CalculatedKnockback {get{return m_calculatedKnockback;} set{m_calculatedKnockback = value;}}

    public override float Knockup {get => m_calculatedKnockup;}
    public override float Knockback {get => m_calculatedKnockback;}

    protected virtual List<FrameEvent_Spirit> Events_Spirit {get {return new List<FrameEvent_Spirit>();}}

    public virtual void EnterState(SpiritController ctx){
        m_spiritCtx = ctx;
        _firstFrameStartup = true;
        _firstFrameActive = true;
        _firstFrameRecovery = true;
        _firstTimePause = true;
        _pause = false;
        _pauseFrames = 0;
    }

    public virtual void SwitchActionStateFunction(){
        if (m_spiritCtx.CurrentFrame <= m_spiritCtx.Action.StartFrames){
            m_spiritCtx.ActionState = ActionStates.Start;
        }
        else if (m_spiritCtx.CurrentFrame > m_spiritCtx.Action.StartFrames && m_spiritCtx.CurrentFrame <= m_spiritCtx.Action.StartFrames + m_spiritCtx.Action.ActiveFrames){
            m_spiritCtx.ActionState = ActionStates.Active;
        }
        else if (m_spiritCtx.CurrentFrame > m_spiritCtx.Action.StartFrames + m_spiritCtx.Action.ActiveFrames && 
        m_spiritCtx.CurrentFrame <= m_spiritCtx.Action.StartFrames + m_spiritCtx.Action.ActiveFrames + m_spiritCtx.Action.RecoveryFrames){
            m_spiritCtx.ActionState = ActionStates.Recovery;
        }
        else m_spiritCtx.ActionState = ActionStates.None;
    }

    public override void FixedUpdateState(){
        switch(m_spiritCtx.ActionState)
        {
            case ActionStates.Start:
                if(_firstFrameStartup){
                    //ctx.Animator.SetFloat("SpeedVar", ctx.Action.AnimSpeedS);
                    m_spiritCtx.ColBoxAnimator.SetFloat("SpeedVar", m_spiritCtx.Action.AnimSpeedS);
                    //ctx.Animator.Play("AttackStart");
                    m_spiritCtx.ColBoxAnimator.PlayInFixedTime("AttackStart");
                    _firstFrameStartup = false;
                }
            break;

            case ActionStates.Active:
                if(_firstFrameActive){
                    //ctx.Animator.SetFloat("SpeedVar", ctx.Action.AnimSpeedA);
                    m_spiritCtx.ColBoxAnimator.SetFloat("SpeedVar", m_spiritCtx.Action.AnimSpeedA);
                    //ctx.Animator.PlayInFixedTime("AttackActive");
                    m_spiritCtx.ColBoxAnimator.PlayInFixedTime("AttackActive");
                    _firstFrameActive = false;
                }
            break;

            case ActionStates.Recovery:
                if(_firstFrameRecovery){
                    //ctx.Animator.SetFloat("SpeedVar", ctx.Action.AnimSpeedR);
                    m_spiritCtx.ColBoxAnimator.SetFloat("SpeedVar", m_spiritCtx.Action.AnimSpeedR);
                    //ctx.Animator.PlayInFixedTime("AttackRecover");
                    m_spiritCtx.ColBoxAnimator.PlayInFixedTime("AttackRecover");
                    _firstFrameRecovery = false;
                }
            break;
        }
       
        // Invoke events.
        foreach(FrameEvent_Spirit e in Events_Spirit){
            if (m_spiritCtx.CurrentFrame == e.Frame){
                e.Event(m_spiritCtx);
            }
        }

        if (m_spiritCtx.IsHit) {
            m_spiritCtx.IsHit = false;

            if (_firstTimePause){
                _firstTimePause = false;
                _pause = true;
                _pauseFrames = m_hitStop;
                
                //ctx.Animator.SetFloat("SpeedVar", 0);
                m_spiritCtx.ColBoxAnimator.SetFloat("SpeedVar", 0);
            }
        } 


        if (_pause){
            if (_pauseFrames <= 0){
                _pause = false;
                //ctx.Animator.SetFloat("SpeedVar", ctx.Action.AnimSpeedA);
                m_spiritCtx.ColBoxAnimator.SetFloat("SpeedVar", m_spiritCtx.Action.AnimSpeedA);
            }
            _pauseFrames--;
        } 
        else
        m_spiritCtx.CurrentFrame++;
    }

    public virtual void ExitStateFunction(SpiritController ctx){
        ctx.IsActive = false;
    }

    public override void EnterState(){}

    public override void UpdateState(){}

    public override void ExitState(){}

    public override void CheckSwitchState(){}

    public override void InitializeSubState(){}
}
