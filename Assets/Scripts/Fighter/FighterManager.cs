using System;
using System.Collections.Generic;
using System.Data;
using EditableFighterActions;
using UnityEngine;

public class FighterManager : MonoBehaviour
{
    [SerializeField] private FighterBlueprint _fighterBlueprint;
    public Player fighterID;
    private RootNode _inputBasedActionTree;
    private RootNode _conditionalActionTree;

    public FighterEvents fighterEvents;

    private Dictionary<InputGestures, ActionBase> _neutralActionMoveDict;
    private Dictionary<string, ActionAttack> _attackMoveDictByName;
    private Dictionary<string, ActionBase> _actionDictionary;

    public Dictionary<InputGestures, ActionBase> GestureActionDict { get => _neutralActionMoveDict; set => _neutralActionMoveDict = value; }
    public Dictionary<string, ActionBase> ActionDictionary { get => _actionDictionary; set => _actionDictionary = value; }
    
    public RootNode InputBasedActionTree { get => _inputBasedActionTree; }
    public RootNode ConditionalActionTree { get => _conditionalActionTree; }

    void Awake()
    {
        fighterEvents = new FighterEvents();

        _fighterBlueprint.InitializeDictionaries();

        _inputBasedActionTree = _fighterBlueprint.GetRootDict()[ActionTypes.InputBased];
        _conditionalActionTree = _fighterBlueprint.GetRootDict()[ActionTypes.Conditional];

        InitializeDictionaries();
    }

    void OnDisable()
    {
    }

    void InitializeDictionaries()
    {
        _neutralActionMoveDict = new Dictionary<InputGestures, ActionBase>();
        _attackMoveDictByName = new Dictionary<string, ActionAttack>();
        _actionDictionary = new Dictionary<string, ActionBase>();


        List<BPNode> inputBasedActionNodes = new List<BPNode>();
        _inputBasedActionTree.InOrderTreeToList(_inputBasedActionTree, ref inputBasedActionNodes);

        foreach (ActionNode actionNode in inputBasedActionNodes)
        {
            //Debug.Log(actionNode.fighterAction.name);

            if(!_actionDictionary.ContainsKey(actionNode.fighterAction.name)) _actionDictionary.Add(actionNode.fighterAction.name, actionNode.fighterAction);

            CancellableAction cancellableAction = actionNode.fighterAction as CancellableAction;

            if(cancellableAction){
                if(actionNode.GetType() == typeof(NeutralActionNode)) _neutralActionMoveDict.Add(actionNode.inputGesture, cancellableAction); // Neutral Attack Actions
            }
            
            ActionAttack attackAction = actionNode.fighterAction as ActionAttack;

            if(attackAction){
                if(!_attackMoveDictByName.ContainsKey(actionNode.fighterAction.name)) _attackMoveDictByName.Add(actionNode.fighterAction.name, attackAction); // All attack actions
            }
        }

        List<BPNode> conditionalActionNodes = new List<BPNode>();
        _conditionalActionTree.InOrderTreeToList(_conditionalActionTree, ref conditionalActionNodes);

        foreach (ActionNode actionNode in conditionalActionNodes)
        {
            _actionDictionary.Add(actionNode.fighterAction.name, actionNode.fighterAction);
        }

    }

    public void SubscribeInput(InputEvents inputEvents){
        inputEvents.OnDrag += OnDrag;
        inputEvents.OnHold += OnHold;
        inputEvents.OnSwipe += OnSwipe;
        inputEvents.OnTap += OnTap;
        inputEvents.OnDirectInputGesture += OnDirectInput;
        inputEvents.DirectAttackInputByAction += OnDirectAttackInputByAction;
        inputEvents.DirectAttackInputByString += OnDirectAttackInputByString;
    }

    #region input listener functions

    public void OnTap(ScreenSide side)
    {
        if(side == ScreenSide.Right){
            fighterEvents.OnFighterAttackGesture?.Invoke(InputGestures.TapR);
        }   
    }

    public void OnHold(ScreenSide side)
    {
        if(side == ScreenSide.LeftNRight) {
            fighterEvents.OnBlock?.Invoke();
        }
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
            InputGestures inputGesture = InputGestures.None;

            if(swipeDirectionR == GestureDirections.Right) inputGesture = InputGestures.SwipeRightR;
            else if(swipeDirectionR == GestureDirections.Left) inputGesture = InputGestures.SwipeLeftR;
            else if(swipeDirectionR == GestureDirections.Up) inputGesture = InputGestures.SwipeUpR;
            else if(swipeDirectionR == GestureDirections.Down) inputGesture = InputGestures.SwipeDownR;

            if(inputGesture != InputGestures.None) fighterEvents.OnFighterAttackGesture?.Invoke(inputGesture);
        }

    }

    public void OnDirectInput(InputGestures gesture){
        fighterEvents.OnDirectInputGesture?.Invoke(gesture);
    }

    // To listen for direct attack inputs. With this method an attackcen be directly performed by providing it to the method as a parameter
    // Useful for some practices such as AI integration
    public void OnDirectAttackInputByAction(ActionAttack attack)
    {
        if(attack.GetType() == typeof(ActionFighterAttack)) fighterEvents.OnFighterAttackByAction?.Invoke(attack as ActionFighterAttack);
        else fighterEvents.OnSpiritAttack?.Invoke(attack as ActionSpiritAttack);
    }

    public void OnDirectAttackInputByString(string attackName)
    {
        ActionAttack attack = _attackMoveDictByName[attackName];
        if(attack.GetType() == typeof(ActionFighterAttack)) fighterEvents.OnFighterAttackByAction?.Invoke(attack as ActionFighterAttack);
        else fighterEvents.OnSpiritAttack?.Invoke(attack as ActionSpiritAttack);
    }

    #endregion

    private bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
    {
        return potentialDescendant.IsSubclassOf(potentialBase)
           || potentialDescendant == potentialBase;
    }

}
