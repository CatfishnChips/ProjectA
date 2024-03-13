using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Default Action", menuName = "ScriptableObject/Action/Conditional")]
public abstract class ActionConditional : FighterBaseState
{
    public List<AnimationData> animationList;
    public Dictionary<string, AnimationData> animationDict;
    public float AnimationSpeed(int index) { return AdjustAnimationTime(animationList[index].meshAnimation, animationList[0].frames); }

    public override void Initialize(IStateMachineRunner ctx, FighterStateFactory factory)
    {
        base.Initialize(ctx, factory);
        animationDict = new Dictionary<string, AnimationData>();
        foreach (AnimationData animationData in animationList){
            animationDict.Add(animationData.name, animationData);
        }
    }

    [SerializeField] private int m_cancelFrame;
    [SerializeField] private int m_inputIgnoreFrames;

    public int CancelFrame { get => m_cancelFrame; }
    public int InputIgnoreFrames { get => m_inputIgnoreFrames; }
}

[Serializable]
public struct AnimationData
{
    [Header("Animation Name")]
    public string name;

    [Header("Frame Data")]
    public int frames;

    [Header("Animations")]
    public AnimationClip meshAnimation;
    public AnimationClip boxAnimation;
}
