using System;
using UnityEngine;
using UnityEngine.Events;

public struct InputEventsStruct
{
    public Action<Vector2> Swipe;
    public Action<float> Move;
    public Action OnTap;
    public Action<bool> OnHoldA;
    public Action<bool> OnHoldB;
    public Action<string> AttackMove;
}

public struct FighterEvents
{
    #region input events

    public Action<Vector2> Swipe;
    public Action<float> Move;
    public Action OnTap;
    public Action<bool> OnHoldA;
    public Action<bool> OnHoldB;
    public UnityAction<ActionFighterAttack> OnFighterAttack;
    public Action<ActionSpiritAttack> OnSpiritAttack;

    #endregion
}
