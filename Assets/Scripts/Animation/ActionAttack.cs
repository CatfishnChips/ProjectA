using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "ScriptableObject/Action/Attack")]
public class ActionAttack : ActionBase
{
    public int m_damage;

    [Header("Hitbox Properties")]
    [Tooltip("Which type of hitbox is prioritized for hit detection.")]
    [SerializeField] private int m_priority;
    [Tooltip("Dictates how many times a move can hit. Set to 1 for single hit moves.")]
    [SerializeField] private int m_part = 1;

    [Header("Stun Properties")]
    [Tooltip("Stun inflicted upon hit (in frames).")]
    [SerializeField] private int m_hitStun;
    [SerializeField] private int m_blockStun;

    [Header("Knockback Properties")]
    [SerializeField] private float m_knockup;
    [SerializeField] private float m_knockback;

    [Header("SFX Properties")]
    [SerializeField] private AudioClip m_sound;
    [SerializeField] private float m_soundLevel;

    //[Header("VFX Properties")]

    [Header("Frame Data")]
    [SerializeField] private int m_startFrames;
    [SerializeField] private int m_activeFrames;
    [SerializeField] private int m_recoveryFrames;

    [Header("Animation Clips")]
    [SerializeField] private AnimationClip m_meshS;
    [SerializeField] private AnimationClip m_meshA;
    [SerializeField] private AnimationClip m_meshR;
    [SerializeField] private AnimationClip m_boxS;
    [SerializeField] private AnimationClip m_boxA;
    [SerializeField] private AnimationClip m_boxR;


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

    public int Damage {get => m_damage;}
    public int Priority {get => m_priority;}
    public int Part {get => m_part;}
    public int HitStun {get => m_hitStun;}
    public int BlockStun {get => m_blockStun;}
    public float Knockup {get => m_knockup;}
    public float Knockback {get => m_knockback;}
    public AudioClip Sound {get => m_sound;}
    public float SoundLevel {get => m_soundLevel;}

    public int StartFrames {get => m_startFrames;}
    public int ActiveFrames {get => m_activeFrames;}
    public int RecoveryFrames {get => m_recoveryFrames;}

    public AnimationClip MeshAnimationS {get => m_meshS;}
    public AnimationClip MeshAnimationA {get => m_meshA;}
    public AnimationClip MeshAnimationR {get => m_meshR;}
    public AnimationClip BoxAnimationS {get => m_boxS;}
    public AnimationClip BoxhAnimationA {get => m_boxA;}
    public AnimationClip BoxhAnimationR {get => m_boxR;}
}
