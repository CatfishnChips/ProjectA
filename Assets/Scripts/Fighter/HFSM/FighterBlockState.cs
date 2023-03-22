using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterBlockState : FighterBaseState
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private float _currentFrame = 0;
    private Vector2 _velocity;
    private float _animationSpeed;
    private bool _isFirstTime = true;

    public FighterBlockState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (_ctx.IsHurt && _ctx.StaminaManager.CanBlock){
            SwitchState(_factory.Block());
        }

        if (_currentFrame >= _action.HitStun){
            SwitchState(_factory.Idle());
        }
    }

    public override void EnterState()
    {
        _currentFrame = 0;
        _collisionData = _ctx.CollisionData;
        _action = _collisionData.action;
        _ctx.IsHurt = false;

        if (_action.Knockback!= 0){
            _velocity.x = Mathf.Sign(_collisionData.hurtbox.Transform.forward.x) * _action.Knockback;
        }
        
        _ctx.CurrentMovement = _velocity;
        _ctx.Velocity = _ctx.CurrentMovement;
        _ctx.Rigidbody2D.velocity = _ctx.Velocity;

        _ctx.StaminaManager.UpdateStamina();

        if (_action.HitStun == 0) return;

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
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Velocity = _ctx.CurrentMovement;
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
