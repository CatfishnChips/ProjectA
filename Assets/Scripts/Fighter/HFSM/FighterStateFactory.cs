using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;

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
        _states[FighterStates.Knockup] = new FighterKnockupState(_context, this);
        _states[FighterStates.Knockdown] = new FighterKnockdownState(_context, this);
        _states[FighterStates.Knockback] = new FighterKnockbackState(_context, this);
        _states[FighterStates.Grabbed] = new FighterGrabbedState(_context, this);
        _states[FighterStates.SlamDunk] = new FighterSlamDunkState(_context, this);
    }

    public void OverrideDictionary(Dictionary<FighterStates, FighterBaseState> dictionary){
        foreach(var state in dictionary){
            _states[state.Key] = state.Value;
        }
    }

    public FighterBaseState GetRootState(FighterRootStates state){
        _context.PreviousRootState = _context.CurrentRootState;
        _context.CurrentRootState = (FighterStates)state;
        return _states[(FighterStates)state];
    }

    public FighterBaseState GetSubState(FighterSubStates state){
        _context.PreviousSubState = _context.CurrentSubState;
        _context.CurrentSubState = (FighterStates)state;
        return _states[(FighterStates)state];
    }
}

public struct StateFactoryElement
{
    public FighterStates State;
    public FighterBaseState Reference;
    public StateFactoryElement(FighterStates state, FighterBaseState reference){
        State = state;
        Reference = reference;
    }
}
