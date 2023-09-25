using System;
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

[Flags]
public enum FighterStates
{
    None = 0 <<0,
    Idle = 1 << 0,
    Walk = 1 << 1,
    Run = 1 << 2,
    Airborne = 1 << 3,
    Grounded = 1 << 4,
    Attack = 1 << 5,
    Stunned = 1 << 6,
    Jump = 1 << 7,
    Dash = 1 << 8,
    Dodge = 1 << 9,
    Block = 1 << 10,
    Knockup = 1 << 11,
    Knockdown = 1 << 12,
    Knockback = 1 << 13,
    Grabbed = 1 << 14,
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
    Grabbed = FighterStates.Grabbed
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

public enum RandomCallMethod
{
    OnEachFrame,
    OncePerCall
}