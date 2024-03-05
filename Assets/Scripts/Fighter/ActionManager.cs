using EditableFighterActions;
using UnityEngine;

public class ActionManager : MonoBehaviour
{

    private BPNode _root;
    private BPNode _currentSearchNode;

    public ActionManager(BPNode root){
        _root = root;
        Debug.Log(_root);
        _currentSearchNode = _root;
    }

    public BPNode CheckIfChain(InputGestures gesture){
        Debug.Log("Checking if chain move.");
        foreach(BPNode child in _currentSearchNode.Children){
            if(gesture == (child as ActionNode).inputGesture){
                Debug.Log("Chain move!");
                return child;
            }
        }
        return null;
    }

    public void ItarateForward(BPNode node){
        _currentSearchNode = node;
    }

    public ActionBase GetAction(InputGestures gesture){
        Debug.Log(_currentSearchNode);
        if(_currentSearchNode == _root){
            foreach(BPNode child in _currentSearchNode.Children){
                ActionNode childActionNode = child as ActionNode;
                if(childActionNode && gesture == (child as ActionNode).inputGesture){
                    _currentSearchNode = child;
                    Debug.Log(childActionNode.fighterAction.name);
                    return childActionNode.fighterAction;
                }
            }
        }
        else{
            Debug.Log(_currentSearchNode);
            return (_currentSearchNode as ActionNode).fighterAction;
        }
        return null;
    }

    public void Reset(){
        Debug.Log("Resetted.");
        _currentSearchNode = _root;
    }

}
    
