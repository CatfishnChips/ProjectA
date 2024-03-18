using UnityEngine;

[CreateAssetMenu(fileName = "New Dash Action", menuName = "ScriptableObject/Action/Dash")]
public class ActionDash : CancellableAction
{
    [Tooltip("The length of dash In frames")]
    public int dashTime;

    [Header("Animations")]
    public AnimationClip meshAnimation;
    public AnimationClip boxAnimation;

    [SerializeField] public int direction;

    public float AnimationSpeed {get{return AdjustAnimationTime(meshAnimation, dashTime);}}
}

