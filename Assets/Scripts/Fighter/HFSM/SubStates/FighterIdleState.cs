using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Idle State", menuName = "FighterStates/Sub/IdleState")]
public class FighterIdleState : ActionConditional
{
    private AnimationData _animation;

    public override void Initialize(IStateMachineRunner ctx, FighterStateFactory factory)
    {
        base.Initialize(ctx, factory);
    }

    public override void CheckSwitchState()
    {
        if (_ctx.AttackInput.Read()){
            SwitchState(_factory.GetSubState(FighterStates.Attack));
        }
        else if (_ctx.DodgeInput.Read() && _ctx.IsGrounded && _ctx.CurrentRootState == FighterStates.Grounded){
            SwitchState(_factory.GetSubState(FighterStates.Dodge));
        }
        else if (_ctx.DashInput.Read() && _ctx.IsGrounded && _ctx.CurrentRootState == FighterStates.Grounded){
            SwitchState(_factory.GetSubState(FighterStates.Dash));
        }
        else if (_ctx.MovementInput.Read() != 0){
            SwitchState(_factory.GetSubState(FighterStates.Walk));
        }  
    }

    public override void EnterState()
    {
        if (_ctx.CurrentRootState == FighterStates.Grounded)
        {
            Debug.Log("Setting up the grounded animation.");
            _animation = animationDict["Grounded"];
        }
        else if (_ctx.CurrentRootState == FighterStates.Airborne)
        {
            _animation = animationDict["Airborne"];
        }
        AnimationClip clip = _animation.meshAnimation;
        AnimationClip boxClip = _animation.boxAnimation;

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
