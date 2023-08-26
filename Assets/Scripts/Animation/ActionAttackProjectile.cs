using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using UnityEditor.PackageManager;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Attack Action", menuName = "ScriptableObject/Action/Projectile")]
public class ActionAttackProjectile : ActionAttack
{   
    public ActionAttackProjectile(){
        m_frameEvents.Add(new FrameEvent(35, (FighterStateMachine ctx, FighterAttackState state) => PoolProjectile(ctx, state)));
    }

    private List<FrameEvent> m_frameEvents = new List<FrameEvent>(){
        new FrameEvent(30, (FighterStateMachine ctx, FighterAttackState state) => Debug.Log("Frame: " + state._currentFrame + " Hello")),
        // new FrameEvent(35, (FighterStateMachine ctx, FighterAttackState state) => 
        // ((FighterStateMachine_Class1)ctx).ProjectileManager.DequeueObject(((FighterStateMachine_Class1)ctx).ProjectileManager.PoolableObjects[0].Prefab, ((FighterStateMachine_Class1)ctx).ProjectileManager.PoolableObjects[0].QueueReference))     
    };

    protected void PoolProjectile(FighterStateMachine ctx, FighterAttackState state){
        var obj = ((FighterStateMachine_Class1)ctx).ProjectileManager.DequeueObject(((FighterStateMachine_Class1)ctx).ProjectileManager.PoolableObjects[0].Prefab, ((FighterStateMachine_Class1)ctx).ProjectileManager.PoolableObjects[0].QueueReference); 
        var projectile = obj.GetComponent<Projectile>();
        projectile.Action = this;
        projectile.Direction = new Vector2(ctx.FaceDirection, 0f);
        //Debug.Log("Script: ActionAttackProjectile - PoolProjectile - Time: " + Time.timeSinceLevelLoad);
        projectile.HitResponder.UpdateData(this);
    }

    protected override List<FrameEvent> Events {get => m_frameEvents;}
     
    public override void FixedUpdateFunction(FighterStateMachine ctx, FighterAttackState state){
        switch(state._actionState)
        {
            case ActionStates.Start:
                if(_firstFrameStartup){
                    ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedS);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedS);
                    ctx.Animator.Play("AttackStart");
                    ctx.ColBoxAnimator.Play("AttackStart");
                    _firstFrameStartup = false;
                }
            break;

            case ActionStates.Active:
                if(_firstFrameActive){
                    ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedA);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedA);
                    ctx.Animator.Play("AttackActive");
                    _firstFrameActive = false;

                    // var obj = ((FighterStateMachine_Class1)ctx).ProjectileManager.DequeueObject(((FighterStateMachine_Class1)ctx).ProjectileManager.PoolableObjects[0].Prefab, ((FighterStateMachine_Class1)ctx).ProjectileManager.PoolableObjects[0].QueueReference);
                    // var pro = obj.GetComponent<Projectile>();
                    // pro.Action = this;
                }
            break;

            case ActionStates.Recovery:
                if(_firstFrameRecovery){
                    ctx.Animator.SetFloat("SpeedVar", state.Action.AnimSpeedR);
                    ctx.ColBoxAnimator.SetFloat("SpeedVar", state.Action.AnimSpeedR);
                    ctx.Animator.Play("AttackRecover");
                    _firstFrameRecovery = false;
                }
            break;
        }
       
        // Invoke events.
        foreach(FrameEvent e in Events){
            if (state._currentFrame == e.Frame){
                e.Event(ctx, state);
            }
        }

        if (ctx.IsHit) ctx.IsHit = false;
        state._currentFrame++;
    }
}
