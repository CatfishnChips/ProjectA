using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterDodgeState_Class0 : FighterDodgeState
{
    public FighterDodgeState_Class0(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
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
                ((FighterStateMachine_Class0)_ctx).ActivateFocus();
                _ctx.IsHurt = false;
            }
        }
        
        _currentFrame++;
        CheckSwitchState();
    }
}
