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
    private float _drag;

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
            float direction = Mathf.Sign(_collisionData.hurtbox.Transform.right.x);
            _drag = (-2 * _action.Knockback) / (_action.BlockStun * _action.BlockStun);
            _drag *= direction;
            _velocity.x = (2 * _action.Knockback) / _action.BlockStun; // Initial horizontal velocity;
            _velocity.x *= direction;
        }
        
        _ctx.CurrentMovement = _velocity;
        _ctx.Velocity = _ctx.CurrentMovement;

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
        _ctx.CurrentMovement = Vector2.zero;
        _ctx.Velocity = _ctx.CurrentMovement;
    }

    public override void FixedUpdateState()
    {   
        _ctx.CurrentMovement = new Vector2(_ctx.CurrentMovement.x + _drag, _ctx.CurrentMovement.y + _ctx.Gravity);
        _ctx.Velocity = _ctx.CurrentMovement;
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
