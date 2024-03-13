using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Run State", menuName = "FighterStates/Sub/RunState")]
public class FighterRunState : FighterBaseState
{
    public override void Initialize(IStateMachineRunner ctx, FighterStateFactory factory)
    {
        base.Initialize(ctx, factory);
    }

    public override void CheckSwitchState()
    {
        if (!(_ctx.Velocity.x < -0.5f || _ctx.Velocity.x > 0.5f)){ // if fighter is not running
            if (_ctx.Velocity.x >= -0.5f && _ctx.Velocity.x <= 0.5f){ // if we just slowed sown
                SwitchState(_factory.GetSubState(FighterStates.Idle));
            }
            else{ // if we directly stopped
                SwitchState(_factory.GetSubState(FighterStates.Idle));
            }
        }
    }

    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }
}
