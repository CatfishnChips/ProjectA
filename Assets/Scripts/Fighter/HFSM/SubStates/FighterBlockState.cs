using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterBlockState : FighterBaseState
{
    private CollisionData _collisionData;
    private ActionAttack _action;
    private Vector2 _velocity;
    private float _drag;

    private int _stun;
    private float _distance;

    public FighterBlockState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override bool CheckSwitchState()
    {
        if (_currentFrame >= _stun){
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
        _currentFrame = 0;
        _collisionData = _ctx.HurtCollisionData;
        _action = _collisionData.action;
        _ctx.IsHurt = false;

        _stun = _action.Block.Stun.stun;
        _distance = _action.Block.Slide.distance;

        _ctx.HealthManager.UpdateHealth(_action.Block.chipDamage);
        
        ActionDefault action = _ctx.ActionDictionary["Block"] as ActionDefault;

        float direction = -Mathf.Sign(_collisionData.hurtbox.Transform.right.x);
        float time = _stun * Time.fixedDeltaTime;
        _drag = _distance / (time * time) * direction;

        _velocity.x = _distance / time; // Initial horizontal velocity;
        _velocity.x *= direction;
        
       // Apply Calculated Variables
        _ctx.Drag = _drag;
        // _ctx.Gravity = 0f;
        _ctx.CurrentMovement = _velocity;

        _ctx.StaminaManager.UpdateBlock(-1);

        if (_stun == 0) return;

        AnimationClip clip = action.meshAnimation;
        AnimationClip colClip = action.boxAnimation;

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = colClip;

        float speedVar = AdjustAnimationTime(clip, _stun);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);
        _ctx.ColBoxAnimator.SetFloat("SpeedVar", speedVar);

        _ctx.Animator.PlayInFixedTime("Action");
        _ctx.ColBoxAnimator.PlayInFixedTime("Action");
    }

    public override void ExitState()
    {
        // _ctx.Gravity = 0f;
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
