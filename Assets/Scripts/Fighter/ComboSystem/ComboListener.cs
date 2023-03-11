using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboListener
{
    private FighterStateMachine _ctx;

    private Dictionary<string, ComboMoveSpecs> _comboMovesDict;
    private Dictionary<string, ComboMoveSpecs> _currentSearchDict;

    private int _comboFrameLimit = 1500;
    private int _currentMoveNumber;
    private string _currentAttackName;
    private string _previousAttackName;
    private int _currentFrame;
    private bool _doesDictContainMove;
    public bool isActive;

    public ComboListener(FighterStateMachine ctx){
        _ctx = ctx;
        _comboMovesDict = new Dictionary<string, ComboMoveSpecs>();
        ResetListener();
        ArrangeMovesList();
    }

    public void FixedUpdate(){
        if(_currentFrame > _comboFrameLimit){
            ResetListener();
        }

        _currentFrame++;
    }

    public ActionAttack AttackOverride(ActionAttack newAttack){
        ActionAttack returnAttack = null;

        Debug.Log("Current Mov Number: " + _currentMoveNumber);
        Debug.Log("Current ActionAttack : " + newAttack.name);

        if(_currentSearchDict == null) _currentSearchDict = _comboMovesDict;

        foreach (string item in _currentSearchDict.Keys)
        {
            Debug.Log(item);
        }

        if(!_currentSearchDict.ContainsKey(newAttack.name)){
            ResetListener();
            if(_comboMovesDict.ContainsKey(newAttack.name)){
                _currentSearchDict = _comboMovesDict[newAttack.name].possibleNextMoves;
            }
            return newAttack;
        }
        else{
            _currentFrame = 0;
            if(_currentSearchDict[newAttack.name].willOverride){
                returnAttack = _currentSearchDict[newAttack.name].moveToOverride;
            }
            else{
                returnAttack = newAttack;
            }
            _currentSearchDict = _currentSearchDict[newAttack.name].possibleNextMoves;
        }

        _currentMoveNumber++;
        return returnAttack;
    }

    public void ReactivateListener(){
        _currentFrame = 0;
        _currentMoveNumber = 1;
        _currentSearchDict = _comboMovesDict[_currentAttackName].possibleNextMoves;
        isActive = true;
    }

    private void ResetListener(){
        _currentFrame = 0;
        _currentMoveNumber = 1;
        _currentSearchDict = null;
        _currentAttackName = null;
        _doesDictContainMove = false;
        isActive = false;
    }

    private void OnAttackStart(string name){
        _currentAttackName = name;
    }

    private void ArrangeMovesList(){
        Dictionary<string, ComboMoveSpecs> tempDictWriter;
        for(int i = 0; i < _ctx.CombosArray.Length; i++){
            tempDictWriter = this._comboMovesDict;
            for(int j = 0; j < _ctx.CombosArray[i].moves.Length; j++){
                ComboMoveSpecs comboMoveSpecs = _ctx.CombosArray[i].moves[j];
                string comboAttackName = comboMoveSpecs.theMove.name;
                Debug.Log("It's me!" + comboAttackName);
                if(!tempDictWriter.ContainsKey(comboAttackName)){
                    tempDictWriter.Add(comboAttackName, comboMoveSpecs);
                }
                //comboMoveSpecs.DeclareNewPossibleNextMoves();
                tempDictWriter = tempDictWriter[comboAttackName].possibleNextMoves;  
            }
        }
        foreach (KeyValuePair<string, ComboMoveSpecs> item in _comboMovesDict){
            Debug.Log("Cem: " + item.Value.theMove.name);
            
            //Debug.Log(item.Value.possibleNextMoves.Keys);
            foreach (KeyValuePair<string, ComboMoveSpecs> kitem in item.Value.possibleNextMoves){
                Debug.Log(kitem.Value.theMove.name);
            }
        }
    }
}
//ş
