using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterIdleState : FighterBaseState
{
    private ActionDefault _action;

    public FighterIdleState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (_ctx.AttackInput.Read()){
            SwitchState(_factory.GetSubState(FighterSubStates.Attack));
        }
        else if (_ctx.DodgeInput.Read() && _ctx.IsGrounded && _ctx.CurrentRootState == FighterStates.Grounded){
            SwitchState(_factory.GetSubState(FighterSubStates.Dodge));
        }
        else if (_ctx.DashInput.Read() && _ctx.IsGrounded && _ctx.CurrentRootState == FighterStates.Grounded){
            SwitchState(_factory.GetSubState(FighterSubStates.Dash));
        }
        else if (_ctx.MovementInput.Read() != 0){
            SwitchState(_factory.GetSubState(FighterSubStates.Walk));
        }  
    }

    public override void EnterState()
    {
        if (_ctx.CurrentRootState == FighterStates.Grounded) 
        {
            _action = _ctx.ActionDictionary["GroundedIdle"] as ActionDefault;
        }
        else if (_ctx.CurrentRootState == FighterStates.Airborne)
        {
            _action = _ctx.ActionDictionary["AirborneIdle"] as ActionDefault;
        }
        AnimationClip clip = _action.meshAnimation;
        AnimationClip boxClip = _action.boxAnimation;

        _ctx.AnimOverrideCont["Idle"] = clip;
        _ctx.ColBoxOverrideCont["Box_Idle"] = boxClip;

        // float speedVar = AdjustAnimationTime(clip, _action.frames);
        _ctx.Animator.SetFloat("SpeedVar", 1f);
        _ctx.ColBoxAnimator.SetFloat("SpeedVar", 1f);

        _ctx.Animator.PlayInFixedTime("IdleFallback");
        _ctx.ColBoxAnimator.PlayInFixedTime("IdleFallback");
    }

    public override void ExitState()
    {
        _ctx.Gravity = 0f;
        _ctx.Drag = 0f;
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Velocity = Vector2.zero;
        //_ctx.Rigidbody2D.velocity = Vector2.zero;
        _ctx.FighterController.targetVelocity = Vector2.zero;
        //Debug.Log("FighterIdleState(ExitState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad  + " Root State: " + _ctx.CurrentRootState + " SubState: " + _ctx.CurrentSubState);
    }

    public override void FixedUpdateState()
    {   
        //Debug.Log("FighterIdleState(FixedUpdateState) - Player: " + _ctx.Player + " Time: " + Time.timeSinceLevelLoad + " Root State: " + _ctx.CurrentRootState + " SubState: " + _ctx.CurrentSubState);
        CheckSwitchState();
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {

    }
}
