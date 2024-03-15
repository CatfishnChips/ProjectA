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

    public override void SwitchActionStateFunction(){
        if (_currentFrame <= StartFrames){
            _actionState = ActionStates.Start;
        }
        else if (_currentFrame > StartFrames && _currentFrame <= StartFrames + ActiveFrames){
            if (m_isDodged){
                _actionState = ActionStates.Recovery;
            }
            else _actionState = ActionStates.Active;
        }
        else if (_currentFrame > StartFrames + ActiveFrames && 
        _currentFrame <= StartFrames + ActiveFrames + RecoveryFrames){
            if (m_isDodged){
                _actionState = ActionStates.Recovery;
            }
            else _actionState = ActionStates.None;
        } 
    }

    public override void FixedUpdateFunction(){
        switch(_actionState)
        {
            case ActionStates.Start:
                if(_firstFrameStartup){
                    _ctx.Animator.SetFloat("SpeedVar", AnimSpeedS);
                    _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedS);
                    _ctx.Animator.Play("AttackStart");
                    _ctx.ColBoxAnimator.Play("AttackStart");
                    _firstFrameStartup = false;
                }
            break;

            case ActionStates.Active:
                if(_firstFrameActive){
                    _ctx.Animator.SetFloat("SpeedVar", AnimSpeedA);
                    _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedA);
                    _ctx.Animator.Play("AttackActive");
                    _firstFrameActive = false;
                    _ctx.IsInvulnerable = true;
                }

                if (_ctx.IsHurt) {
                    _ctx.IsHurt = false;
                    m_isDodged = true;
                    _ctx_0.SetFocus(true);
                    m_focus = true;
                }
            break;

            case ActionStates.Recovery:
                if(_firstFrameRecovery){
                    _ctx.Animator.SetFloat("SpeedVar", AnimSpeedR);
                    _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedR);
                    _ctx.Animator.Play("AttackRecover");
                    _firstFrameRecovery = false;
                    _currentFrame = StartFrames + ActiveFrames;
                }
            break;
        }
       
        // Invoke events.
        foreach(FrameEvent e in Events){
            if (_currentFrame == e.Frame){
                e.Event(_ctx, _ctx.CurrentState as FighterAttackState);
            }
        }

        if (_ctx.IsHit) _ctx.IsHit = false;
        _currentFrame++;
    }

    public override void ExitStateFunction(){
        _ctx.IsInvulnerable = false;
    }
}
