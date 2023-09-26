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
            bool calledPerFrame;

            if (callMethod == RandomCallMethod.OnEachFrame) calledPerFrame = true;
            else calledPerFrame = false;

            Debug.Log("HELLO!");

            float attackScore = Random.Range(0.0f, 100.0f);

            if (context.difficultySettings.AggressionResult(attackScore, calledPerFrame)) // If decided to perform an attack.
            {

                List<string> hittingAttackNames = context.hittingAttacks.Keys.ToList(); // Get the list of all attacks that has a great possibility to make a hit on the opponent and get their name on a string list
                List<string> allAttackNames = context.selfFSM.AttackMoveDict.Keys.ToList(); // Get the list of all attacks and get their name on a string list

                if (!context.selfFSM.ComboListener.isActive) // If not inside a combo phase
                {
                    float attackAccuracyScore = Random.Range(0.0f, 100.0f); // Decide if the AI is going to perform an accurate or inaccurate attack.

                    if (context.difficultySettings.AttackAccuracy(attackAccuracyScore))
                    {
                        blackboard.choosenAgressiveAction = hittingAttackNames[Random.Range(0, hittingAttackNames.Count())];
                        return State.Success;
                    }
                    else
                    {
                        List<string> nonHittingAttackNames = allAttackNames.Except(hittingAttackNames).ToList(); // Find only inaccurate attacks.
                        blackboard.choosenAgressiveAction = hittingAttackNames[Random.Range(0, hittingAttackNames.Count())];
                        return State.Success;
                    }

                }
                else
                {
                    float comboProbability = Random.Range(0.0f, 100.0f);

                    List<string> comboMovesList = context.selfFSM.ComboListener.GetCurrentSearchDict().Keys.ToList();

                    List<string> hittingComboAttacks = hittingAttackNames.Intersect(comboMovesList).ToList();

                    if (context.difficultySettings.ComboResult(comboProbability)) // If proficient enough to know which move to do to keep the combo active.
                    {
                        if(hittingComboAttacks.Count == 0) blackboard.choosenAgressiveAction = allAttackNames[Random.Range(0, allAttackNames.Count())];
                        else blackboard.choosenAgressiveAction = hittingComboAttacks[Random.Range(0, hittingComboAttacks.Count())];
                        return State.Success;
                    }
                    else
                    {
                        List<string> exceptComboMoves = allAttackNames.Except(hittingComboAttacks).ToList();

                        blackboard.choosenAgressiveAction = exceptComboMoves[Random.Range(0, exceptComboMoves.Count())];
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
