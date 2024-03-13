using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter KnockDown State", menuName = "FighterStates/Sub/KnockDownState")]
public class FighterKnockdownState : ActionDefault
{
    private CollisionData _collisionData;
    private ActionAttack _action;

    public override void Initialize(IStateMachineRunner ctx, FighterStateFactory factory)
    {
        base.Initialize(ctx, factory);
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= _action.KnockdownStun){   
            SwitchState(_factory.GetSubState(FighterStates.Idle));
        }
    }

    public override void EnterState()
    {
        _currentFrame = 0;
        _ctx.CurrentFrame = _currentFrame;
        _collisionData = _ctx.HurtCollisionData;
        _action = _collisionData.action;

        if (_action.KnockdownStun == 0) return;

        if (_ctx.PreviousSubState == FighterStates.Knockup || _ctx.PreviousSubState == FighterStates.SlamDunk){
            CameraController.Instance.ScreenShake(new Vector3(0f, -0.1f, 0f));
            return; // If transitioned from Knockup state, do not play the animation.
        } 

        AnimationClip clip = meshAnimation;
        AnimationClip colClip = boxAnimation;

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = colClip;
        

        float speedVar = AdjustAnimationTime(clip, _action.KnockdownStun);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);
        _ctx.ColBoxAnimator.SetFloat("SpeedVar", speedVar);

        _ctx.Animator.PlayInFixedTime("Action");
        _ctx.ColBoxAnimator.PlayInFixedTime("Action");
    }

    public override void ExitState()
    {
        _ctx.CurrentFrame = 0;
        _ctx.Gravity = 0f;
        _ctx.Drag = 0f;
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Velocity = Vector2.zero;
        //_ctx.Rigidbody2D.velocity = Vector2.zero;
        _ctx.FighterController.targetVelocity = Vector2.zero;
    }

    public override void FixedUpdateState()
    {
        CheckSwitchState();
        _currentFrame++;
        _ctx.CurrentFrame =_currentFrame;
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }
}
