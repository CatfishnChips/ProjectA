using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterKnockupState : FighterBaseState
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private float _currentFrame = 0;
    private Vector2 _velocity;
    private float _animationSpeed;
    private bool _isFirstTime = true;

    public FighterKnockupState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (_ctx.IsHurt){
            SwitchState(_factory.Stunned());
        }

        if (_currentFrame >= _action.KnockupStun){   
            FighterBaseState state;         
            
            // Knockup always transitions to Knockdown state.
            if (_action.KnockdownStun > 0){
                state = _factory.Knockdown();
            }
            else{
                state = _factory.Idle();
            }
            SwitchState(state);
        }
    }

    public override void EnterState()
    {
        _currentFrame = 0;
        _collisionData = _ctx.CollisionData;
        _action = _collisionData.action;
        _ctx.IsHurt = false;

        _velocity = Vector2.zero;

        // Note: Velocity calculations are handled here due to vertical and horizontal movemenents' requirement of blending together. Substates are used for adjusting animations.

        if (_action.Knockup != 0){
            float _time = _action.KnockupStun * Time.fixedDeltaTime;
            _ctx.Gravity = (-2 * _action.Knockup) / Mathf.Pow(_time, 2);
            _velocity.y = (2 * _action.Knockup) / _time;   
        }

        if (_action.Knockback!= 0){
            _velocity.x = Mathf.Sign(_collisionData.hurtbox.Transform.forward.x) * _action.Knockback;
        }

        _ctx.CurrentMovement = _velocity;
        _ctx.Velocity = _ctx.CurrentMovement;
        _ctx.Rigidbody2D.velocity = _ctx.Velocity;

        if (_action.KnockupStun == 0) return;

        ActionDefault action = _ctx.ActionDictionary["Block"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;

        _ctx.AnimOverrideCont["Block"] = clip;

        float speedVar = AdjustAnimationTime(clip, _action.HitStun);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);

        _ctx.Animator.Play("Block");
        _ctx.ColBoxAnimator.Play("Block");
    }

    public override void ExitState()
    {
    }

    public override void FixedUpdateState()
    {
        _currentFrame++;
        CheckSwitchState();
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }
}
