using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ActionAttack : CancellableAction
{   
    [SerializeField] protected Tags m_tags;
    [Tooltip("Damage dealt upon a successful uncontested hit to the target.")]
    [SerializeField] protected int m_damage;
    [Tooltip("How many Juggle points will be deduced from the Juggle state?")]
    [SerializeField] protected int m_juggle;
    [Tooltip("Face other the opponent before the action?")]
    [SerializeField] protected bool m_face;
    [Tooltip("Properties of the hit. Determines which state the opponent will be in.")]
    [SerializeField] protected HitFlags m_hitFlags;

    [Header("Wall Bounce Properties")]
    [Tooltip("Velocity of the bounce after hitting a wall.")]
    [SerializeField] protected Vector2 m_wallBounceVelocity;
    [Tooltip("Duration of the stun received after bouncing of a wall (in frames).")]
    [SerializeField] protected int m_wallBounceStun;

    [Header("Ground Bounce Properties")]
    [Tooltip("Velocity of the bounce after hitting the ground.")]
    [SerializeField] protected Vector2 m_groundBounceVelocity;
    [Tooltip("Duration of the stun recieved after bouncing of the ground x: Rise Time, y: Fall Time (in frames).")]
    [SerializeField] protected Vector2Int m_groundBounceStun;

    [Header("Wall Splat Properties")]
    [Tooltip("Duration of the stun received after splatting to a wall (in frames).")]
    [SerializeField] protected int m_wallSplatStun;

    [Header("Hitbox Properties")]
    [Tooltip("Which type of hitbox is prioritized for hit detection.")] // Probably won't be used.
    [SerializeField] protected int m_priority;
    [Tooltip("Dictates how many times a move can hit. Set to 1 for single hit moves.")]
    [SerializeField] protected int m_part = 1;

    [Header("Block Properties")]
    [Tooltip("Damage dealt upon a successful hit to a blocking target.")]
    [SerializeField] protected int m_chipDamage;
    [Tooltip("Stun inflicted upon hitting the target that is blocking (in frames).")]
    [SerializeField] protected int m_blockStun;
    [Tooltip("Distance of pushback of hit when blocked.")]
    [SerializeField] protected float blockKnocback;

    [Header("Stun Properties")]
    [Tooltip("Does attack ignore target's Block state?")]
    [SerializeField] protected bool m_ignoreBlock;

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

    [Header("Recovery Properties")]
    [Tooltip("How much Stamina is recovered after a successful hit.")]
    [SerializeField] protected float m_staminaRecovery;
    [Tooltip("How much Spirit is recovered after a successful hit.")]
    [SerializeField] protected float m_spiritRecovery;

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
    [SerializeField] protected int m_hitboxFrame;
    [SerializeField] protected Vector2 m_hitboxOffset;
    [SerializeField] protected Vector2 m_hitboxLocation;
    [SerializeField] protected Vector2 m_hitboxSize;

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

    public virtual HitFlags HitFlags {get => m_hitFlags;}
    public virtual Tags Tags {get => m_tags;}
    public virtual int Damage {get => m_damage;}
    public virtual int ChipDamage {get => m_chipDamage;}
    public float BlockKnocback {get => blockKnocback;}
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
    public virtual float SpiritRecovery {get => m_spiritRecovery;}
    public virtual AudioClip Sound {get => m_sound;}
    public virtual float SoundLevel {get => m_soundLevel;}
    public virtual Vector3 ScreenShakeVelocity {get => m_screenShakeVelocity;}

    public virtual int Juggle {get => m_juggle;}

    public virtual Vector2 WallBounceVelocity {get => m_wallBounceVelocity;}
    public virtual int WallBounceStun {get => m_wallBounceStun;}

    public virtual Vector2 GroundBounceVelocity {get => m_groundBounceVelocity;}
    public virtual Vector2Int GroundBounceStun {get => m_groundBounceStun;}

    public virtual int WallSplatStun {get => m_wallSplatStun;}

    public virtual int StartFrames {get => m_startFrames;}
    public virtual int ActiveFrames {get => m_activeFrames;}
    public virtual int RecoveryFrames {get => m_recoveryFrames;}
    public virtual int FrameLenght {get => (m_startFrames + m_activeFrames + m_recoveryFrames);}

    public int HitboxFrame {get => m_hitboxFrame; set {m_hitboxFrame = value;}}
    public Vector2 HitboxOffset {get => m_hitboxOffset; set {m_hitboxOffset = value;}}
    public Vector2 HitboxLocation {get => m_hitboxLocation; set {m_hitboxLocation = value;}} 
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

    public bool Pause { get => _pause; }

    // public virtual void FixedUpdateFunction(FighterStateMachine ctx, FighterAttackState state){

    //     if(state.listeningForChainInput && state._currentFrame >= m_inputIgnoreFrames){
    //         if(ctx.AttackInput.ReadContent() != InputGestures.None){
    //             if(ctx.ActionManager.CheckIfChain(ctx.AttackInput.ReadContent())){
    //                 state.chainInputGesture = ctx.AttackInput.ReadContent();
    //                 state.performedComboMove = true;
    //             }
    //             else{
    //                 state.listeningForChainInput = false;
    //             }
    //         }
    //     }

    //     if(state._currentFrame == m_cancelFrames + 1) ctx.ActionManager.Reset();
    // }

    public static float KinematicsXY(float time, float initialPositionXY, float initialVelocityXY, float accelerationXY){
        return initialPositionXY + initialVelocityXY * time + accelerationXY * time * time / 2;
    }

    public static float TimeOfFlight(float initialVelocityY, float gravity){
        return -2 * initialVelocityY / gravity;
    }

    public static float Height(float initialVelocityY, float time){
        return initialVelocityY / 2 * time;
    }

    public static float Range(float initialVelocityX, float time){
        return initialVelocityX * time;
    }

    [SerializeField] private Vector2 _velocity;
    [ReadOnly] [SerializeField] private float Time;
    [ReadOnly] [SerializeField] private int Frame;
    [ReadOnly] [SerializeField] private float Apex;
    [ReadOnly] [SerializeField] private float Displacement;

    public void Calculate(){
        Time = TimeOfFlight(_velocity.y, Physics2D.gravity.y);
        Frame = Mathf.CeilToInt(Time /  UnityEngine.Time.fixedDeltaTime);
        Apex = Height(_velocity.y, Time);
        Displacement = Range(_velocity.x, Time);
    }

    [SerializeField] private GroundProperties _ground;
    [SerializeField] private AirProperties _air;
    [SerializeField] private KnockdownProperties _knockdown;

    public GroundProperties Ground { get => _ground; }
    public AirProperties Air { get => _air; }
    public KnockdownProperties Knockdown { get => _knockdown; }
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

[System.Flags]
public enum HitFlags
{
    NONE = 0, //0000
    // Slide = 1 << 0, //0001
    // Fall = 1 << 1, //0010
    // Trip = 1 << 2, //0100
    // Bounce = 1 << 3, //1000

    KNOCK_BACK = 1 << 0,
    KNOCK_UP = 1 << 1,
    KNOCK_DOWN = 1 << 2,
    BOUNCE_WALL = 1 << 3,
    BOUNCE_GROUND = 1 << 4,
    SPLAT_WALL = 1 << 5
}
[Serializable]
public struct StunProperties{
    [Header("Stun Properties")]

    [Tooltip("Time stop applied to the target and/or self upon hit (in frames).")]
    public int hitStop;

    [Tooltip("Stun inflicted upon hitting the target (in frames).")]
    public int stun;
}

[Serializable]
public struct ArcProperties{
    [Header("Arc Properties")]

    [Tooltip("Apex height of the arc.")]
    public float apex;

    [Tooltip("Range of the arc.")]
    public float range;

    [Tooltip("Time (in frames) it takes to reach the Apex of the arc.")]
    public int timeToApex;

    [Tooltip("Time (in frames) spent at Apex of the arc before starting to fall.")]
    public int timeAtApex;

    [Tooltip("Time (in frames) it takes to reach the end of the arc from the Apex.")]
    public int timeToFall;
}

[Serializable]
public struct LineProperties{
    [Header("Line Properties")]

    [Tooltip("Initial velocity applied to the target.")]
    public Vector2 velocity;
}

[Serializable]
public struct KnockdownProperties{
    [Header("Knockdown Properties")]

    [Tooltip("Damage inflicted upon hitting the ground.")]
    public int damage;

    public StunProperties Stun;
}

[Serializable]
public struct SlideProperties{
    [Header("Slide Properties")]

    [Tooltip("Time (in frames) to slide after getting hit.")]
    public int slide;

    [Tooltip("Distance to slide.")]
    public int distance;
}

[Serializable]
public struct GroundProperties
{
    [Header("Ground Properties")]

    public StunProperties Stun;
    public SlideProperties Slide;
    public Trajectory trajectory;
    public ArcProperties Arc;
    public LineProperties Line;
    public BlockProperties Block;
    public WallBounceProperties WallBounce;
    public GroundBounceProperties GroundBounce;
    public WallSplatProperties WallSplat;
}

[Serializable]
public struct AirProperties
{
    [Header("Air Properties")]

    public StunProperties Stun;
    public Trajectory trajectory;
    public ArcProperties Arc;
    public LineProperties Line;
    public BlockProperties Block;
    public WallBounceProperties WallBounce;
    public GroundBounceProperties GroundBounce;
    public WallSplatProperties WallSplat;
}

[Serializable]
public struct BlockProperties
{   
    [Header("Block Properties")]

    [Tooltip("Does attack ignore target's Block state?")]
    public bool ignoreBlock;

    [Tooltip("Damage dealt upon a successful hit to a blocking target.")]
    public int damage;

    public StunProperties Stun;
    public SlideProperties Slide;
}

[Serializable]
public struct BounceProperties{
    [Header("Bounce Properties")]

    [Tooltip("Damage inflicted upon hitting an obstacle.")]
    public int damage;

    public StunProperties Stun;
    public Trajectory trajectory;
    public ArcProperties Arc;
    public LineProperties Line;
}

[Serializable]
public struct WallBounceProperties{
    [Header("Wall Bounce Properties")]

    public BounceProperties Bounce;
}

[Serializable]
public struct GroundBounceProperties{
    [Header("Ground Bounce Properties")]

    public BounceProperties Bounce;
}

[Serializable]
public struct WallSplatProperties{
    [Header("Wall Splat Properties")]

    [Tooltip("Damage inflicted upon hitting an obstacle.")]
    public int damage;

    public StunProperties Stun;
}

[Serializable]
public enum Trajectory{
    ARC,
    LINE
}