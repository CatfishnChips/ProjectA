using System;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttemptDefensiveAction : ActionNode
{

    [ReadOnly] private ActionAttack enemyAttack;

    protected override void OnStart()
    {
        // If the enemy is attacking(check this just to be safe) also this way we can access the attack action that the enemy is currently performing.
        if (context.enemyFSM.CurrentState.GetCurrentSubState is FighterAttackState enemyAttackState)
        {
            enemyAttack = enemyAttackState.Action;
        }
    }

    protected override void OnStop()
    {
        enemyAttack = null;
    }

    protected override State OnUpdate()
    {
        float defenseScore = Random.Range(0.0f, 100.0f);
        bool successfulDefensiveAction = context.difficultySettings.DefensiveActionResult(defenseScore);

        if (successfulDefensiveAction) // AI Decided to take a Defensive Action (Options are; Dodge or Counter).
        {
            string actionType;
            Debug.Log("There are total of " + context.hittingAttacks.Count + " attacks that will hit.");
            if (context.hittingAttacks.Count == 0) // If AI doesn't have any attack that will hit the opponent then it has to dodge
            {
                actionType = "Dodge";
            }
            else
            { // If AI has an attack or attacks that will hit then it should decide.
                float actionTypeScore = Random.Range(0.0f, 100f);

                actionType = context.difficultySettings.DefensiveActionType(actionTypeScore);
            }

            if (actionType == "Counter") // AI Decided To Counter.
            {
                List<string> viableAttackOptions = new List<string>();

                foreach (KeyValuePair<string, ActionAttack> attack in context.hittingAttacks) // Get attacks that can counter the opponent.
                {
                    Debug.Log("My Attack: " + attack.Key.ToString() + " Has: " + attack.Value.StartFrames + " Enemy Attack: " + enemyAttack.name + "Has: " + enemyAttack.StartFrames);
                    if (attack.Value.StartFrames < enemyAttack.StartFrames - 2)
                    {
                        viableAttackOptions.Add(attack.Key);
                        Debug.Log("Viable Attacks: " + attack.Key);
                    }
                }

                if(viableAttackOptions.Count > 0)
                {
                    blackboard.choosenDefensiveAction = viableAttackOptions[Random.Range(0, viableAttackOptions.Count)]; // If we have an attack that will Counter, we counter.
                } 
                else
                { 
                    blackboard.choosenDefensiveAction = "Dodge"; // If we don't have an attack that will counter, we will dodge since we decided to perform a successfull defensive action.
                    blackboard.enemyAttackAction = enemyAttack;
                }
                return State.Success;
            }
            else // AI Decided to Dodge.
            {
                blackboard.choosenDefensiveAction = "Dodge";
                blackboard.enemyAttackAction = enemyAttack;
                return State.Success;
            }
        }
        else return State.Failure;

    }
}
