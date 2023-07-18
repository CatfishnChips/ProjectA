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
    Grabbed
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
