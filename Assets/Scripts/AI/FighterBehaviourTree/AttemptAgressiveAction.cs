using UnityEngine;
using TheKiwiCoder;
using System.Linq;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class AttemptAgressiveAction : ActionNode
{

    public RandomCallMethod callMethod;

    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        try
        {
            bool callMethodArguement;

            if (callMethod == RandomCallMethod.OnEachFrame) callMethodArguement = true;
            else callMethodArguement = false;

            float attackScore = Random.Range(0.0f, 100.0f);

            if (context.difficultySettings.AggressionResult(attackScore, callMethodArguement)) // If decided to perform an attack.
            {
                if (!context.selfFSM.ComboListener.isActive) // If not inside a combo phase
                {
                    float attackAccuracyScore = Random.Range(0.0f, 100.0f); // Decide if the AI is going to perform an accurate or inaccurate attack.

                    List<string> hittingAttackNames = context.hittingAttacks.Keys.ToList(); // Get the list of all attacks that has a great possibility to make a hit on the opponent and get their name on a string list
                    List<string> attackNames = context.enemyFSM.AttackMoveDict.Keys.ToList(); // Get the list of all attacks and get their name on a string list

                    if (context.difficultySettings.AttackAccuracy(attackAccuracyScore))
                    {
                        blackboard.choosenAgressiveAction = hittingAttackNames[Random.Range(0, hittingAttackNames.Count())];
                        return State.Success;
                    }
                    else
                    {
                        List<string> nonHittingAttackNames = attackNames.Except(hittingAttackNames).ToList(); // Find only inaccurate attacks.
                        blackboard.choosenAgressiveAction = hittingAttackNames[Random.Range(0, hittingAttackNames.Count())];
                        return State.Success;
                    }

                }
                else
                {
                    float comboProbability = Random.Range(0.0f, 100.0f);

                    List<string> comboMovesList = context.selfFSM.ComboListener.GetCurrentSearchDict().Keys.ToList();
                    List<string> hittingAttackNames = context.hittingAttacks.Keys.ToList();

                    List<string> hittingComboAttacks = hittingAttackNames.Intersect(comboMovesList).ToList();

                    if (context.difficultySettings.ComboResult(comboProbability)) // If proficient enough to know which move to do to keep the combo active.
                    {
                        blackboard.choosenAgressiveAction = hittingAttackNames[Random.Range(0, hittingAttackNames.Count())];
                        return State.Success;
                    }
                    else
                    {
                        List<string> attackMovesList = context.selfFSM.AttackMoveDict.Keys.ToList();

                        attackMovesList = attackMovesList.Except(hittingComboAttacks).ToList();

                        blackboard.choosenAgressiveAction = hittingAttackNames[Random.Range(0, hittingAttackNames.Count())];
                        return State.Success;
                    }

                }

            }
            else
            {
                Debug.Log("Don't want to attack");
                return State.Failure;

            }
        }
        catch(IndexOutOfRangeException e)
        {
            Debug.Log("Exception Handled: " + e);
            return State.Failure;
        }
            
    }
}
