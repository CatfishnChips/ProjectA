using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "ScriptableObject/Action/Attack")]
public class ActionAttack : ActionBase
{   
    [SerializeField] private Tags m_tags;
    [SerializeField] private int m_damage;

    [Header("Hitbox Properties")]
    [Tooltip("Which type of hitbox is prioritized for hit detection.")] // Probably won't be used.
    [SerializeField] private int m_priority;
    [Tooltip("Dictates how many times a move can hit. Set to 1 for single hit moves.")]
    [SerializeField] private int m_part = 1;

    [Header("Stun Properties")]
    [Tooltip("Stun inflicted upon hitting the target that is blocking (in frames).")]
    [SerializeField] private int m_blockStun;
    [Tooltip("Time stop applied to the target upon hit (in frames).")]
    [SerializeField] private int m_freeze;
    [Tooltip("Stun inflicted upon hitting the target (in frames).")]
    [SerializeField] private int m_knockbackStun;
    [Tooltip("Duration that the target will stay in air, inflicted upon hitting the target (in frames).")]
    [SerializeField] private int m_knockupStun;
    [Tooltip("Duration that the target will stay lying on ground, inflicted upon hitting the target (in frames).")]
    [SerializeField] private int m_knockdownStun;

    [Header("Knockback Properties")]
    [Tooltip("Distance of the knockup.")]
    [SerializeField] private float m_knockup;
    [Tooltip("Distance of the knockback.")]
    [SerializeField] private float m_knockback;

    [Header("Gravity Properties")]
    [Tooltip("Is gravity applied to the performing character during the action?")]
    [SerializeField] private bool m_gravity;

    [Header("SFX Properties")]
    [SerializeField] private AudioClip m_sound;
    [SerializeField] private float m_soundLevel;

    [Header("VFX Properties")]
    [SerializeField] private Vector3 m_screenShakeVelocity;
    [SerializeField] private Vector3 m_cameraPosition;
    [SerializeField] private Vector3 m_cameraRotation;
    [SerializeField] private float m_cameraEaseFactor;

    [Header("Events")]
    [SerializeField] private List<FrameEvent> m_events; 

    [Header("Frame Data")]
    [SerializeField] private int m_startFrames;
    [SerializeField] private int m_activeFrames;
    [SerializeField] private int m_recoveryFrames;

    [Header("Animation Clips")]
    [SerializeField] private AnimationClip m_meshAnimationS;
    [SerializeField] private AnimationClip m_meshAnimationA;
    [SerializeField] private AnimationClip m_meshAnimationR;
    [SerializeField] private AnimationClip m_boxAnimationS;
    [SerializeField] private AnimationClip m_boxAnimationA;
    [SerializeField] private AnimationClip m_boxAnimationR;

    public float AnimSpeedS {get{return AdjustAnimationTime(m_meshAnimationS, m_startFrames);}}
    public float AnimSpeedA {get{return AdjustAnimationTime(m_meshAnimationA, m_activeFrames);}}
    public float AnimSpeedR {get{return AdjustAnimationTime(m_meshAnimationR, m_recoveryFrames);}}

    public Tags Tags {get => m_tags;}
    public int Damage {get => m_damage;}
    public int Priority {get => m_priority;}
    public int Part {get => m_part;}
    public int HitStun {get => m_knockbackStun + m_knockupStun + m_knockdownStun;}
    public int BlockStun {get => m_blockStun;}
    public int Freeze {get => m_freeze;}
    public int KnockbackStun {get => m_knockbackStun;}
    public int KnockupStun {get => m_knockupStun;}
    public int KnockdownStun {get => m_knockdownStun;}
    public float Knockup {get => m_knockup;}
    public float Knockback {get => m_knockback;}
    public bool Gravity {get => m_gravity;}
    public AudioClip Sound {get => m_sound;}
    public float SoundLevel {get => m_soundLevel;}
    public Vector3 ScreenShakeVelocity {get => m_screenShakeVelocity;}

    public int StartFrames {get => m_startFrames;}
    public int ActiveFrames {get => m_activeFrames;}
    public int RecoveryFrames {get => m_recoveryFrames;}
    public int FrameLenght {get => (m_startFrames + m_activeFrames + m_recoveryFrames);}

    public AnimationClip MeshAnimationS {get => m_meshAnimationS;}
    public AnimationClip MeshAnimationA {get => m_meshAnimationA;}
    public AnimationClip MeshAnimationR {get => m_meshAnimationR;}
    public AnimationClip BoxAnimationS {get => m_boxAnimationS;}
    public AnimationClip BoxAnimationA {get => m_boxAnimationA;}
    public AnimationClip BoxAnimationR {get => m_boxAnimationR;}

    [ReadOnly] public bool _firstFrameStartup = true;
    [ReadOnly] public bool _firstFrameActive = true;
    [ReadOnly] public bool _firstFrameRecovery = true;

    public virtual void EnterStateFunction(FighterStateMachine ctx, FighterAttackState state){
        _firstFrameStartup = true;
        _firstFrameActive = true;
        _firstFrameRecovery = true;
        ctx.IsGravityApplied = m_gravity;
    }

    public virtual void FixedUpdateFunction(FighterStateMachine ctx, FighterAttackState state){
        if (state._currentFrame <= state._action.StartFrames){
            if(_firstFrameStartup){
                ctx.Animator.SetFloat("SpeedVar", state._action.AnimSpeedS);
                ctx.ColBoxAnimator.SetFloat("SpeedVar", state._action.AnimSpeedS);
                ctx.Animator.Play("AttackStart");
                ctx.ColBoxAnimator.Play("AttackStart");
                _firstFrameStartup = false;
            }
        }
        else if (state._currentFrame > state._action.StartFrames && state._currentFrame <= state._action.StartFrames + state._action.ActiveFrames){
            if(_firstFrameActive){
                ctx.Animator.SetFloat("SpeedVar", state._action.AnimSpeedA);
                ctx.ColBoxAnimator.SetFloat("SpeedVar", state._action.AnimSpeedA);
                ctx.Animator.Play("AttackActive");
                _firstFrameActive = false;
            }
        }
        else if(state._currentFrame > state._action.StartFrames + state._action.ActiveFrames && 
        state._currentFrame <= state._action.StartFrames + state._action.ActiveFrames + state._action.RecoveryFrames){
            if(_firstFrameRecovery){
                ctx.Animator.SetFloat("SpeedVar", state._action.AnimSpeedR);
                ctx.ColBoxAnimator.SetFloat("SpeedVar", state._action.AnimSpeedR);
                ctx.Animator.Play("AttackRecover");
                _firstFrameRecovery = false;
            }
        }

        // Invoke events.
        // foreach(FrameEvent e in m_events){
        //     if (state._currentFrame == e.Frame){
        //         e.Event?.Invoke();
        //     }
        // }

        state._currentFrame++;
    }

    public virtual void ExitStateFunction(FighterStateMachine ctx, FighterAttackState state){
        ctx.IsGravityApplied = true;
    }
}

[System.Flags]
public enum Tags 
{
    None = 0, //0000
    ShortRanged = 1 << 0, //0001 
    MidRanged = 1 << 1, //0010
    LongRanged = 1 << 2, //0100
    HighDamage = 1 << 3, //1000
    MidDamage = 1 << 4, 
    LowDamage = 1 << 5,
    Projectile = 1 << 6,
    SlowAnimation = 1 << 7,
    MidAnimation = 1 << 8,
    FastAnimation = 1 << 9
}