using UnityEngine;

public abstract class FighterBaseState
{
    protected bool _isRootState = false;
    protected FighterStateMachine _ctx;
    protected FighterStateFactory _factory;
    protected FighterBaseState _currentSubState;
    protected FighterBaseState _currentSuperState;

    public FighterBaseState GetCurrentSubState { get { return _currentSubState; } }

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

    protected void SwitchState(FighterBaseState newState){
        ExitStates();

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

    public static float AdjustAnimationTime(AnimationClip clip, int frames){
        float length;
        float time;
        float speed;

        length = clip.length;
        //time = frames / clip.frameRate;
        time = frames * Time.deltaTime;
        speed = length / time;
        //Debug.Log("Calculated Lenght: " + time + " Actual Lenght: " + length  + " Frame Rate: " + clip.frameRate + " Calculated Speed: " + speed);

        return speed;
    }
}
