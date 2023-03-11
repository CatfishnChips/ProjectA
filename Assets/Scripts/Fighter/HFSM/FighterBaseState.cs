using UnityEngine;

public abstract class FighterBaseState
{
    protected bool _isRootState = false;
    protected string _stateName;
    protected FighterStateMachine _ctx;
    protected FighterStateFactory _factory;
    protected FighterBaseState _currentSubState;
    protected FighterBaseState _currentSuperState;

    protected FighterBaseState(FighterStateMachine currentState, FighterStateFactory fighterStateFactory)
    =>(_ctx, _factory) = (currentState, fighterStateFactory);

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
        FixedUpdateState();
        if(_currentSubState != null){
            _currentSubState.FixedUpdateStates();
        }
    }

    public void ExitStates(){
        ExitStates();
        if(_currentSubState != null){
            _currentSubState.ExitStates();
        }
    }

    protected void SwitchState(FighterBaseState newState){
        ExitState();

        newState.EnterState();

        if(_isRootState){
            // set current state of the context (FighterStateMachine) to a new state
            _ctx.CurrentState = newState;
        }
        else if(_currentSuperState != null){
            _currentSuperState.SetSubState(newState);
        }
    }

    protected void SetSuperState(FighterBaseState newSuperState){
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(FighterBaseState newSubState){
        _currentSubState = newSubState;
        _currentSubState.SetSuperState(this);
    }

    public string StateName{get{return _stateName;}}
    public string SubStateName(){
        if(_currentSubState != null){
            return _currentSubState.StateName;
        }
        else{
            return "None";
        }
    }
    public string SuperStateName(){
        if(_currentSuperState != null){
            return _currentSuperState.StateName;
        }
        else{
            return "None";
        }
    }

    public static float AdjustAnimationTime(AnimationClip clip, int frames){
        float length;
        float time;
        float speed;

        length = clip.length;
        time = frames * Time.fixedDeltaTime;
        speed = length / time;

        return speed;
    }
}
