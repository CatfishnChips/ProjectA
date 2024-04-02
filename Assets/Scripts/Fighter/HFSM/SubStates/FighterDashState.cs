using UnityEngine;

public class FighterDashState : FighterCancellableState
{
    private ActionDash _action;
    private float _direction;
    private float _time;
    private float _initialVelocity;
    private float _drag;

    public FighterDashState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override bool CheckSwitchState()
    {
        if(base.CheckSwitchState()) return true;
        if (_currentFrame >= _ctx.DashTime){
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
        base.EnterState();
        _currentFrame = 0;
        _ctx.IsGravityApplied = false;
        _drag = 0f;

        _action = _cancellableAction as ActionDash;

        _ctx.ChainActionGesture = InputGestures.None;

        AnimationClip clip;
        AnimationClip colClip;

        clip = _action.meshAnimation;
        colClip = _action.boxAnimation;        

        _direction = _action.direction;
        _time = _action.dashTime * Time.fixedDeltaTime;

        _drag = -2 * _ctx.DashDistance / Mathf.Pow(_time, 2);
        _drag *= _direction;

        _initialVelocity = 2 * _ctx.DashDistance / _time; // Initial horizontal velocity;
        _initialVelocity *= _direction;

        // Apply Calculated Variables
        _ctx.Drag = _drag;
        _ctx.Gravity = 0f;
        _ctx.CurrentMovement = new Vector2(_initialVelocity, _ctx.CurrentMovement.y);

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = colClip;

        // For this action, DashTime variable is used instead of animation's Frame variable.
        float speedVar = AdjustAnimationTime(clip, _ctx.DashTime);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);
        _ctx.ColBoxAnimator.SetFloat("SpeedVar", speedVar);
        
        _ctx.Animator.PlayInFixedTime("Action");
        _ctx.ColBoxAnimator.PlayInFixedTime("Action");
    }

    public override void ExitState()
    {
        base.ExitState();
        _drag = 0f;
        _direction = 0f;
        _time = 0f;
        _initialVelocity = 0f;
        _currentFrame = 0;

        _ctx.IsGravityApplied = true;
        _ctx.Gravity = 0f;
        _ctx.Drag = 0f;
        _ctx.CurrentFrame = 0;
        _ctx.CurrentMovement = Vector2.zero;
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
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
