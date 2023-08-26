using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Grab Action", menuName = "ScriptableObject/Action/Grab")]
public class ActionGrab : ActionAttack
{   
    [Header("Custom Variables")]
    [SerializeField] private AnimationClip m_alternativeMeshAnimation;
    [SerializeField] private AnimationClip m_alternativeColliderAnimation;
    [SerializeField] private int m_frame;

    public override void EnterStateFunction(FighterStateMachine ctx, FighterAttackState state)
    {
        base.EnterStateFunction(ctx, state);
    }

    // REWORK HERE, HAS THE OLD LOGIC
    public override void FixedUpdateFunction(FighterStateMachine ctx, FighterAttackState state)
    {
        if (state._currentFrame <= state.Action.StartFrames){
            if(_firstFrameStartup){
                ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedS);
                ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedS);
                ctx.Animator.Play("AttackStart");
                ctx.ColBoxAnimator.Play("AttackStart");
                _firstFrameStartup = false;
            }
        }
        else if (state._currentFrame > state.Action.StartFrames && state._currentFrame <= state.Action.StartFrames + state.Action.ActiveFrames){
            if(_firstFrameActive){
                ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedA);
                ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedA);
                ctx.Animator.Play("AttackActive");
                _firstFrameActive = false;
            }
        }
        else if(state._currentFrame > state.Action.StartFrames + state.Action.ActiveFrames && 
        state._currentFrame <= state.Action.StartFrames + state.Action.ActiveFrames + state.Action.RecoveryFrames){
            if(_firstFrameRecovery){
                // If attack successfuly connects, change the animation.
                if (ctx.IsHit){
                    FighterStateMachine target = ctx.HitCollisionData.hurtbox.Owner;
                    ctx.IsHit = false;
                    // Do the further grab logic here.

                    ctx.AnimOverrideCont["DirectPunchR"] = m_alternativeMeshAnimation;
                    ctx.ColBoxOverrideCont["Uppercut_Recovery"] = m_alternativeColliderAnimation;
                    ctx.Animator.SetFloat("SpeedVar", m_frame);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", m_frame);
                }
                else{
                    ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedR);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedR);
                }

                ctx.Animator.Play("AttackRecover");
                _firstFrameRecovery = false;
            }
        }
        state._currentFrame++;
    }

    public override void ExitStateFunction(FighterStateMachine ctx, FighterAttackState state)
    {
        base.ExitStateFunction(ctx, state);
    }
}