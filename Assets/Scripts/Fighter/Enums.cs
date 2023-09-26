using UnityEngine;

// public enum AnimationsEnum{
//     Walking_Backwards,
//     Running_Backwards,
//     Standart_Run,
//     Standart_Walk,
//     Surprise_Uppercut,
//     Idle1,
//     DirectPunch
// }

public enum FighterStates
{
    None,
    Idle,
    Walk,
    Run,
    Airborne,
    Grounded,
    Attack,
    Stunned,
    Jump,
    Dash,
    Dodge,
    Block,
    Knockup,
    Knockdown,
    Knockback,
    Grabbed,
    SlamDunk,
    FreeFall
}

public enum FighterRootStates
{
    None = FighterStates.None,
    Airborne = FighterStates.Airborne,
    Grounded = FighterStates.Grounded,
    Stunned = FighterStates.Stunned
}

public enum FighterSubStates
{
    None = FighterStates.None,
    Idle = FighterStates.Idle,
    Walk = FighterStates.Walk,
    Attack = FighterStates.Attack,
    Dash = FighterStates.Dash,
    Dodge = FighterStates.Dodge,
    Block = FighterStates.Block,
    Knockup = FighterStates.Knockup,
    Knockdown = FighterStates.Knockdown,
    Knockback = FighterStates.Knockback,
    Grabbed = FighterStates.Grabbed,
    SlamDunk = FighterStates.SlamDunk,
    FreeFall = FighterStates.FreeFall
}

public enum Player
{
    None,
    P1,
    P2
}

public enum ActionStates
{
    None,
    Start,
    Active,
    Recovery,
}

public enum Interactions
{
    None, 
    Counter,
    Punish,
    Break
}

public enum Fighters
{
    Char0,
    Char1,
    Char2,
    Char3
}

public enum AIPositionMethod
{
    StraightRandom,
    ArithmeticMean,
    GeometricMean,
    Mod
}

public enum SpiritState
{
    Idle,
    Knockup,
    Knockback
}

public enum StunnedState
{
    Grounded,
    Rise,
    Fall
}