using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Dash State", menuName = "FighterStates/Sub/DashState")]
public class FighterDashState : FighterCancelleableState
{
    public List<AnimationData> Animations;
    public float AnimationSpeed(int index) { return AdjustAnimationTime(Animations[index].meshAnimation, Animations[0].frames); }

    [SerializeField] private int m_cancelFrame;
    [SerializeField] private int m_inputIgnoreFrames;

    public int CancelFrame { get => m_cancelFrame; }
    public int InputIgnoreFrames { get => m_inputIgnoreFrames; }

    private float _direction;
    private float _time;
    private float _initialVelocity;
    private float _drag;

    public override void Initialize(IStateMachineRunner ctx, FighterStateFactory factory)
    {
        base.Initialize(ctx, factory);
    }

    public override void CheckSwitchState()
    {
        base.CheckSwitchState();
        if (_currentFrame >= _ctx.DashTime){
            SwitchState(_factory.GetSubState(FighterStates.Idle));
        }
    }

    public override void EnterState()
    {
        base.EnterState();
        _currentFrame = 0;
        _ctx.IsGravityApplied = false;
        _drag = 0f;

        _ctx.ChainActionGesture = InputGestures.None;

        AnimationClip clip;
        AnimationClip colClip;

        if (_ctx.DashDirection == -1)
        {
            clip = Animations[0].meshAnimation;
            colClip = Animations[0].boxAnimation;
        }
        else
        {
            clip = Animations[1].meshAnimation;
            colClip = Animations[1].boxAnimation;
        }

        _direction = _ctx.DashDirection;
        _time = _ctx.DashTime * Time.fixedDeltaTime;

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
