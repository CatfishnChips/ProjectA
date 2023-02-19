using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "FighterAttack")]
public class AttackMove : ScriptableObject
{
    public new string name;

    public float damage;
    [Tooltip("In frames")]
    public float stunDuration;

    [Header("Attack frame data (in frames)")]
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

    private float _animSpeedS;
    private float _animSpeedA;
    private float _animSpeedR;

    public float AnimSpeedS{get{return _animSpeedS;}}
    public float AnimSpeedA{get{return _animSpeedA;}}
    public float AnimSpeedR{get{return _animSpeedR;}}

    public void AdjustAnimationTimes(){
        float animLength;
        float startupTime;

        animLength = meshAnimationS.length;
        startupTime = startUpFrames * Time.fixedDeltaTime;
        _animSpeedS = animLength / startupTime;

        animLength = meshAnimationA.length;
        startupTime = startUpFrames * Time.fixedDeltaTime;
        _animSpeedA = animLength / startupTime;

        animLength = meshAnimationR.length;
        startupTime = startUpFrames * Time.fixedDeltaTime;
        _animSpeedR = animLength / startupTime;
    }
}
