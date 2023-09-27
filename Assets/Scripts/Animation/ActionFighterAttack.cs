using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fighter Attack Action", menuName = "ScriptableObject/Action/Attack/FighterAttack")]
public class ActionFighterAttack : ActionAttack
{
    private List<FrameEvent> _frameEvents = new List<FrameEvent>();
    protected virtual List<FrameEvent> Events {get {return _frameEvents;}}

    public ActionFighterAttack(){
        // Add FrameEvents here:
        //Events.Add(new FrameEvent(21, (FighterStateMachine ctx, FighterAttackState state) => Debug.Log("ActionFighterAttack Event Callback - Frame: " + state._currentFrame)));
    }

    public virtual void EnterStateFunction(FighterStateMachine ctx, FighterAttackState state){
        _firstFrameStartup = true;
        _firstFrameActive = true;
        _firstFrameRecovery = true;
        _firstTimePause = true;
        _pause = false;
        _pauseFrames = 0;
        ctx.IsGravityApplied = m_gravity;
    }

    public virtual void SwitchActionStateFunction(FighterStateMachine ctx, FighterAttackState state){
        if (state._currentFrame <= state.Action.StartFrames){
            state._actionState = ActionStates.Start;
        }
        else if (state._currentFrame > state.Action.StartFrames && state._currentFrame <= state.Action.StartFrames + state.Action.ActiveFrames){
            state._actionState = ActionStates.Active;
        }
        else if (state._currentFrame > state.Action.StartFrames + state.Action.ActiveFrames && 
        state._currentFrame <= state.Action.StartFrames + state.Action.ActiveFrames + state.Action.RecoveryFrames){
            state._actionState = ActionStates.Recovery;
        }
        else state._actionState = ActionStates.None;

        //Debug.Log("ActionFighterAttack(SwitchActionStateFunction) - Frame: " + state._currentFrame + " State: " + state._actionState);
    }

    public virtual void FixedUpdateFunction(FighterStateMachine ctx, FighterAttackState state){
        switch(state._actionState)
        {
            case ActionStates.Start:
                if(_firstFrameStartup){
                    ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedS);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedS);
                    ctx.Animator.PlayInFixedTime("AttackStart");
                    ctx.ColBoxAnimator.PlayInFixedTime("AttackStart");
                    _firstFrameStartup = false;
                }
            break;

            case ActionStates.Active:
                if(_firstFrameActive){
                    ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedA);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedA);
                    ctx.Animator.PlayInFixedTime("AttackActive");
                    ctx.ColBoxAnimator.PlayInFixedTime("AttackActive");
                    _firstFrameActive = false;
                }
            break;

            case ActionStates.Recovery:
                if(_firstFrameRecovery){
                    ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedR);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedR);
                    ctx.Animator.PlayInFixedTime("AttackRecover");
                    ctx.ColBoxAnimator.PlayInFixedTime("AttackRecover");
                    _firstFrameRecovery = false;
                }
            break;
        }
       
        // Invoke events.
        foreach(FrameEvent e in Events){
            if (state._currentFrame == e.Frame){
                e.Event(ctx, state);
            }
        }

        if (ctx.IsHit) {
            ctx.IsHit = false;
            state.HadHit = true;

            if (_firstTimePause){
                _firstTimePause = false;
                _pause = true;
                _pauseFrames = m_hitStop;
                
                ctx.Animator.SetFloat("SpeedVar", 0);
                ctx.ColBoxAnimator.SetFloat("SpeedVar", 0);
            }
        } 


        if (_pause){
            if (_pauseFrames <= 0){
                _pause = false;
                ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedA);
                ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedA);
            }
            _pauseFrames--;
        } 
        else
        state._currentFrame++;

        //Debug.Log("ActionFighterAttack(FixedUpdateFunction) - Frame: " + state._currentFrame + " State: " + state._actionState);
    }

    public virtual void ExitStateFunction(FighterStateMachine ctx, FighterAttackState state){
    }
}
