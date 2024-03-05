using System.Collections.Generic;
using EditableFighterActions;
using UnityEngine;

public class ActionManager : MonoBehaviour
{

    private Dictionary<InputGestures, ActionNode> _rootDict;
    private Dictionary<InputGestures, ActionNode> _currentSearchDict;

    public ActionManager(Dictionary<InputGestures, ActionNode> rootDict){
        _rootDict = rootDict;
        Debug.Log(_rootDict);
        _currentSearchDict = _rootDict;
    }

    public bool CheckIfChain(InputGestures gesture){
        Debug.Log("Checking if chain move.");

        return _currentSearchDict.ContainsKey(gesture);
    }

    public ActionBase GetAction(InputGestures gesture){
        if(_currentSearchDict.ContainsKey(gesture)){
            _currentSearchDict = _currentSearchDict[gesture].childrenDict;
            return _currentSearchDict[gesture].fighterAction;
        }
        else return null;
    }

    public void Reset(){
        Debug.Log("Resetted.");
        _currentSearchDict = _rootDict;
    }

}
    
