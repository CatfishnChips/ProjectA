using UnityEngine;

public abstract class StateMachineBaseState 
{
    protected bool _isRootState = false;
    protected bool _preserveSubstates = false;
    protected IStateMachineRunner _ctx;
    protected StateMachineBaseState _currentSubState;
    protected StateMachineBaseState _currentSuperState;

    public StateMachineBaseState(IStateMachineRunner ctx) => _ctx = ctx;

    public StateMachineBaseState GetCurrentSubState { get { return _currentSubState; } }

    public abstract void EnterState();

    public abstract void UpdateState();
    
    public abstract void FixedUpdateState();

    public abstract void ExitState();

    public abstract bool CheckSwitchState();

    public abstract void InitializeSubState();

    public void UpdateStates(){ // This function allows for a chained multi-substate architecture by calling update of every substate of supdates.
    //Debug.Log(_stateName);
        if(_currentSubState != null){
            //Debug.Log(_currentSubState._stateName);
        }
        UpdateState();
        if(_currentSubState != null){
            _currentSubState.UpdateStates();
        }
    }

    public void FixedUpdateStates(){ // This function allows for a chained multi-substate architecture by calling update of every substate of supdates.
        if(_currentSubState != null){
            //Debug.Log("FighterBaseState(FixedUpdateStates) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " State: " + _currentSubState);
            _currentSubState.FixedUpdateStates();  
        }
        //Debug.Log("FighterBaseState(FixedUpdateStates) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " State: " + this);
        FixedUpdateState();
    }

    public void ExitStates(){
        if (_preserveSubstates){
            ExitState();
            return;
        }

        StateMachineBaseState subState = _currentSubState;
        while(subState != null){
            //Debug.Log("Exitted Substate : " + subState + " Current Substate : " + _currentSubState);
            subState.ExitState();
            subState = subState._currentSubState;
        }
        ExitState();
        //Debug.Log("Exitted State : " + this);

        // if(_currentSubState != null && !_preserveSubstates){
        //     _currentSubState.ExitStates();
        //     _currentSubState = null;
        // }
        // ExitState();
    }

    public void SwitchState(StateMachineBaseState newState){
        ExitStates();
        newState.EnterState();
        if (_preserveSubstates){
            newState._currentSubState = _currentSubState;
            newState._currentSubState.SetSuperState(newState);
        }
        else{
            newState.InitializeSubState();
            //if (_isRootState) Debug.Log("Root State : " + newState + " New Sub State : " + newState._currentSubState);
        }

        if(_isRootState){
            // set current state of the context (FighterStateMachine) to a new state
            _ctx.SwitchState(newState);
        }
        else if(_currentSuperState != null){
            _currentSuperState.SetSubState(newState);
        }

        _preserveSubstates = false;

        //_currentSubState = null;
    }

    protected void SetSuperState(StateMachineBaseState newSuperState){
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(StateMachineBaseState newSubState){
        _currentSubState = newSubState;
        _currentSubState.SetSuperState(this);
    }
}
