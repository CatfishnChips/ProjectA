using UnityEngine;

public class FighterWalkState : FighterBaseState
{
    public FighterWalkState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
        _stateName = "Walk";
    }

    public override void CheckSwitchState()
    {
        if(_ctx.Velocity.x == 0){
            SwitchState(_factory.Idle());
        }
    }

    public override void EnterState()
    {
        string prev_anim_state = _ctx.Animator.GetCurrentAnimatorStateInfo(0).ToString();
        _ctx.Animator.SetTrigger("ToMove");
        string current_anim_state = _ctx.Animator.GetCurrentAnimatorStateInfo(0).ToString();
        if(current_anim_state == prev_anim_state){
            _ctx.Animator.Play("MoveBT");
        }
    }

    public override void ExitState()
    {
        _ctx.Animator.SetFloat("Blend", 0f);
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        //_ctx.CharController.Move(_ctx.Velocity * _ctx.MoveSpeed);
        _ctx.Animator.SetFloat("Blend", _ctx.Velocity.x);
        CheckSwitchState();
    }
}
