using System;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class UpdateContext : ActionNode
{
    protected override void OnStart()
    {
        context.optimalDistance = CalcOptimalXDistance(context.optimalDistanceMethod);
        Debug.Log("Start: " + context.optimalDistance);
    }

    protected override void OnStop(){}

    protected override State OnUpdate()
    {
        context.hittingAttacks = CalcHittingAttacks();
        context.distanceToOpponent.x = Math.Abs(context.enemyFSM.transform.position.x - context.selfFSM.transform.position.x);
        context.distanceToOpponent.y = Math.Abs(context.enemyFSM.transform.position.y - context.selfFSM.transform.position.y);
        return State.Success;
    }

    private Dictionary<string, ActionAttack> CalcHittingAttacks()
    {
        Dictionary<string, ActionAttack> hittingAttacks = new Dictionary<string, ActionAttack>();

        foreach (KeyValuePair<string, ActionAttack> attack in context.selfFSM.AttackMoveDict)
            {

                // This calculation takes hitbox's size into account and achieves the result by calculating if the player's transform position is within the hitbox.
                if ((context.distanceToOpponent.x + attack.Value.HitboxSize.x + context.attackBoxFlexibilityMargin) > attack.Value.HitboxOffset.x &&
                    (context.distanceToOpponent.x - attack.Value.HitboxSize.x - context.attackBoxFlexibilityMargin) < attack.Value.HitboxOffset.x &&
                    (context.distanceToOpponent.y + attack.Value.HitboxSize.y + context.attackBoxFlexibilityMargin) > attack.Value.HitboxOffset.y &&
                    (context.distanceToOpponent.y - attack.Value.HitboxSize.y - context.attackBoxFlexibilityMargin) < attack.Value.HitboxOffset.y)
                    {
                        //Debug.Log("Name: " + attack.Value.name);
                        //Debug.Log("Offset X: " + attack.Value.HitboxOffset.x);
                        //Debug.Log("Offset Y: " + attack.Value.HitboxOffset.y);
                        //Debug.Log("Size X: " + attack.Value.HitboxSize.x);
                        //Debug.Log("Size Y: " + attack.Value.HitboxSize.y);
                        //Debug.Log("Distance: " + _distanceToOpponent);
                        //Debug.Log("Found an Attack that will hit: " + attack.Value.name);
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

    private float CalcOptimalXDistance(AIPositionMethod method)
    {
        switch (method)
        {
            case AIPositionMethod.ArithmeticMean:
                float xTotal = 0.0f;
                foreach (KeyValuePair<string, ActionAttack> attack in context.selfFSM.GroundedAttackMoveDict)
                {
                    // Debug.Log("Attack Name: " + attack.Key + ", Attack Distance: " + attack.Value.HitboxOffset.x);
                    xTotal += attack.Value.HitboxOffset.x;
                }
                return xTotal / context.selfFSM.GroundedAttackMoveDict.Count;
            default:
                return 10.0f; // Place Holder
        }
        
    }

}
