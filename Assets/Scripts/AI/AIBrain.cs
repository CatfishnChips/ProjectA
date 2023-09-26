//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

//public abstract class AIBrain : MonoBehaviour
//{

//    [SerializeField] private AIDifficultySettings _difficultySettings;
//    [Tooltip("Enlarges the hitboxes to allow AI to take action while the hitbox is not precisely hitting the opponent.")]
//    [SerializeField] private float _attackBoxFlexibilityMargin;
//    [SerializeField] private float _distanceMargin;
//    [SerializeField] private AIPositionMethod _optimalDistanceMethod;

//    protected delegate void NoParamVoidFunctions();
//    protected NoParamVoidFunctions nonAttackingStateFuncitons;
//    protected NoParamVoidFunctions attackingStateFuncitons;

//    private AIDecisionMechanism _decisionMechanism;

//    private FighterStateMachine _ctxSelf;
//    private Dictionary<string, ActionAttack> _hittingAttacks;

//    private FighterStateMachine _ctxEnemy;
//    private Transform _tfEnemy;
//    private ActionAttack _previousEnemyAttack;
//    private ActionAttack _currentEnemyAttack;

//    private System.Type _currentState;
//    private System.Type _previousState;
//    private bool _stateChange;
//    private bool _attackMoveEnded; // When an attack Move is finished, this bool gets set to true by an event to let the script know, that next desicion for attack move can be made.
//    private bool _drivenByContext; // This boolean may need to be changed to an enum later on. It indicates whether the fixed update method of the AI is driven by it's ctx FighterState or not.

//    private Vector2 _distanceToOpponent;
//    private float _optimalDistance;

//    public Dictionary<string, ActionAttack> HittingAttacks { get => _hittingAttacks; set => _hittingAttacks = value; }

//    // Move Event expects a float between -1 and 1.

//    # region MonoBehavior Functions

//    void OnDisable()
//    {
//        OnDisableFunciton();
//    }

//    void Start()
//    {
//        StartFunction();
//    }

//    void FixedUpdate()
//    {
//        FixedUpdateFunciton();
//    }

//    # endregion

//    protected virtual void OnDisableFunciton()
//    {
//        EventManager.Instance.P2FighterAttackEnded -= AttackMoveEnded;
//    }

//    // Start is called before the first frame update
//    protected virtual void StartFunction()
//    {
//        EventManager.Instance.P2FighterAttackEnded += AttackMoveEnded;

//        nonAttackingStateFuncitons += NonAttackingStateMovement;
//        nonAttackingStateFuncitons += NonAttackingStateDesicion;
//        attackingStateFuncitons += AttackingStateDesicion;

//        _ctxSelf = transform.GetComponent<FighterStateMachine>();

//        GameObject enemyGO = GameObject.FindWithTag("Player");
//        _tfEnemy = enemyGO.GetComponent<Transform>();
//        _ctxEnemy = enemyGO.GetComponent<FighterStateMachine>();

//        _decisionMechanism = new AIDecisionMechanism(this, _difficultySettings, _ctxSelf, _ctxEnemy);

//        EventManager.Instance.P2FighterAttackEnded += AttackMoveEnded;

//        _optimalDistance = CalcOptimalXDistance(_optimalDistanceMethod);
//        Debug.Log("Optimal Distance is: " + _optimalDistance);

//        _drivenByContext = true;
//        _stateChange = false;
//        _attackMoveEnded = true;

//    }


//    protected virtual void FixedUpdateFunciton()
//    {

//        //_currentState = _ctxSelf.CurrentState.GetType();

//        //_stateChange = (_currentState != _previousState);

//        //_previousState = _currentState;

//        _distanceToOpponent.x = Math.Abs(_tfEnemy.position.x - _ctxSelf.transform.position.x);
//        _distanceToOpponent.y = Math.Abs(_tfEnemy.position.y - _ctxSelf.transform.position.y);
//        Debug.Log("X distance is: " + _distanceToOpponent.x);

//        _hittingAttacks = CalcHittingAttacks(); // At each fixed update calcualte which attack are going to hit the oppenent when performed.

//        Debug.Log(_ctxSelf.CurrentSubState.ToString());

//        if (_drivenByContext)
//        {

//            switch (_ctxSelf.CurrentSubState)
//            {
//                case FighterStates.Idle:
//                case FighterStates.Walk:
//                    NonAttackingStateMovement();
//                    NonAttackingStateDesicion();
//                    break;
//                case FighterStates.Attack:
//                    if (_attackMoveEnded && _ctxSelf.ValidAttackInputInterval)
//                    {
//                        AttackingStateDesicion();
//                        _attackMoveEnded = false;
//                    }
//                    break;

//            }
//        }
//        else
//        {
//            if (_ctxEnemy.CurrentSubState != FighterStates.Attack)
//            {
//                _drivenByContext = true;
//            }
//        }
//    }

//    private void NonAttackingStateDesicion()
//    {

//        switch (_ctxEnemy.CurrentSubState)
//        {
//            case FighterStates.Idle:
//                string attackMove = _decisionMechanism.AttemptAggresiveAciton("Update");
//                if (attackMove != "Fail")
//                {
//                    EventManager.Instance.P2AttackMove?.Invoke(attackMove);
//                }
//                break;

