using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Attack Action", menuName = "ScriptableObject/Action/Projectile")]
public class ActionAttackProjectile : FighterAttackState
{   
    public ActionAttackProjectile(){
        m_frameEvents.Add(new FrameEvent(21, (FighterStateMachine ctx, ActionAttack state) => PoolProjectile(ctx, state)));
    }

    private List<FrameEvent> m_frameEvents = new List<FrameEvent>(){
        //new FrameEvent(30, (FighterStateMachine ctx, ActionAttack state) => Debug.Log("Frame: " + state._currentFrame + " Hello")),
        // new FrameEvent(35, (FighterStateMachine ctx, ActionAttack state) => 
        // ((FighterStateMachine_Class1)ctx).ProjectileManager.DequeueObject(((FighterStateMachine_Class1)ctx).ProjectileManager.PoolableObjects[0].Prefab, ((FighterStateMachine_Class1)ctx).ProjectileManager.PoolableObjects[0].QueueReference))     
    };

    protected void PoolProjectile(FighterStateMachine ctx, ActionAttack state){
        //var obj = ((FighterStateMachine_Class1)ctx).ProjectileManager.DequeueObject(((FighterStateMachine_Class1)ctx).ProjectileManager.PoolableObjects[0].Prefab, ((FighterStateMachine_Class1)ctx).ProjectileManager.PoolableObjects[0].QueueReference); 
        var obj = ((FighterStateMachine)ctx).ProjectileManager.DequeueObject(((FighterStateMachine)ctx).ProjectileManager.PoolableObjects[0].Prefab, ((FighterStateMachine)ctx).ProjectileManager.PoolableObjects[0].QueueReference); 
        var projectile = obj.GetComponent<Projectile>();
        projectile.Action = this;
        projectile.Direction = new Vector2(ctx.FaceDirection, 0f);
        //Debug.Log("Script: ActionAttackProjectile - PoolProjectile - Time: " + Time.timeSinceLevelLoad);
        projectile.Rotate();
        projectile.HitResponder.UpdateData(this);
    }

    protected override List<FrameEvent> Events {get => m_frameEvents;}
     
    public override void FixedUpdateState(){
        switch(_actionState)
        {
            case ActionStates.Start:
                if(_firstFrameStartup){
                    _ctx.Animator.SetFloat("SpeedVar", AnimSpeedS);
                    _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedS);
                    _ctx.Animator.PlayInFixedTime("AttackStart");
                    _ctx.ColBoxAnimator.Play("AttackStart");
                    _firstFrameStartup = false;
                }
            break;

            case ActionStates.Active:
                if(_firstFrameActive){
                    _ctx.Animator.SetFloat("SpeedVar", AnimSpeedA);
                    _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedA);
                    _ctx.Animator.PlayInFixedTime("AttackActive");
                    _firstFrameActive = false;

                    // var obj = ((FighterStateMachine_Class1)_ctx).ProjectileManager.DequeueObject(((FighterStateMachine_Class1)_ctx).ProjectileManager.PoolableObjects[0].Prefab, ((FighterStateMachine_Class1)_ctx).ProjectileManager.PoolableObjects[0].QueueReference);
                    // var pro = obj.GetComponent<Projectile>();
                    // pro.Action = this;
                }
            break;

            case ActionStates.Recovery:
                if(_firstFrameRecovery){
                    _ctx.Animator.SetFloat("SpeedVar", AnimSpeedR);
                    _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedR);
                    _ctx.Animator.PlayInFixedTime("AttackRecover");
                    _firstFrameRecovery = false;
                }
            break;
        }
       
        // Invoke events.
        foreach(FrameEvent e in Events){
            if (_currentFrame == e.Frame){
                e.Event(_ctx, this);
            }
        }

        if (_ctx.IsHit) _ctx.IsHit = false;
        _currentFrame++;
    }
}
