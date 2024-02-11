using System;
using System.Collections.Generic;
using System.Linq;
using TheKiwiCoder;
using Random = UnityEngine.Random;

public class AttemptComboAttack : ActionNode
{
    List<ActionAttack> comboAttacks;
    List<ActionAttack> nonComboAttacks;

    public bool hasAttackEnded;

    public State comboAttemptResult; 

    protected override void OnStart()
    {
        comboAttemptResult = ComboAttemptResult();
    }

    protected override void OnStop()
    {
        hasAttackEnded = false;
    }

    protected override State OnUpdate()
    {
        if(blackboard.selfAttackEnded) return State.Failure;
        else return comboAttemptResult;

    }

    private State ComboAttemptResult()
    {
        try{
            foreach(ComboMoveSpecs comboMoveSpec in context.selfFSM.ComboListener.GetCurrentSearchDict().Values)
            {
                comboAttacks.Add(comboMoveSpec.theMove);
            }
        }
        catch(NullReferenceException){
            comboAttacks = null;
            return State.Running;
        }

        nonComboAttacks = context.selfFSM.AttackMoveDict.Values.ToList().Except(comboAttacks).ToList();

        int agressionCheck = Random.Range(0, 100);

        if(context.difficultySettings.AggressionResult(agressionCheck, false)){

            int comboProfCheck = Random.Range(0, 100);

            if(context.difficultySettings.ComboResult(comboProfCheck)){
                if(comboAttacks is not null && comboAttacks.Count > 0) 
                {
                    blackboard.choosenAgressiveAction = comboAttacks[Random.Range(0, comboAttacks.Count)];
                    return State.Success;
                }
                else
                {
                    return State.Running;
                } 
            }
            else
            {
                blackboard.choosenAgressiveAction = nonComboAttacks[Random.Range(0, nonComboAttacks.Count)];
                return State.Success;
            }
        }
        else return State.Running;
    }
}
