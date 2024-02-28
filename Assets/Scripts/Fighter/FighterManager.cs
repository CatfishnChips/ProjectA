using System;
using System.Collections.Generic;
using System.Data;
using EditableFighterActions;
using UnityEngine;

public class FighterManager : MonoBehaviour
{
    [SerializeField] private FighterBlueprint _fighterBlueprint;
    private RootNode _inputBasedActionTree;
    private RootNode _conditionalActionTree;

    public FighterEvents fighterEvents;

    private Dictionary<InputGestures, ActionAttack> _neutralAttackMoveDict;
    private Dictionary<string, ActionAttack> _attackMoveDictByName;
    private Dictionary<string, ActionBase> _actionDictionary;

    public Dictionary<InputGestures, ActionAttack> AttackMoveDict { get => _neutralAttackMoveDict; set => _neutralAttackMoveDict = value; }
    public Dictionary<string, ActionBase> ActionDictionary { get => _actionDictionary; set => _actionDictionary = value; }

    void Awake()
    {
        fighterEvents = new FighterEvents();

        _inputBasedActionTree = _fighterBlueprint.GetRootDict()[ActionTypes.InputBased];
        _conditionalActionTree = _fighterBlueprint.GetRootDict()[ActionTypes.Conditional];

        InitializeDictionaries();
    }

    void OnDisable()
    {
    }

    void InitializeDictionaries()
    {
        _neutralAttackMoveDict = new Dictionary<InputGestures, ActionAttack>();
        _attackMoveDictByName = new Dictionary<string, ActionAttack>();
        _actionDictionary = new Dictionary<string, ActionBase>();


        List<BPNode> inputBasedActionNodes = new List<BPNode>();
        _inputBasedActionTree.InOrderTreeToList(_inputBasedActionTree, ref inputBasedActionNodes);

        foreach (ActionNode actionNode in inputBasedActionNodes)
        {
            Debug.Log(actionNode.fighterAction.name);

            if(!_actionDictionary.ContainsKey(actionNode.fighterAction.name)) _actionDictionary.Add(actionNode.fighterAction.name, actionNode.fighterAction);

            ActionAttack actionAttack = actionNode.fighterAction as ActionAttack;

            if(actionAttack){
                if(actionNode.GetType() == typeof(NeutralActionNode)) _neutralAttackMoveDict.Add(actionNode.inputGesture, actionAttack); // Neutral Attack Actions

                if(!_attackMoveDictByName.ContainsKey(actionNode.fighterAction.name)) _attackMoveDictByName.Add(actionNode.fighterAction.name, actionAttack); // All attack actions
            }
        }

        List<BPNode> conditionalActionNodes = new List<BPNode>();
        _conditionalActionTree.InOrderTreeToList(_conditionalActionTree, ref conditionalActionNodes);

        foreach (ActionNode actionNode in conditionalActionNodes)
        {
            _actionDictionary.Add(actionNode.fighterAction.name, actionNode.fighterAction);
        }

    }

    void EmbedFighter()
    {
        
    }

    #region input listener functions

    public void OnTap(ScreenSide side)
    {
        if(side == ScreenSide.Right){
            fighterEvents.OnFighterAttack?.Invoke(_neutralAttackMoveDict[InputGestures.TapR] as ActionFighterAttack);
        }   
    }

    public void OnHold(ScreenSide side)
    {
        if(side == ScreenSide.LeftNRight) fighterEvents.OnBlock?.Invoke();
    }

    public void OnDrag(ScreenSide side, GestureDirections direction)
    {
        if(side == ScreenSide.Left) {
            if(direction == GestureDirections.Left) fighterEvents.OnMove?.Invoke(-1);
            else if(direction == GestureDirections.Right) fighterEvents.OnMove?.Invoke(1);
        }
    }

    public void OnSwipe(ScreenSide screenSide, GestureDirections swipeDirectionL, GestureDirections swipeDirectionR)
    {
        if(screenSide == ScreenSide.Left)
        {
            if(swipeDirectionL == GestureDirections.Left) fighterEvents.OnDash?.Invoke(-1);
            if(swipeDirectionL == GestureDirections.Right) fighterEvents.OnDash?.Invoke(1);
            
        }

        if(screenSide == ScreenSide.Right)
        {
            ActionAttack actionAttack = null;

            if(swipeDirectionR == GestureDirections.Right) actionAttack = _neutralAttackMoveDict[InputGestures.SwipeRightR];
            else if(swipeDirectionR == GestureDirections.Left) actionAttack = _neutralAttackMoveDict[InputGestures.SwipeLeftR];
            else if(swipeDirectionR == GestureDirections.Up) actionAttack = _neutralAttackMoveDict[InputGestures.SwipeUpR];
            else if(swipeDirectionR == GestureDirections.Down) actionAttack = _neutralAttackMoveDict[InputGestures.SwipeDownR];

            if(actionAttack != null) fighterEvents.OnFighterAttack?.Invoke(actionAttack as ActionFighterAttack);
        }

    }

    // To listen for direct attack inputs. With this method an attackcen be directly performed by providing it to the method as a parameter
    // Useful for some practices such as AI integration
    public void OnDirectAttackInputByAction(ActionAttack attack)
    {
        if(attack.GetType() == typeof(ActionFighterAttack)) fighterEvents.OnFighterAttack?.Invoke(attack as ActionFighterAttack);
        else fighterEvents.OnSpiritAttack?.Invoke(attack as ActionSpiritAttack);
    }

    public void OnDirectAttackInputByString(string attackName)
    {
        ActionAttack attack = _attackMoveDictByName[attackName];
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
