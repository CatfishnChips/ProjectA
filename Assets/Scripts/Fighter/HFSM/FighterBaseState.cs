using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FighterBaseState
{
    protected string _stateName;
    protected FighterStateMachine _ctx;
    protected FighterStateFactory _factory;

    protected FighterBaseState(FighterStateMachine currentState, FighterStateFactory fighterStateFactory)
    =>(_ctx, _factory) = (currentState, fighterStateFactory);

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchState();

    public abstract void InitializeSubState();

    void UpdateStates(){}

    protected void SwitchState(FighterBaseState newState){
        ExitState();

        newState.EnterState();

        _ctx.CurrentState = newState;
    }

    protected void SetSuperState(){}

    protected void SetSubState(){}

    public string StateName{get{return _stateName;}}

}
