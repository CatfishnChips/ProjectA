using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterGrabbedState : FighterBaseState
{
    private ActionDefault _action;
    private CollisionData _collisionData;
    private ActionAttack _attackAction;
    private int _currentFrame = 0;

    public FighterGrabbedState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {

        if (_ctx.AttackPerformed){
            SwitchState(_factory.Attack());
        }
        else if (_ctx.IsDodgePressed){
            SwitchState(_factory.Dodge());
        }
        else if (_ctx.IsDashPressed){
            SwitchState(_factory.Dash());
        }
        else if (_ctx.MovementInput != 0){            
            SwitchState(_factory.Walk());
        }  
    }

    public override void EnterState()
    { 
        // Take the animation info from the Action as a paramater. 
        // Then let the animation handle all the work (movement).
        // The animation should move the character to the desired location while applying damage at certain times.
        _currentFrame = 0;
        _collisionData = _ctx.HurtCollisionData;
        _attackAction = _collisionData.action;
        _ctx.IsHurt = false;
        
        if (_ctx.IsGrounded) 
        {
            _action = _ctx.ActionDictionary["GroundedIdle"] as ActionDefault;
        }
        else
        {
            _action = _ctx.ActionDictionary["AirborneIdle"] as ActionDefault;
        }

        _ctx.AnimOverrideCont["Idle 1"] = _action.meshAnimation;

        _ctx.Animator.Play("Idle");
        _ctx.ColBoxAnimator.Play("Idle");
    }

    public override void ExitState()
    {
    }

    public override void FixedUpdateState()
    {
        CheckSwitchState();
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {

    }
}
