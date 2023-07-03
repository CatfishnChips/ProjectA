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


public enum AnimationStates
{
    None,
    Start,
    Active,
    Recovery,
    Complete
}
