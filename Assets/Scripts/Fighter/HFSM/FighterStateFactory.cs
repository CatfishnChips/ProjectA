using System.Collections.Generic;
using UnityEngine;

public class FighterStateFactory
{
    FighterStateMachine _context;
    Dictionary<FighterStates, FighterBaseState> _states;
    Dictionary<string, ActionAttack> _attackStates;

    public FighterStateFactory(FighterStateMachine currentContext, FighterManager manager){
        _context = currentContext;
        _states = manager.ActionDictionary;
        _attackStates = manager.AttackMoveDictByName;
    }

    public void OverrideDictionary(Dictionary<FighterStates, FighterBaseState> dictionary){
        foreach(var state in dictionary){
            _states[state.Key] = state.Value;
        }
    }

    public FighterBaseState GetRootState(FighterStates state){
        _context.ActionManager.Reset();
        _context.PreviousRootState = _context.CurrentRootState;
        _context.CurrentRootState = state;
        Debug.Log(_states[state]);
        return _states[state];
    }

    public FighterBaseState GetSubState(FighterStates state){
        _context.ActionManager.Reset();
        _context.PreviousSubState = _context.CurrentSubState;
        _context.CurrentSubState = state;
        return _states[state];
    }

    public FighterBaseState GetChainState(InputGestures gesture){
        FighterBaseState action = _context.ActionManager.GetAction(gesture);
        _context.PreviousSubState = _context.CurrentSubState;
        _context.CurrentSubState = action.stateName;
        return action;
    }

}

// public struct StateFactoryElement
// {
//     public FighterStates State;
//     public FighterBaseState Reference;
//     public StateFactoryElement(FighterStates state, FighterBaseState reference){
//         State = state;
//         Reference = reference;
//     }
// }
