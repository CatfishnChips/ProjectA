using UnityEngine;

public abstract class ActionDefault : FighterBaseState
{
    [Header("Frame Data")]
    public int frames;

    [Header("Animations")]
    public AnimationClip meshAnimation;
    public AnimationClip boxAnimation;

    public float AnimationSpeed {get{return AdjustAnimationTime(meshAnimation, frames);}}
}
