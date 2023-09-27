using System;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class UpdateContext : ActionNode
{
    protected override void OnStart(){}

    protected override void OnStop(){}

    protected override State OnUpdate()
    {
        context.distanceToOpponent.x = Math.Abs(context.enemyFSM.transform.position.x - context.selfFSM.transform.position.x);
        context.distanceToOpponent.y = Math.Abs(context.enemyFSM.transform.position.y - context.selfFSM.transform.position.y);
        context.hittingAttacks = CalcHittingAttacks();
        // Debug.Log("Update Context X distance: " + context.distanceToOpponent.x);
        // Debug.Log("Update Context Y distance: " + context.distanceToOpponent.x);
        return State.Success;
    }

    private Dictionary<string, ActionAttack> CalcHittingAttacks()
    {
        Dictionary<string, ActionAttack> hittingAttacks = new Dictionary<string, ActionAttack>();

        foreach (KeyValuePair<string, ActionAttack> attack in context.selfFSM.AttackMoveDict)
        {
            // Debug.Log("Name: " + attack.Value.name);
            // Debug.Log("Offset X: " + attack.Value.HitboxOffset.x);
            // Debug.Log("Offset Y: " + attack.Value.HitboxOffset.y);
            // Debug.Log("Size X: " + attack.Value.HitboxSize.x);
            // Debug.Log("Size Y: " + attack.Value.HitboxSize.y);
            // Debug.Log("Furthest X: " + (context.attackBoxFlexibilityMargin + attack.Value.HitboxOffset.x + attack.Value.HitboxSize.x));
            // Debug.Log("Closest X: " + (context.attackBoxFlexibilityMargin - attack.Value.HitboxOffset.x - attack.Value.HitboxSize.x));
            // Debug.Log("Distance: " + context.distanceToOpponent);
            // Debug.Log("**********************************");
            // This calculation takes hitbox's size into account and achieves the result by calculating if the player's transform position is within the hitbox.
            // This calculation does not take Y position into account. It take a point from the enemy for referance and trying to find if the point is within the box.
            // Therefore Y calculations might be misleading. 
            // SOLUTION: Implement a Box class for chacking intersecitons and other things.
            if (context.distanceToOpponent.x < (attack.Value.HitboxOffset.x + attack.Value.HitboxSize.x + context.attackBoxFlexibilityMargin) &&
                context.distanceToOpponent.x > (attack.Value.HitboxOffset.x - attack.Value.HitboxSize.x - context.attackBoxFlexibilityMargin ))
                {
                    // Debug.Log("Found an Attack that will hit: " + attack.Value.name);
                    if (!hittingAttacks.ContainsKey(attack.Key))
                    {
                        hittingAttacks[attack.Key] = attack.Value;
                    }
                }
            else
                {
                    //Debug.Log("Couldn't find any attack.");
                }
        }
        return hittingAttacks;
    }

}
