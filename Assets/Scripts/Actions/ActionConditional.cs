using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Default Action", menuName = "ScriptableObject/Action/Conditional")]
public class ActionConditional : ActionBase
{
    public List<AnimationData> Animations;
    public float AnimationSpeed(int index) { return AdjustAnimationTime(Animations[index].meshAnimation, Animations[0].frames); }

    [SerializeField] private int m_cancelFrame;
    [SerializeField] private int m_inputIgnoreFrames;

    public int CancelFrame { get => m_cancelFrame; }
    public int InputIgnoreFrames { get => m_inputIgnoreFrames; }
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
