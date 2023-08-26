using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Parry Action", menuName = "ScriptableObject/Action/Class0/Parry")]
public class ActionParry : ActionAttack_Class0
{
    // Recovery is only played if an attack is successfully dodged in Active period.
    // Attack is made (hitboxes are activated) in the Recovery animation.
    // If an is not dodged when the Active period ends, the move ends without entering the Recovery state.
    // When an attack is dodged, immediately enter the Recovery state.
    protected bool m_isDodged = false;

    public override void EnterStateFunction(FighterStateMachine ctx, FighterAttackState state){
        base.EnterStateFunction(ctx, state);
        m_isDodged = false;
    }

    public override void SwitchActionStateFunction(FighterStateMachine ctx, FighterAttackState state){
        if (state._currentFrame <= state.Action.StartFrames){
            state._actionState = ActionStates.Start;
        }
        else if (state._currentFrame > state.Action.StartFrames && state._currentFrame <= state.Action.StartFrames + state.Action.ActiveFrames){
            if (m_isDodged){
                state._actionState = ActionStates.Recovery;
            }
            else state._actionState = ActionStates.Active;
        }
        else if (state._currentFrame > state.Action.StartFrames + state.Action.ActiveFrames && 
        state._currentFrame <= state.Action.StartFrames + state.Action.ActiveFrames + state.Action.RecoveryFrames){
            if (m_isDodged){
                state._actionState = ActionStates.Recovery;
            }
            else state._actionState = ActionStates.None;
        } 
    }

    public override void FixedUpdateFunction(FighterStateMachine ctx, FighterAttackState state){
        switch(state._actionState)
        {
            case ActionStates.Start:
                if(_firstFrameStartup){
                    ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedS);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedS);
                    ctx.Animator.Play("AttackStart");
                    ctx.ColBoxAnimator.Play("AttackStart");
                    _firstFrameStartup = false;
                }
            break;

            case ActionStates.Active:
                if(_firstFrameActive){
                    ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedA);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedA);
                    ctx.Animator.Play("AttackActive");
                    _firstFrameActive = false;
                    ctx.IsInvulnerable = true;
                }

                if (ctx.IsHurt) {
                    ctx.IsHurt = false;
                    m_isDodged = true;
                    ((FighterStateMachine_Class0)ctx).SetFocus(true);
                    m_focus = true;
                }
            break;

            case ActionStates.Recovery:
                if(_firstFrameRecovery){
                    ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedR);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedR);
                    ctx.Animator.Play("AttackRecover");
                    _firstFrameRecovery = false;
                    state._currentFrame = state.Action.StartFrames + state.Action.ActiveFrames;
                }
            break;
        }
       
        // Invoke events.
        foreach(FrameEvent e in Events){
            if (state._currentFrame == e.Frame){
                e.Event(ctx, state);
            }
        }

        if (ctx.IsHit) ctx.IsHit = false;
        state._currentFrame++;
    }

    public override void ExitStateFunction(FighterStateMachine ctx, FighterAttackState state){
        ctx.IsInvulnerable = false;
    }
}
