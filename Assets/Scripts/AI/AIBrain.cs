using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIBrain : MonoBehaviour
{

    [SerializeField] private AIDifficultySettings _difficultySettings;
    [Tooltip("Enlarges the hitboxes to allow AI to take action while the hitbox is not precisely hitting the opponent.")]
    [SerializeField] private float _attackBoxFlexibilityMargin;

    private AIDecisionMechanism _decisionMechanism;

    private FighterStateMachine _ctxSelf;
    private Dictionary<string, ActionAttack> _hittingAttacks;

    private FighterStateMachine _ctxEnemy;
    private Transform _tfEnemy;
    private ActionAttack _previousEnemyAttack;
    private ActionAttack _currentEnemyAttack;

    private System.Type _currentState;
    private System.Type _previousState;
    private bool _stateChange;
    private bool _attackMoveEnded; // When an attack Move is finished, this bool gets set to true by an event to let the script know, that next desicion for attack move can be made.
    private bool _drivenByContext; // This boolean may need to be changed to an enum later on.

    private Vector2 _distanceToOpponent; 
    private Vector2 _desiredPosition;
    private float _optimalDistance;

    public Dictionary<string, ActionAttack> HittingAttacks { get => _hittingAttacks; set => _hittingAttacks = value; }

    // Move Event expects a float between -1 and 1.

    void OnDisable()
    {
        EventManager.Instance.P2FighterAttackEnded -= AttackMoveEnded;
    }

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.P2FighterAttackEnded += AttackMoveEnded;

        _ctxSelf = transform.GetComponent<FighterStateMachine>();

        GameObject enemyGO = GameObject.FindWithTag("Player");
        _tfEnemy = enemyGO.GetComponent<Transform>();
        _ctxEnemy = enemyGO.GetComponent<FighterStateMachine>();

        _decisionMechanism = new AIDecisionMechanism(this, _difficultySettings, _ctxSelf, _ctxEnemy);

        EventManager.Instance.P2FighterAttackEnded += AttackMoveEnded;

        _drivenByContext = true;
        _stateChange = false;
        _attackMoveEnded = true;

    }


    void FixedUpdate()
    {

        //_currentState = _ctxSelf.CurrentState.GetType();

        //_stateChange = (_currentState != _previousState);

        //_previousState = _currentState;

        _distanceToOpponent.x = Math.Abs(_tfEnemy.position.x - _ctxSelf.transform.position.x);
        _distanceToOpponent.y = Math.Abs(_tfEnemy.position.y - _ctxSelf.transform.position.y);

        _hittingAttacks = CalcHittingAttacks(); // At each fixed update calcualte which attack are going to hit the oppenent when performed.

        Debug.Log(_ctxSelf.CurrentSubState.ToString());

        if (_drivenByContext) { 
            switch (_ctxSelf.CurrentSubState)
            {
                case FighterStates.Idle:
                    NonAttackingStateDesicion();
                    break;
                case FighterStates.Attack:
                    if (_attackMoveEnded && _ctxSelf.ValidAttackInputInterval)
                    {
                        AttackingStateDesicion();
                        _attackMoveEnded = false;
                    }
                    break;

            }
        }
        else
        {
            if(_ctxEnemy.CurrentSubState != FighterStates.Attack)
            {
                _drivenByContext = true;
            }
        }
    }

    void Update()
    {
        
    }

    private void NonAttackingStateDesicion()
    {
        
        switch (_ctxEnemy.CurrentSubState)
        {
            case FighterStates.Idle:
                string attackMove = _decisionMechanism.AttemptAggresiveAciton("Update");
                if (attackMove != "Fail")
                {
                    EventManager.Instance.P2AttackMove?.Invoke(attackMove);
                }
                break;

            case FighterStates.Attack:

                if(_ctxEnemy.CurrentState.GetCurrentSubState is FighterAttackState enemyAttackState)
                {
                    _currentEnemyAttack = enemyAttackState.Action;
                }

                string actionMove = _decisionMechanism.TakeDefensiveAction(_currentEnemyAttack);
                if(actionMove != "Fail")
                {
                    if(actionMove == "Dodge")
                    {
                        _drivenByContext = false;
                        StartCoroutine(DelayInvoke(EventManager.Instance.P2Swipe, new Vector2(1.0f, 1.0f), _currentEnemyAttack.StartFrames - 2));
                    }
                    else
                    {
                        EventManager.Instance.P2AttackMove?.Invoke(actionMove);
                    }
                }

                break;
        }   
    }

    private void AttackingStateDesicion()
    {
        string attackMove = _decisionMechanism.AttemptAggresiveAciton("Fixed");
        // Debug.Log(attackMove);
        if (attackMove != "Fail")
        {
            EventManager.Instance.P2AttackMove?.Invoke(attackMove);
        }
    }

    private Dictionary<string, ActionAttack> CalcHittingAttacks()
    {
        Dictionary<string, ActionAttack> hittingAttacks = new Dictionary<string, ActionAttack>();

        foreach (KeyValuePair<string, ActionAttack> attack in _ctxSelf.AttackMoveDict)
        {
            
            // This calculation takes hitbox's size into account and achieves the result by calculating if the player's transform position is within the hitbox.
            if ((_distanceToOpponent.x + attack.Value.HitboxSize.x + _attackBoxFlexibilityMargin) > attack.Value.HitboxOffset.x && 
               (_distanceToOpponent.x - attack.Value.HitboxSize.x - _attackBoxFlexibilityMargin) < attack.Value.HitboxOffset.x &&
               (_distanceToOpponent.y + attack.Value.HitboxSize.y + _attackBoxFlexibilityMargin) > attack.Value.HitboxOffset.y &&
               (_distanceToOpponent.y - attack.Value.HitboxSize.y - _attackBoxFlexibilityMargin) < attack.Value.HitboxOffset.y)
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


    public void AttackMoveEnded()
    {
        // Debug.Log(_ctxEnemy.Player.ToString() + "Attack Move Ended!");
        _attackMoveEnded = true;
    }

    private IEnumerator DelayInvoke(UnityAction<Vector2> actionEvent, Vector2 direction, int frames)
    {
        for(int i = 0; i < frames; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        Debug.Log("NOW!");
        actionEvent?.Invoke(direction);
    }
}