//            case FighterStates.Attack:

//                if (_ctxEnemy.CurrentState.GetCurrentSubState is FighterAttackState enemyAttackState)
//                {
//                    _currentEnemyAttack = enemyAttackState.Action;
//                }

//                string actionMove = _decisionMechanism.TakeDefensiveAction(_currentEnemyAttack);
//                if (actionMove != "Fail")
//                {
//                    if (actionMove == "Dodge")
//                    {
//                        _drivenByContext = false;
//                        StartCoroutine(DelayInvoke(EventManager.Instance.P2Swipe, new Vector2(1.0f, 1.0f), _currentEnemyAttack.StartFrames - 2));
//                    }
//                    else
//                    {
//                        EventManager.Instance.P2AttackMove?.Invoke(actionMove);
//                    }
//                }

//                break;
//        }
//    }

//    private void NonAttackingStateMovement()
//    {

//        if (_distanceToOpponent.x < _optimalDistance - _distanceMargin)
//        {
//            Debug.Log("So close");
//            EventManager.Instance.P2Move(-1.0f * _ctxSelf.FaceDirection);
//        }
//        else if (_distanceToOpponent.x > _optimalDistance + _distanceMargin)
//        {
//            Debug.Log("So far");
//            EventManager.Instance.P2Move(_ctxSelf.FaceDirection);
//        }
//        else
//        {
//            Debug.Log("At Optimal Range");
//            EventManager.Instance.P2Move(0.0f);
//        }

//    }

//    private void AttackingStateDesicion()
//    {
//        string attackMove = _decisionMechanism.AttemptAggresiveAciton("Fixed");
//        // Debug.Log(attackMove);
//        if (attackMove != "Fail")
//        {
//            EventManager.Instance.P2AttackMove?.Invoke(attackMove);
//        }
//    }

//    private Dictionary<string, ActionAttack> CalcHittingAttacks()
//    {
//        Dictionary<string, ActionAttack> hittingAttacks = new Dictionary<string, ActionAttack>();

//        foreach (KeyValuePair<string, ActionAttack> attack in _ctxSelf.AttackMoveDict)
//        {

//            // This calculation takes hitbox's size into account and achieves the result by calculating if the player's transform position is within the hitbox.
//            if ((_distanceToOpponent.x + attack.Value.HitboxSize.x + _attackBoxFlexibilityMargin) > attack.Value.HitboxOffset.x &&
//               (_distanceToOpponent.x - attack.Value.HitboxSize.x - _attackBoxFlexibilityMargin) < attack.Value.HitboxOffset.x &&
//               (_distanceToOpponent.y + attack.Value.HitboxSize.y + _attackBoxFlexibilityMargin) > attack.Value.HitboxOffset.y &&
//               (_distanceToOpponent.y - attack.Value.HitboxSize.y - _attackBoxFlexibilityMargin) < attack.Value.HitboxOffset.y)
//            {
//                //Debug.Log("Name: " + attack.Value.name);
//                //Debug.Log("Offset X: " + attack.Value.HitboxOffset.x);
//                //Debug.Log("Offset Y: " + attack.Value.HitboxOffset.y);
//                //Debug.Log("Size X: " + attack.Value.HitboxSize.x);
//                //Debug.Log("Size Y: " + attack.Value.HitboxSize.y);
//                //Debug.Log("Distance: " + _distanceToOpponent);
//                //Debug.Log("Found an Attack that will hit: " + attack.Value.name);
//                if (!hittingAttacks.ContainsKey(attack.Key))
//                {
//                    hittingAttacks[attack.Key] = attack.Value;
//                }
//            }
//            else
//            {
//                //Debug.Log("Couldn't find any attack.");
//            }
//        }
//        return hittingAttacks;
//    }


//    public void AttackMoveEnded()
//    {
//        // Debug.Log(_ctxEnemy.Player.ToString() + "Attack Move Ended!");
//        _attackMoveEnded = true;
//    }

//    private IEnumerator DelayInvoke(UnityAction<Vector2> actionEvent, Vector2 direction, int frames)
//    {
//        for (int i = 0; i < frames; i++)
//        {
//            yield return new WaitForFixedUpdate();
//        }
//        actionEvent?.Invoke(direction);
//    }

//    private float CalcOptimalXDistance(AIPositionMethod method)
//    {

//        switch (method)
//        {
//            case AIPositionMethod.ArithmeticMean:
//                float xTotal = 0.0f;
//                Debug.Log("HELLO");
//                foreach (KeyValuePair<string, ActionAttack> attack in _ctxSelf.GroundedAttackMoveDict)
//                {
//                    Debug.Log("Attack Name: " + attack.Key + ", Attack Distance: " + attack.Value.HitboxOffset.x);
//                    xTotal += attack.Value.HitboxOffset.x;
//                }
//                return xTotal / _ctxSelf.GroundedAttackMoveDict.Count;
//            default:
//                return 10.0f; // Place Holder
//        }
//    }
//}
