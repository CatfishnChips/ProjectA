using System;
using UnityEngine;
using UnityEngine.Events;

public class InputEvents
{
    public Action<Vector2> Swipe;
    public Action<float> Move;
    public Action OnTap;
    public Action<bool> OnHoldA;
    public Action<bool> OnHoldB;
    public Action<string> AttackMove;

    public void SyncorinzeEvents(ref InputEvents inputEvents)
    {
        Debug.Log("Event being synched");
        // Debug.Log(Swipe.ToString());
        Swipe = inputEvents.Swipe;
        // Debug.Log(Swipe.ToString());
        Move = inputEvents.Move;
        OnTap = inputEvents.OnTap;
        OnHoldA = inputEvents.OnHoldA;
        OnHoldB = inputEvents.OnHoldB;
        AttackMove = inputEvents.AttackMove;
    }
}

public class FighterEvents
{
    #region input events

    public Action<Vector2> Swipe;
    public Action<float> OnMove;
    public Action OnTap;
    public Action<bool> OnHoldA;
    public Action<bool> OnHoldB;
    public UnityAction<ActionFighterAttack> OnFighterAttack;
    public Action<ActionSpiritAttack> OnSpiritAttack;

    #endregion
}
