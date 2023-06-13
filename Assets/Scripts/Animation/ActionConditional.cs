using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Default Action", menuName = "ScriptableObject/Action/Conditional")]
public class ActionConditional : ActionBase
{
    public List<AnimationData> Animations;
    public float AnimationSpeed(int index) { return AdjustAnimationTime(Animations[index].meshAnimation, Animations[0].frames); }
}

[Serializable]
public struct AnimationData
{
    [Header("Frame Data")]
    public int frames;

    [Header("Animations")]
    public AnimationClip meshAnimation;
    public AnimationClip boxAnimation;
}
