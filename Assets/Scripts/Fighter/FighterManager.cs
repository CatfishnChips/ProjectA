using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FighterManager : MonoBehaviour
{
    [SerializeField] private FighterID _fighterID;

    public FighterEvents fighterEvents;

    protected Dictionary<string, ActionAttack> _attackMoveDict;
    protected Dictionary<string, ActionAttack> _groundedAttackMoveDict;
    protected Dictionary<string, ActionAttack> _aerialAttackMoveDict;
    protected Dictionary<string, ActionBase> _actionDictionary;

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
        _attackMoveDict = new Dictionary<string, ActionAttack>();
        _groundedAttackMoveDict = new Dictionary<string, ActionAttack>();
        _aerialAttackMoveDict = new Dictionary<string, ActionAttack>();
        _actionDictionary = new Dictionary<string, ActionBase>();

        foreach (ActionAttribution attribution in _fighterID.ActionAttribution)
        {
            if (IsSameOrSubclass(typeof(ActionAttack), attribution.action.GetType()))
            {
                ActionAttack action = Instantiate(attribution.action) as ActionAttack;
                _attackMoveDict.Add(action.name, action); // All Attack Actions
                if(action.Tags.HasFlag(Tags.Grounded)) _groundedAttackMoveDict.Add(action.name, action); // Grounded attack Actions
                else if(action.Tags.HasFlag(Tags.Aerial)) _aerialAttackMoveDict.Add(action.name, action); // Aerial Attack Actions
            }
            else 
            {
                _actionDictionary.Add(attribution.action.name, attribution.action);
            }
        }
    }

    void EmbedFighter()
    {
        
    }

    #region input listener functions

    public void OnSwipe(Vector2 direction){
        Debug.Log("Swipe Detected");
        fighterEvents.Swipe?.Invoke(direction);
    }

    public void OnTapA(){
        fighterEvents.OnTap?.Invoke();
    }

    public void OnHoldA(bool value){
        fighterEvents.OnHoldA?.Invoke(value);
    }

    public void OnHoldB(bool value){
        fighterEvents.OnHoldB?.Invoke(value);
    }

    public void OnMoveA(float value){
        fighterEvents.OnMove?.Invoke(value);
    }

    public void OnGestureB(string attackName){
        if(attackName == "L") return;

        ActionAttack attack = _attackMoveDict[attackName];

        if (IsSameOrSubclass(typeof(ActionFighterAttack), attack.GetType())){
            fighterEvents.OnFighterAttack?.Invoke(attack as ActionFighterAttack);
        }
        else if (IsSameOrSubclass(typeof(ActionSpiritAttack), attack.GetType())){
            fighterEvents.OnSpiritAttack?.Invoke(attack as ActionSpiritAttack);
        }
    }

    #endregion

    private bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
    {
        return potentialDescendant.IsSubclassOf(potentialBase)
           || potentialDescendant == potentialBase;
    }

}
