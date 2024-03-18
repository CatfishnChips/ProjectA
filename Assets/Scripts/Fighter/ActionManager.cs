using System.Collections.Generic;
using EditableFighterActions;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    private int _treeLevel;
    private InputGestures _lastGesture;
    private Dictionary<InputGestures, ActionNode> _rootDict;
    private Dictionary<InputGestures, ActionNode> _currentSearchDict;

    public int TreeLevel { get => _treeLevel; set => _treeLevel = value; }

    public ActionManager(Dictionary<InputGestures, ActionNode> rootDict){
        _rootDict = rootDict;
        _currentSearchDict = _rootDict;
        _treeLevel = 0;
        _lastGesture = InputGestures.None;
    }

    public CancellableAction GetAction(InputGestures gesture){
        // Debug.Log("Trying to get an action with gesture: " + gesture);
        int i = 0;
        foreach(KeyValuePair<InputGestures, ActionNode> keyValuePair in _currentSearchDict){
            // Debug.Log(i + ": " + "Key: " + keyValuePair.Key + " Value: " + keyValuePair.Value.fighterAction.name);
            i++;
        }
        if(_currentSearchDict.ContainsKey(gesture)){
            CancellableAction action = _currentSearchDict[gesture].fighterAction as CancellableAction;
            _lastGesture = gesture;
            // Debug.Log("Found a key with the given gesture.");
            return action;
        }
        else{
            // Debug.Log("Couldn't find a key with the given gesture.");
            return null;
        } 
    }

    public void ItarateForward(InputGestures gesture){
        _currentSearchDict = _currentSearchDict[gesture].childrenDict;
        _treeLevel++;
    }

    public CancellableAction PeekAction(InputGestures gesture){
        if(_currentSearchDict.ContainsKey(gesture)){
            CancellableAction action = _currentSearchDict[gesture].fighterAction as CancellableAction;
            return action;
        }
        else return null;
    }

    public bool CheckIfChain(InputGestures gesture){
        // Debug.Log("Chain move check result: " + _currentSearchDict.ContainsKey(gesture));

        return _currentSearchDict.ContainsKey(gesture);
    }

    public void Reset(){
        //Debug.Log("Resetted.");
        _currentSearchDict = _rootDict;
        _treeLevel = 0;
        _lastGesture = InputGestures.None;
    }

}
    
