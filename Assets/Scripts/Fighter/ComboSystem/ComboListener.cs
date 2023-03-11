using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboListener
{
    private FighterStateMachine _ctx;

    private Dictionary<string, ComboMoveSpecs> _comboMovesDict;
    private Dictionary<string, ComboMoveSpecs> _currentSearchDict;

    private int _comboFrameLimit = 50;
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
        _previousAttackName = _currentAttackName;
        _currentAttackName = _ctx.AttackName;

        if(_currentFrame > _comboFrameLimit){
            ResetListener();
            return;
        }

        
        if(!_doesDictContainMove){
            if(!_currentSearchDict.ContainsKey(_currentAttackName)){
                ResetListener();
                if(_currentMoveNumber != 1 && _comboMovesDict.ContainsKey(_currentAttackName)){
                    ReactivateListener();
                }
                else{
                    return;
                }
            }
            else{
                _doesDictContainMove = true;
                if(_currentSearchDict[_currentAttackName].willOverride){
                    _ctx.AttackName = _currentSearchDict[_currentAttackName].moveToOverride.name;
                }
            }
        }

        if(_previousAttackName != _currentAttackName && _previousAttackName != null){
            _currentSearchDict = _comboMovesDict[_currentAttackName].possibleNextMoves;
            _doesDictContainMove = false;
            _currentFrame = 0;
        }

        _currentFrame++;
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
        _currentSearchDict = _comboMovesDict;
        _currentAttackName = null;
        _doesDictContainMove = false;
        isActive = false;
    }

    private void OnAttackStart(string name){
        _currentAttackName = name;
    }

    private void ArrangeMovesList(){
        Dictionary<string, ComboMoveSpecs> tempDictWriter = _comboMovesDict;
        for(int i = 0; i < _ctx.CombosArray.Length; i++){
            tempDictWriter = this._comboMovesDict;
            for(int j = 0; j < _ctx.CombosArray[i].moves.Length; j++){
                ComboMoveSpecs comboMoveSpecs = _ctx.CombosArray[i].moves[j];
                if(!tempDictWriter.ContainsKey(comboMoveSpecs.theMove.name)){
                    tempDictWriter.Add(comboMoveSpecs.theMove.name, comboMoveSpecs);
                }
                tempDictWriter = comboMoveSpecs.possibleNextMoves;
            }
        }
    }
}
//ş
