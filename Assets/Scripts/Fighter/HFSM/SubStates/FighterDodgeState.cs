using UnityEngine;

public class FighterDodgeState : FighterBaseState
{
    protected ActionDefault _action;
    protected bool _isFirstTime = true;

    public FighterDodgeState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override bool CheckSwitchState()
    {
        if (_currentFrame >= _ctx.DodgeTime.x + _ctx.DodgeTime.y){
            if(IdleStateSwitchCheck()) return true; 
            
            SwitchState(_factory.GetSubState(FighterSubStates.Idle));
            return true;
            
        }
        else{
            return false;
        }
    }

    public override void EnterState()
    {
        _isFirstTime = true;
        _currentFrame = 0;
        
        if (_ctx.IsGrounded) 
        {
            _action = _ctx.ActionDictionary["Dodge"] as ActionDefault;
        }
        else 
        {
            _action = _ctx.ActionDictionary["Dodge"] as ActionDefault;
        }
        AnimationClip clip = _action.meshAnimation;
        AnimationClip boxClip = _action.boxAnimation;

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
