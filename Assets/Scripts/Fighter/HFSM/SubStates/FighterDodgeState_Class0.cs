using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Dodge State", menuName = "FighterStates/Class_0/Sub/DodgeState")]
public class FighterDodgeState_Class0 : FighterDodgeState
{
    public override void Initialize(IStateMachineRunner ctx, FighterStateFactory factory)
    {
        base.Initialize(ctx, factory);
    }

    public override void FixedUpdateState()
    {
        if (_currentFrame > _ctx.DodgeTime.x){

            if (_isFirstTime){
                _ctx.IsInvulnerable = true;
                _isFirstTime = false;
            }

            if (_ctx.IsHurt){
                Debug.Log("Script: FighterDodgeState_Class0 - FixedUpdateState : Attack Dodged");
                ((FighterStateMachine_Class0)_ctx).SetFocus(true);
                _ctx.IsHurt = false;
            }
        }
        //Debug.Log("FighterDodgeState_Class0(FixedUpdateState) - Frame: " + _currentFrame);
        _currentFrame++;
        CheckSwitchState();
    }
}
