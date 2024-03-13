using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Grounded State", menuName = "FighterStates/Base/GroundedState")]
public class FighterGroundedState : FighterBaseState
{

    public override void Initialize(IStateMachineRunner ctx, FighterStateFactory factory)
    {
        base.Initialize(ctx, factory);
        _isRootState = true;
    }

    public override void CheckSwitchState()
    {
        if (_ctx.IsHurt && !_ctx.IsInvulnerable){
            //Debug.Log("FighterGroundedState(CheckSwitchState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Root State: " + _ctx.CurrentRootState + " SubState: " + _ctx.CurrentSubState);
            Debug.Log("CHANGING THE STATE, HURT");
            SwitchState(_factory.GetRootState(FighterStates.Stunned));
        }

        if (!_ctx.IsGrounded || (_ctx.JumpInput.Read() && (_ctx.CurrentSubState == FighterStates.Idle || _ctx.CurrentSubState == FighterStates.Walk))){
            Debug.Log("CHANGING THE STATE, JUMP");
            SwitchState(_factory.GetRootState(FighterStates.Airborne));
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
    }

    public override void ExitState()
    {    
        _ctx.Gravity = 0f;
        _ctx.Drag = 0f;
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Velocity = Vector2.zero;
        //_ctx.Rigidbody2D.velocity = Vector2.zero;
        _ctx.FighterController.targetVelocity = Vector2.zero;
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
            state = _factory.GetSubState(FighterStates.Idle);
        }
        else{
            state = _factory.GetSubState(FighterStates.Walk);
        }
        SetSubState(state);
        state.EnterState();
    }

    public override void UpdateState()
    {     
    }
}
