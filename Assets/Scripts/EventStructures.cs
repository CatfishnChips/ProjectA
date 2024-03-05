using System;
using UnityEngine;
using UnityEngine.Events;

public class InputEvents
{
    public Action<ScreenSide, GestureDirections, GestureDirections> OnSwipe;
    public Action<ScreenSide, GestureDirections> OnDrag;
    public Action<ScreenSide> OnHold;
    public Action<ScreenSide> OnTap;
    public Action<ActionAttack> DirectAttackInputByAction; // To directly perform the attack without passing the input method
    public Action<string> DirectAttackInputByString;

    public void SyncorinzeEvents(ref InputEvents inputEvents)
    {
        Debug.Log("Event being synched");
        OnSwipe = inputEvents.OnSwipe;
        OnHold= inputEvents.OnHold;
        OnTap = inputEvents.OnTap;
    }
}

public class FighterEvents
{
    #region input events

    public Action<int> OnDash;
    public Action<int> OnMove;
    public Action OnBlock;
    public UnityAction<InputGestures> OnFighterAttackGesture;
    public UnityAction<ActionAttack> OnFighterAttackByAction;
    public UnityAction<string> OnFighterAttackByName;
    public Action<ActionSpiritAttack> OnSpiritAttack;

    #endregion
}
