using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Dodge State", menuName = "FighterStates/Sub/DodgeState")]
public class FighterDodgeState : ActionDefault
{
    protected bool _isFirstTime = true;

    public override void Initialize(IStateMachineRunner ctx, FighterStateFactory factory)
    {
        base.Initialize(ctx, factory);
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= _ctx.DodgeTime.x + _ctx.DodgeTime.y){
            SwitchState(_factory.GetSubState(FighterStates.Idle));
        }
    }

    public override void EnterState()
    {
        _isFirstTime = true;
        _currentFrame = 0;
        
        AnimationClip clip = meshAnimation;
        AnimationClip boxClip = boxAnimation;

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = boxClip;

        float speedVar = AdjustAnimationTime(clip, _ctx.DodgeTime.x + _ctx.DodgeTime.y);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);
        _ctx.ColBoxAnimator.SetFloat("SpeedVar", speedVar);

        _ctx.Animator.PlayInFixedTime("Action");
        _ctx.ColBoxAnimator.PlayInFixedTime("Action");
    }

    public override void ExitState()
    {
        _ctx.IsInvulnerable = false;
    }

    public override void FixedUpdateState()
    {
        if (_currentFrame > _ctx.DodgeTime.x){

            if (_isFirstTime){
                _ctx.IsInvulnerable = true;
                _isFirstTime = false;
            }

            if (_ctx.IsHurt){
                Debug.Log("Script: FighterDodgeState - FixedUpdateState : Attack Dodged");
                _ctx.IsHurt = false;
            }
        }
        
        CheckSwitchState();
        _currentFrame++;
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }
}
