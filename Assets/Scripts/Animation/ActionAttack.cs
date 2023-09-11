using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "ScriptableObject/Action/Attack")]
public class ActionAttack : ActionBase
{   
    [SerializeField] protected Tags m_tags;
    [Tooltip("Damage dealt upon a successful uncontested hit to the target.")]
    [SerializeField] protected int m_damage;
    [Tooltip("Damage dealt upon a successful hit to a blocking target.")]
    [SerializeField] protected int m_chipDamage;

    [Header("Hitbox Properties")]
    [Tooltip("Which type of hitbox is prioritized for hit detection.")] // Probably won't be used.
    [SerializeField] protected int m_priority;
    [Tooltip("Dictates how many times a move can hit. Set to 1 for single hit moves.")]
    [SerializeField] protected int m_part = 1;

    [Header("Stun Properties")]
    [Tooltip("Does attack ignore target's Block state?")]
    [SerializeField] protected bool m_ignoreBlock;

    [Tooltip("Stun inflicted upon hitting the target that is blocking (in frames).")]
    [SerializeField] protected int m_blockStun;

    [Tooltip("Time stop applied to the target and self upon hit (in frames).")]
    [SerializeField] protected int m_hitStop;

    [Tooltip("Stun inflicted upon hitting the target (in frames).")]
    [SerializeField] protected int m_knockbackStun;

    [Tooltip("Duration that the target will stay lying on ground, inflicted upon hitting the target (in frames).")]
    [SerializeField] protected int m_knockdownStun;

    [Tooltip("Time it takes to complete the arc. x: Rise Time, y: Fall Time (in frames)")]
    [SerializeField] protected Vector2Int m_knockupStun;

    [Header("Knockback Properties")]
    [Tooltip("Distance of the knockup.")]
    [SerializeField] protected float m_knockup;
    [Tooltip("Distance of the knockback.")]
    [SerializeField] protected float m_knockback;

    [Header("Gravity Properties")]
    [Tooltip("Is gravity applied to the performing character during the action?")]
    [SerializeField] protected bool m_gravity = true;

    [Header("Stamina Properties")]
    [Tooltip("How much Stamina is recovered after a successful hit.")]
    [SerializeField] protected float m_staminaRecovery;

    [Header("SFX Properties")]
    [SerializeField] protected AudioClip m_sound;
    [SerializeField] protected float m_soundLevel;

    [Header("VFX Properties")]
    [SerializeField] protected Vector3 m_screenShakeVelocity;
    [SerializeField] protected Vector3 m_cameraPosition;
    [SerializeField] protected Vector3 m_cameraRotation;
    [SerializeField] protected float m_cameraEaseFactor; 

    [Header("Frame Data")]
    [SerializeField] protected int m_startFrames;
    [SerializeField] protected int m_activeFrames;
    [SerializeField] protected int m_recoveryFrames;

    [Header("AI Properties")]
    [ReadOnly] [SerializeField] protected int m_hitboxFrame;
    [ReadOnly] [SerializeField] protected Vector2 m_hitboxOffset;
    [ReadOnly] [SerializeField] protected Vector2 m_hitboxSize;

    [Header("Animation Clips")]
    [SerializeField] protected AnimationClip m_meshAnimationS;
    [SerializeField] protected AnimationClip m_meshAnimationA;
    [SerializeField] protected AnimationClip m_meshAnimationR;
    [SerializeField] protected AnimationClip m_boxAnimationS;
    [SerializeField] protected AnimationClip m_boxAnimationA;
    [SerializeField] protected AnimationClip m_boxAnimationR;

    public float AnimSpeedS {get{return AdjustAnimationTime(m_meshAnimationS, m_startFrames);}}
    public float AnimSpeedA {get{return AdjustAnimationTime(m_meshAnimationA, m_activeFrames);}}
    public float AnimSpeedR {get{return AdjustAnimationTime(m_meshAnimationR, m_recoveryFrames);}}
    public float AnimSpeedAExtended {get{return AdjustAnimationTime(m_meshAnimationA, m_activeFrames + m_hitStop);}}

    public virtual Tags Tags {get => m_tags;}
    public virtual int Damage {get => m_damage;}
    public virtual int ChipDamage {get => m_chipDamage;}
    public virtual int Priority {get => m_priority;}
    public virtual int Part {get => m_part;}
    public virtual bool IgnoreBlock {get => m_ignoreBlock;}
    public virtual int BlockStun {get => m_blockStun;}
    public virtual int HitStop {get => m_hitStop;}
    public virtual int KnockbackStun {get => m_knockbackStun;}
    public virtual Vector2Int KnockupStun {get => m_knockupStun;}
    public virtual int KnockdownStun {get => m_knockdownStun;}
    public virtual float Knockup {get => m_knockup;}
    public virtual float Knockback {get => m_knockback;}
    public virtual bool Gravity {get => m_gravity;}
    public virtual float StaminaRecovery {get => m_staminaRecovery;}
    public virtual AudioClip Sound {get => m_sound;}
    public virtual float SoundLevel {get => m_soundLevel;}
    public virtual Vector3 ScreenShakeVelocity {get => m_screenShakeVelocity;}

