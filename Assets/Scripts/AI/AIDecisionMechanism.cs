using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIDecisionMechanism
{

    private AIDifficultySettings _aiDifficulty;

    private FighterStateMachine _selfCtx;
    private FighterStateMachine _enemyCtx;

    private List<string> fighterAttacks;

    private float _optimalDistance;


    public AIDecisionMechanism(AIDifficultySettings difficulty, FighterStateMachine selfStateMachine, FighterStateMachine enemyStateMachine)
    {
        _aiDifficulty = difficulty;
        _selfCtx = selfStateMachine;
        _enemyCtx = enemyStateMachine;
    }

    public string PerformAggresiveAciton()
    {

        float attackProbabilty = Random.Range(0.0f, 100.0f);

        if (_aiDifficulty.AggressionResult(attackProbabilty)) // If decided to perform an attack.
        {
            if (!_selfCtx.ComboListener.isActive) // If not inside a combo phase
            {
                List<string> attackMovesList = _selfCtx.AttackMoveDict.Keys.ToList();

                return attackMovesList[Random.Range(0, attackMovesList.Count())];

            }
            else
            {
                float comboProbability = Random.Range(0.0f, 100.0f);

                if (_aiDifficulty.ComboResult(comboProbability)) // If proficient enough to know which move to do to keep the combo active.
                {
                    List<string> comboMovesList = _selfCtx.ComboListener.GetCurrentSearchDict().Keys.ToList();

                    return comboMovesList[Random.Range(0, comboMovesList.Count())];
                }
                else
                {
                    List<string> attackMovesList = _selfCtx.AttackMoveDict.Keys.ToList();

                    return attackMovesList[Random.Range(0, attackMovesList.Count())];
                }
                
            }
            
        }
        else
        {

            return "Fail";

        }
            
    }

}
