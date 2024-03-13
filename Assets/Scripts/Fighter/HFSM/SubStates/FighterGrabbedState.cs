using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Grabbed State", menuName = "FighterStates/Sub/GrabbedState")]
public class FighterGrabbedState : ActionDefault
{
    private CollisionData _collisionData;
    private ActionAttack _attackAction;

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
        else if (_ctx.MovementInput.Read() != 0){            
            SwitchState(_factory.GetSubState(FighterStates.Walk));
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

        AnimationClip clip = meshAnimation;
        AnimationClip boxClip = boxAnimation;

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = boxClip;

        float speedVar = AdjustAnimationTime(clip, frames);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);
        _ctx.ColBoxAnimator.SetFloat("SpeedVar", speedVar);

        _ctx.Animator.PlayInFixedTime("Action");
        _ctx.ColBoxAnimator.PlayInFixedTime("Action");
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