    public virtual int StartFrames {get => m_startFrames;}
    public virtual int ActiveFrames {get => m_activeFrames;}
    public virtual int RecoveryFrames {get => m_recoveryFrames;}
    public virtual int FrameLenght {get => (m_startFrames + m_activeFrames + m_recoveryFrames);}

    public int HitboxFrame {get => m_hitboxFrame; set {m_hitboxFrame = value;}}
    public Vector2 HitboxOffset {get => m_hitboxOffset; set {m_hitboxOffset = value;}} 
    public Vector2 HitboxSize {get => m_hitboxSize; set {m_hitboxSize = value;}}

    public virtual AnimationClip MeshAnimationS {get => m_meshAnimationS;}
    public virtual AnimationClip MeshAnimationA {get => m_meshAnimationA;}
    public virtual AnimationClip MeshAnimationR {get => m_meshAnimationR;}
    public virtual AnimationClip BoxAnimationS {get => m_boxAnimationS;}
    public virtual AnimationClip BoxAnimationA {get => m_boxAnimationA;}
    public virtual AnimationClip BoxAnimationR {get => m_boxAnimationR;}

    protected bool _firstFrameStartup = true;
    protected bool _firstFrameActive = true;
    protected bool _firstFrameRecovery = true;
    protected bool _firstTimePause = true;
    protected bool _pause = false;
    protected int _pauseFrames = 0;
    
    protected virtual List<FrameEvent> Events {get {return new List<FrameEvent>();}}

    public virtual void EnterStateFunction(FighterStateMachine ctx, FighterAttackState state){
        _firstFrameStartup = true;
        _firstFrameActive = true;
        _firstFrameRecovery = true;
        _firstTimePause = true;
        _pause = false;
        _pauseFrames = 0;
        ctx.IsGravityApplied = m_gravity;
    }

    public virtual void SwitchActionStateFunction(FighterStateMachine ctx, FighterAttackState state){
        if (state._currentFrame <= state.Action.StartFrames){
            state._actionState = ActionStates.Start;
        }
        else if (state._currentFrame > state.Action.StartFrames && state._currentFrame <= state.Action.StartFrames + state.Action.ActiveFrames){
            state._actionState = ActionStates.Active;
        }
        else if (state._currentFrame > state.Action.StartFrames + state.Action.ActiveFrames && 
        state._currentFrame <= state.Action.StartFrames + state.Action.ActiveFrames + state.Action.RecoveryFrames){
            state._actionState = ActionStates.Recovery;
        }
        else state._actionState = ActionStates.None;
    }

    public virtual void FixedUpdateFunction(FighterStateMachine ctx, FighterAttackState state){
        switch(state._actionState)
        {
            case ActionStates.Start:
                if(_firstFrameStartup){
                    ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedS);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedS);
                    ctx.Animator.Play("AttackStart");
                    ctx.ColBoxAnimator.Play("AttackStart");
                    _firstFrameStartup = false;
                }
            break;

            case ActionStates.Active:
                if(_firstFrameActive){
                    ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedA);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedA);
                    ctx.Animator.Play("AttackActive");
                    _firstFrameActive = false;
                }
            break;

            case ActionStates.Recovery:
                if(_firstFrameRecovery){
                    ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedR);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedR);
                    ctx.Animator.Play("AttackRecover");
                    _firstFrameRecovery = false;
                }
            break;
        }
       
        // Invoke events.
        foreach(FrameEvent e in Events){
            if (state._currentFrame == e.Frame){
                e.Event(ctx, state);
            }
        }

        if (ctx.IsHit) {
            ctx.IsHit = false;

            if (_firstTimePause){
                _firstTimePause = false;
                _pause = true;
                _pauseFrames = m_hitStop;
                
                ctx.Animator.SetFloat("SpeedVar", 0);
                ctx.ColBoxAnimator.SetFloat("SpeedVar", 0);
            }
        } 


        if (_pause){
            if (_pauseFrames <= 0){
                _pause = false;
                ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedA);
                ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedA);
            }
            _pauseFrames--;
        } 
        else
        state._currentFrame++;
    }

    public virtual void ExitStateFunction(FighterStateMachine ctx, FighterAttackState state){
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
    FastAnimation = 1 << 9,
    Grounded = 1 << 10,
    Aerial = 1 << 11
}