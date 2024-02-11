using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FighterManager : MonoBehaviour
{
    [SerializeField] private FighterID _fighterID;

    public FighterEvents fighterEvents;

    private Dictionary<InputGestures, ActionAttack> _attackMoveDict;
    private Dictionary<string, ActionBase> _actionDictionary;

    public Dictionary<InputGestures, ActionAttack> AttackMoveDict { get => _attackMoveDict; set => _attackMoveDict = value; }
    public Dictionary<string, ActionBase> ActionDictionary { get => _actionDictionary; set => _actionDictionary = value; }
    public ComboMove[] CombosArray{get => _fighterID.CombosArray;}

    void Awake()
    {
        fighterEvents = new FighterEvents();

        InitializeDictionaries();
    }

    void OnDisable()
    {
    }

    void InitializeDictionaries()
    {
        _attackMoveDict = new Dictionary<InputGestures, ActionAttack>();
        _actionDictionary = new Dictionary<string, ActionBase>();

        foreach (InputAttackAttribution attribution in _fighterID.InputAttackAttribution)
        {
            _attackMoveDict.Add(attribution.inputGesture, attribution.actionFighterAttack); // All Attack Actions
        }

        foreach (ActionAttribution attribution in _fighterID.ActionAttribution)
        {
            _actionDictionary.Add(attribution.action.name, attribution.action);
        }

    }

    void EmbedFighter()
    {
        
    }

    #region input listener functions

    public void OnTap(ScreenSide side)
    {
        if(side == ScreenSide.Right){
            fighterEvents.OnFighterAttack?.Invoke(_attackMoveDict[InputGestures.TapR] as ActionFighterAttack);
        }   
    }

    public void OnHold(ScreenSide side)
    {
        if(side == ScreenSide.Left) fighterEvents.OnMove?.Invoke(-1);
        else if(side == ScreenSide.Right) fighterEvents.OnMove?.Invoke(1);
        else if(side == ScreenSide.LeftNRight) fighterEvents.OnBlock?.Invoke();
    }

    public void OnSwipe(ScreenSide screenSide, SwipeDirections swipeDirectionL, SwipeDirections swipeDirectionR)
    {
        if(screenSide == ScreenSide.Left)
        {
            if(swipeDirectionL == SwipeDirections.Left) fighterEvents.OnDash?.Invoke(-1);
            if(swipeDirectionL == SwipeDirections.Right) fighterEvents.OnDash?.Invoke(1);
            
        }

        if(screenSide == ScreenSide.Right)
        {
            ActionAttack actionAttack = null;

            if(swipeDirectionR == SwipeDirections.Right) actionAttack = _attackMoveDict[InputGestures.SwipeRightR];
            else if(swipeDirectionR == SwipeDirections.Left) actionAttack = _attackMoveDict[InputGestures.SwipeLeftR];
            else if(swipeDirectionR == SwipeDirections.Up) actionAttack = _attackMoveDict[InputGestures.SwipeUpR];
            else if(swipeDirectionR == SwipeDirections.Down) actionAttack = _attackMoveDict[InputGestures.SwipeDownR];

            if(actionAttack != null) fighterEvents.OnFighterAttack?.Invoke(actionAttack as ActionFighterAttack);
        }

    }

    // To listen for direct attack inputs. With this method an attackcen be directly performed by providing it to the method as a parameter
    // Useful for some practices such as AI integration
    public void OnDirectAttackInput(ActionAttack attack)
    {
        if(attack.GetType() == typeof(ActionFighterAttack)) fighterEvents.OnFighterAttack?.Invoke(attack as ActionFighterAttack);
        else fighterEvents.OnSpiritAttack?.Invoke(attack as ActionSpiritAttack);
    }

    #endregion

    private bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
    {
        return potentialDescendant.IsSubclassOf(potentialBase)
           || potentialDescendant == potentialBase;
    }

}
