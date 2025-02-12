using UnityEngine;

public class FighterGroundedState : FighterBaseState
{
    public FighterGroundedState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _isRootState = true;
    }

    public override bool CheckSwitchState()
    {
        if (_ctx.IsHurt && !_ctx.IsInvulnerable){
            //Debug.Log("FighterGroundedState(CheckSwitchState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Root State: " + _ctx.CurrentRootState + " SubState: " + _ctx.CurrentSubState);
            SwitchState(_factory.GetRootState(FighterRootStates.Stunned));
            return true;
        }

        if (!_ctx.IsGrounded || (_ctx.JumpInput.Read() && (_ctx.CurrentSubState == FighterStates.Idle || _ctx.CurrentSubState == FighterStates.Walk))){
            SwitchState(_factory.GetRootState(FighterRootStates.Airborne));
            return true;
        }

        return false;

    }

    public override void EnterState()
    {
        // Reset Gravity to zero.
        _ctx.Gravity = 0f;

        _ctx.Drag = 0f;
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Velocity = Vector2.zero;
        _ctx.FighterController.targetVelocity = Vector2.zero;
    }

    public override void ExitState()
    {    
        // Reset Gravity to default.
        _ctx.Gravity = Physics2D.gravity.y;

        // _ctx.Drag = 0f;
        // _ctx.CurrentMovement = Vector2.zero;
        // _ctx.Velocity = Vector2.zero;
        //_ctx.Rigidbody2D.velocity = Vector2.zero;
        // _ctx.FighterController.targetVelocity = Vector2.zero;
        //Debug.Log("FighterGroundedState(ExitState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Root State: " + _ctx.CurrentRootState + " SubState: " + _ctx.CurrentSubState);
    }

    public override void FixedUpdateState()
    {
        _ctx.CurrentMovement += new Vector2(_ctx.Drag, _ctx.Gravity) * Time.fixedDeltaTime;
        _ctx.Velocity = _ctx.CurrentMovement;
        //_ctx.Rigidbody2D.velocity = _ctx.Velocity;
        _ctx.FighterController.targetVelocity = _ctx.Velocity;
        //Debug.Log("FighterGroundedState(FixedUpdateState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Root State: " + _ctx.CurrentRootState + " SubState: " + _ctx.CurrentSubState);
        //Debug.Log("FighterGroundedState(FixedUpdateState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Drag: " + _ctx.Drag + " Gravity: " + _ctx.Gravity + " Current Movement: " + _ctx.CurrentMovement + " Velocity: " + _ctx.Velocity);
        CheckSwitchState();
    }

    public override void InitializeSubState()
    {
        FighterBaseState state;
        if(_ctx.MovementInput.Read() == 0){
            state = _factory.GetSubState(FighterSubStates.Idle);
        }
        else{
            state = _factory.GetSubState(FighterSubStates.Walk);
        }
        SetSubState(state);
        state.EnterState();
    }

    public override void UpdateState()
    {     
    }
}
