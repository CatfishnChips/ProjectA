using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Default Action", menuName = "ScriptableObject/Action/Default")]
public class ActionDefault : ActionBase
{
    [Header("Frame Data")]
    public int frames;

    [Header("Animations")]
    public AnimationClip meshAnimation;
    public AnimationClip boxAnimation;

    public float AnimationSpeed {get{return AdjustAnimationTime(meshAnimation, frames);}}
}
