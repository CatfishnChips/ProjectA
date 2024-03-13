// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [CreateAssetMenu(fileName = "New Grab Action", menuName = "ScriptableObject/Action/Grab")]
// public class ActionGrab : ActionFighterAttack
// {   
//     [Header("Custom Variables")]
//     [SerializeField] private AnimationClip m_alternativeMeshAnimation;
//     [SerializeField] private AnimationClip m_alternativeColliderAnimation;
//     [SerializeField] private int m_frame;

//     public override void EnterStateFunction(FighterStateMachine ctx, FighterAttackState state)
//     {
//         base.EnterStateFunction(ctx, state);
//     }

//     // REWORK HERE, HAS THE OLD LOGIC
//     public override void FixedUpdateFunction()
//     {
//         if (_currentFrame <= StartFrames){
//             if(_firstFrameStartup){
//                 _ctx.Animator.SetFloat("SpeedVar", AnimSpeedS);
//                 _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedS);
//                 _ctx.Animator.Play("AttackStart");
//                 _ctx.ColBoxAnimator.Play("AttackStart");
//                 _firstFrameStartup = false;
//             }
//         }
//         else if (_currentFrame > StartFrames && _currentFrame <= StartFrames + ActiveFrames){
//             if(_firstFrameActive){
//                 _ctx.Animator.SetFloat("SpeedVar", AnimSpeedA);
//                 _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedA);
//                 _ctx.Animator.Play("AttackActive");
//                 _firstFrameActive = false;
//             }
//         }
//         else if(_currentFrame > StartFrames + ActiveFrames && 
//         _currentFrame <= StartFrames + ActiveFrames + RecoveryFrames){
//             if(_firstFrameRecovery){
//                 // If attack successfuly connects, change the animation.
//                 if (_ctx.IsHit){
//                     FighterStateMachine target = _ctx.HitCollisionData.hurtbox.Owner;
//                     _ctx.IsHit = false;
//                     // Do the further grab logic here.

//                     _ctx.AnimOverrideCont["DirectPunchR"] = m_alternativeMeshAnimation;
//                     _ctx.ColBoxOverrideCont["Uppercut_Recovery"] = m_alternativeColliderAnimation;
//                     _ctx.Animator.SetFloat("SpeedVar", m_frame);
//                     _ctx.ColBoxAnimator.SetFloat("SpeedVar", m_frame);
//                 }
//                 else{
//                     _ctx.Animator.SetFloat("SpeedVar", AnimSpeedR);
//                     _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedR);
//                 }

//                 _ctx.Animator.Play("AttackRecover");
//                 _firstFrameRecovery = false;
//             }
//         }
//         _currentFrame++;
//     }

//     public override void ExitStateFunction()
//     {
//         base.ExitStateFunction();
//     }
// }