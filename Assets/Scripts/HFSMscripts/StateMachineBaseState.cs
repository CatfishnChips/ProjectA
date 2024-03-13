using UnityEngine;

public abstract class StateMachineBaseState : ScriptableObject
{
    protected bool _isRootState = false;
    protected IStateMachineRunner _context;
    protected StateMachineBaseState _currentSubState;
    protected StateMachineBaseState _currentSuperState;

    public StateMachineBaseState GetCurrentSubState { get { return _currentSubState; } }

    public void Initialize(IStateMachineRunner ctx){_context = ctx;}

    public abstract void EnterState();

    public abstract void UpdateState();
    
    public abstract void FixedUpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchState();

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
        if(_currentSubState != null){
            _currentSubState.ExitStates();
        }
        ExitState();
    }

    protected void SwitchState(StateMachineBaseState newState){
        ExitStates();

        newState.EnterState();

        if(_isRootState){
            // set current state of the context (FighterStateMachine) to a new state
            _context.SwitchState(newState);
        }
        else if(_currentSuperState != null){
            _currentSuperState.SetSubState(newState);
        }
    }

    protected void SetSuperState(StateMachineBaseState newSuperState){
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(StateMachineBaseState newSubState){
        _currentSubState = newSubState;
        _currentSubState.SetSuperState(this);
    }
}
