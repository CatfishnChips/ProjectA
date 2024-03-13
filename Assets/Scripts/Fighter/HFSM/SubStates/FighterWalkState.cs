using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Walk State", menuName = "FighterStates/Sub/WalkState")]
public class FighterWalkState : FighterBaseState
{
    public override void Initialize(IStateMachineRunner ctx, FighterStateFactory factory)
    {
        base.Initialize(ctx, factory);
    }

    public override void CheckSwitchState()
    {
        if (_ctx.AttackInput.Read()){
            SwitchState(_factory.GetSubState(FighterStates.Attack));
        }
        else if (_ctx.DodgeInput.Read()){
            SwitchState(_factory.GetSubState(FighterStates.Dodge));
        }
        else if (_ctx.DashInput.Read()){
            SwitchState(_factory.GetSubState(FighterStates.Dash));
        }
        else if(_ctx.MovementInput.Read() == 0){
            SwitchState(_factory.GetSubState(FighterStates.Idle));
        }
    }

    public override void EnterState()
    {   
        _ctx.Drag = 0f;
        
        if (_ctx.IsGrounded)
        {
            _ctx.Animator.PlayInFixedTime("MoveBT");
        }
    }

    public override void ExitState()
    {
        _ctx.Animator.SetFloat("Blend", 0f);
        _ctx.Gravity = 0f;
        _ctx.Drag = 0f;
        _ctx.CurrentMovement = new Vector2(0, _ctx.Velocity.y);
        _ctx.Velocity = _ctx.CurrentMovement;
        //_ctx.Rigidbody2D.velocity = _ctx.Velocity;
        _ctx.FighterController.targetVelocity = _ctx.Velocity;
    }

    public override void FixedUpdateState()
    {
        if (_ctx.CurrentRootState == FighterStates.Grounded){
            _ctx.Animator.SetFloat("Blend", _ctx.MovementInput.Read() * _ctx.FaceDirection);
            _ctx.CurrentMovement = _ctx.RootMotion;
        }
        else if (_ctx.CurrentRootState == FighterStates.Airborne)
        {
            _ctx.CurrentMovement = new Vector2(_ctx.MovementInput.Read() * _ctx.FaceDirection * _ctx.AirMoveSpeed, _ctx.CurrentMovement.y);
        }

        CheckSwitchState();
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }
}
