using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterStateFactory
{
    FighterStateMachine _context;

    public FighterStateFactory(FighterStateMachine currentContext){
        this._context = currentContext;
    }

    public FighterBaseState Idle(){
        return new FighterIdleState(_context, this);
    }
    public FighterBaseState Walk(){
        return new FighterWalkState(_context, this);
    }
    public FighterBaseState Run(){
        return new FighterRunState(_context, this);
    }
    public FighterBaseState Jump(){
        return new FighterJumpState(_context, this);
    }
    public FighterBaseState Grounded(){
        return new FighterGroundedState(_context, this);
    }
    public FighterBaseState Attack(){
        return new FighterAttackState(_context, this, 20, 20, 10);
    }
}
