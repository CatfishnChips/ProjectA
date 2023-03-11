using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum FighterStates
{
    Idle,
    Walk,
    Run,
    Airborne,
    Grounded,
    Attack,
    Stunned,
    Jump,
    Dash,
    Dodge,
    Block
}

public class FighterStateFactory
{
    FighterStateMachine _context;
    Dictionary<FighterStates, FighterBaseState> _states = new Dictionary<FighterStates, FighterBaseState>();

    public FighterStateFactory(FighterStateMachine currentContext){
        this._context = currentContext;
        _states[FighterStates.Idle] = new FighterIdleState(_context, this);
        _states[FighterStates.Walk] = new FighterWalkState(_context, this);
        _states[FighterStates.Run] = new FighterRunState(_context, this);
        _states[FighterStates.Airborne] = new FighterAirborneState(_context, this);
        _states[FighterStates.Grounded] = new FighterGroundedState(_context, this);
        _states[FighterStates.Attack] = new FighterAttackState(_context, this);
        _states[FighterStates.Stunned] = new FighterStunnedState(_context, this);
        _states[FighterStates.Jump] = new FighterJumpState(_context, this);
        _states[FighterStates.Dash] = new FighterDashState(_context, this);
        _states[FighterStates.Dodge] = new FighterDodgeState(_context, this);
        _states[FighterStates.Block] = new FighterBlockState(_context, this);
    }

    public FighterBaseState Idle(){
        return _states[FighterStates.Idle];
    }
    public FighterBaseState Walk(){
        return _states[FighterStates.Walk];
    }
    public FighterBaseState Run(){
        return _states[FighterStates.Run];
    }
    public FighterBaseState Airborne(){
        return _states[FighterStates.Airborne];
    }
    public FighterBaseState Grounded(){
        return _states[FighterStates.Grounded];
    }
    public FighterBaseState Attack(){
        return _states[FighterStates.Attack];
    }
    public FighterBaseState Stunned(){
        return _states[FighterStates.Stunned];
    }
    public FighterBaseState Jump(){
        return _states[FighterStates.Jump];
    }
    public FighterBaseState Dash(){
        return _states[FighterStates.Dash];
    }
    public FighterBaseState Dodge(){
        return _states[FighterStates.Dodge];
    }
    public FighterBaseState Block(){
        return _states[FighterStates.Block];
    }
}
