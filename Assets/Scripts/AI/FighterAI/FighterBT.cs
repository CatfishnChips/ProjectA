using UnityEngine;
using BehaviourTree;
using Tree = BehaviourTree.Tree;
using System.Collections.Generic;

[RequireComponent(typeof(FighterStateMachine))]

public class FighterBT : Tree
{
    [SerializeField] private AIDifficultySettings _difficultySettings;
    [Tooltip("Enlarges the hitboxes to allow AI to take action while the hitbox is not precisely hitting the opponent.")]
    [SerializeField] private float _attackBoxFlexibilityMargin;
    [SerializeField] private float _distanceMargin;
    [SerializeField] private AIPositionMethod _optimalDistanceMethod;

    private AIDecisionMechanism _decisionMechanism;

    private FighterStateMachine _contextSelf;
    private Dictionary<string, ActionAttack> _hittingAttacks;

    private FighterStateMachine _contextEnemy;
    private Transform _tfEnemy;
    private ActionAttack _previousEnemyAttack;
    private ActionAttack _currentEnemyAttack;

    private System.Type _currentState;
    private System.Type _previousState;
    private bool _stateChange;
    private bool _attackMoveEnded; // When an attack Move is finished, this bool gets set to true by an event to let the script know, that next desicion for attack move can be made.
    private bool _drivenByContext; // This boolean may need to be changed to an enum later on. It indicates whether the fixed update method of the AI is driven by it's ctx FighterState or not.

    private Vector2 _distanceToOpponent;
    private float _optimalDistance;


    public Dictionary<string, ActionAttack> HittingAttacks { get => _hittingAttacks; set => _hittingAttacks = value; }
    public FighterStateMachine ContextSelf { get => _contextSelf; }
    public FighterStateMachine ContextEnemy { get => _contextEnemy; }
    public Transform TfEnemy { get => _tfEnemy; }
    public float DistanceMargin { get => _distanceMargin; }

    protected override Node SetupTree()
    {
        Node root = new TaskMovement(this);

        return root;
    }

    protected override void SetupTreeFields(){
        _contextSelf = transform.GetComponent<FighterStateMachine>();

        GameObject enemyGO = GameObject.FindWithTag("Player");
        _tfEnemy = enemyGO.GetComponent<Transform>();
        _contextEnemy = enemyGO.GetComponent<FighterStateMachine>();

        _decisionMechanism = new AIDecisionMechanism(this, _difficultySettings, _contextSelf, _contextEnemy);

        _optimalDistance = CalcOptimalXDistance(_optimalDistanceMethod);
        Debug.Log("Optimal Distance is: " + _optimalDistance);
    }

    private float CalcOptimalXDistance(AIPositionMethod method)
    {

        switch (method)
        {
            case AIPositionMethod.ArithmeticMean:
                float xTotal = 0.0f;
                Debug.Log("HELLO");
                foreach (KeyValuePair<string, ActionAttack> attack in _contextSelf.GroundedAttackMoveDict)
                {
                    Debug.Log("Attack Name: " + attack.Key + ", Attack Distance: " + attack.Value.HitboxOffset.x);
                    xTotal += attack.Value.HitboxOffset.x;
                }
                return xTotal / _contextSelf.GroundedAttackMoveDict.Count;
            default:
                return 10.0f; // Place Holder
        }
    }
}
