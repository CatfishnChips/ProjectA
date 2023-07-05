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

    public FighterBlockState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (_ctx.IsHurt && _ctx.StaminaManager.CanBlock){
            SwitchState(_factory.Block());
        }

        if (_currentFrame >= _action.BlockStun){
            SwitchState(_factory.Idle());
        }
    }

    public override void EnterState()
    {
        _currentFrame = 0;
        _collisionData = _ctx.HurtCollisionData;
        _action = _collisionData.action;
        _ctx.IsHurt = false;

        if (_action.Knockback!= 0){
            float direction = -Mathf.Sign(_collisionData.hurtbox.Transform.right.x);
            float time = _action.BlockStun * Time.fixedDeltaTime;
            _ctx.Drag = _action.Knockback / (time * time) * direction;

            _velocity.x = _action.Knockback / time; // Initial horizontal velocity;
            _velocity.x *= direction;
        }
        
        _ctx.CurrentMovement = _velocity;

        _ctx.StaminaManager.UpdateStamina();

        if (_action.BlockStun == 0) return;

        ActionDefault action = _ctx.ActionDictionary["Block"] as ActionDefault;
        AnimationClip clip = action.meshAnimation;

        _ctx.AnimOverrideCont["Block"] = clip;

        float speedVar = AdjustAnimationTime(clip, _action.BlockStun);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);

        _ctx.Animator.Play("Block");
        _ctx.ColBoxAnimator.Play("Idle");
    }

    public override void ExitState()
    {
        _ctx.Gravity = 0f;
        _ctx.Drag = 0f;
        _ctx.CurrentMovement = Vector2.zero;
    }

    public override void FixedUpdateState()
    {   
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
