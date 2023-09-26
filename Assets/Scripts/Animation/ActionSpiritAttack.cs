using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spirit Attack Action", menuName = "ScriptableObject/Action/Attack/SpiritAttack")]
public class ActionSpiritAttack : ActionAttack
{
    protected float m_calculatedKnockup;
    protected float m_calculatedKnockback;

    public float CalculatedKnockup {get{return m_calculatedKnockup;} set{m_calculatedKnockup = value;}}
    public float CalculatedKnockback {get{return m_calculatedKnockback;} set{m_calculatedKnockback = value;}}

    public override float Knockup {get => m_calculatedKnockup;}
    public override float Knockback {get => m_calculatedKnockback;}

    protected virtual List<FrameEvent_Spirit> Events_Spirit {get {return new List<FrameEvent_Spirit>();}}

    public virtual void EnterStateFunction(SpiritController ctx){
        _firstFrameStartup = true;
        _firstFrameActive = true;
        _firstFrameRecovery = true;
        _firstTimePause = true;
        _pause = false;
        _pauseFrames = 0;
    }

    public virtual void SwitchActionStateFunction(SpiritController ctx){
        if (ctx.CurrentFrame <= ctx.Action.StartFrames){
            ctx.ActionState = ActionStates.Start;
        }
        else if (ctx.CurrentFrame > ctx.Action.StartFrames && ctx.CurrentFrame <= ctx.Action.StartFrames + ctx.Action.ActiveFrames){
            ctx.ActionState = ActionStates.Active;
        }
        else if (ctx.CurrentFrame > ctx.Action.StartFrames + ctx.Action.ActiveFrames && 
        ctx.CurrentFrame <= ctx.Action.StartFrames + ctx.Action.ActiveFrames + ctx.Action.RecoveryFrames){
            ctx.ActionState = ActionStates.Recovery;
        }
        else ctx.ActionState = ActionStates.None;
    }

    public virtual void FixedUpdateFunction(SpiritController ctx){
        switch(ctx.ActionState)
        {
            case ActionStates.Start:
                if(_firstFrameStartup){
                    //ctx.Animator.SetFloat("SpeedVar", ctx.Action.AnimSpeedS);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", ctx.Action.AnimSpeedS);
                    //ctx.Animator.Play("AttackStart");
                    ctx.ColBoxAnimator.PlayInFixedTime("AttackStart");
                    _firstFrameStartup = false;
                }
            break;

            case ActionStates.Active:
                if(_firstFrameActive){
                    //ctx.Animator.SetFloat("SpeedVar", ctx.Action.AnimSpeedA);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", ctx.Action.AnimSpeedA);
                    //ctx.Animator.PlayInFixedTime("AttackActive");
                    ctx.ColBoxAnimator.PlayInFixedTime("AttackActive");
                    _firstFrameActive = false;
                }
            break;

            case ActionStates.Recovery:
                if(_firstFrameRecovery){
                    //ctx.Animator.SetFloat("SpeedVar", ctx.Action.AnimSpeedR);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", ctx.Action.AnimSpeedR);
                    //ctx.Animator.PlayInFixedTime("AttackRecover");
                    ctx.ColBoxAnimator.PlayInFixedTime("AttackRecover");
                    _firstFrameRecovery = false;
                }
            break;
        }
       
        // Invoke events.
        foreach(FrameEvent_Spirit e in Events_Spirit){
            if (ctx.CurrentFrame == e.Frame){
                e.Event(ctx);
            }
        }

        if (ctx.IsHit) {
            ctx.IsHit = false;

            if (_firstTimePause){
                _firstTimePause = false;
                _pause = true;
                _pauseFrames = m_hitStop;
                
                //ctx.Animator.SetFloat("SpeedVar", 0);
                ctx.ColBoxAnimator.SetFloat("SpeedVar", 0);
            }
        } 


        if (_pause){
            if (_pauseFrames <= 0){
                _pause = false;
                //ctx.Animator.SetFloat("SpeedVar", ctx.Action.AnimSpeedA);
                ctx.ColBoxAnimator.SetFloat("SpeedVar", ctx.Action.AnimSpeedA);
            }
            _pauseFrames--;
        } 
        else
        ctx.CurrentFrame++;
    }

    public virtual void ExitStateFunction(SpiritController ctx){
        ctx.IsActive = false;
    }
}
