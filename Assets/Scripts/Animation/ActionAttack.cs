using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "ScriptableObject/Action/Attack")]
public class ActionAttack : ActionBase
{
    public float damage;

    [Tooltip("In frames.")]
    public float stunDuration;

    [Header("Frame Data")]
    public int startUpFrames;
    public int activeFrames;
    public int recoveryFrames;

    [Header("Animations")]
    public AnimationClip meshAnimationS;
    public AnimationClip meshAnimationA;
    public AnimationClip meshAnimationR;

    public AnimationClip boxAnimationS;
    public AnimationClip boxAnimationA;
    public AnimationClip boxAnimationR;

    public float AnimSpeedS {get{return AdjustAnimationTime(meshAnimationS, startUpFrames);}}
    public float AnimSpeedA {get{return AdjustAnimationTime(meshAnimationA, activeFrames);}}
    public float AnimSpeedR {get{return AdjustAnimationTime(meshAnimationR, recoveryFrames);}}
}
