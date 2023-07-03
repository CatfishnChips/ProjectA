using UnityEngine;

public class AIBrain : MonoBehaviour
{

    public AIDifficultySettings difficultySettings;

    private AIDecisionMechanism decisionMechanism;

    private FighterStateMachine _ctxSelf;

    private int _nonAttackingStateFrameCounter = 0;

    private FighterStateMachine _ctxEnemy;
    private Transform _tfEnemy;

    private System.Type _currentState;
    private System.Type _previousState;
    private bool _stateChange;
    private bool _attackMoveEnded; // When an attack Move is finished, this bool gets set to true by an event to let the script know, that next desicion for attack move can be made.

    // Move Event'i -1 ile 1 arasında bir float bekliyor.    

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

        decisionMechanism = new AIDecisionMechanism(difficultySettings, _ctxSelf, _ctxEnemy);

        EventManager.Instance.P2FighterAttackEnded += AttackMoveEnded;

        _stateChange = false;
        _attackMoveEnded = true;

    }


    void FixedUpdate()
    {

        //_currentState = _ctxSelf.CurrentState.GetType();

        //_stateChange = (_currentState != _previousState);

        //_previousState = _currentState;
        
        switch(_ctxSelf.CurrentState)
        {
            case FighterGroundedState:
                NonAttackingStateDesicion();
                break;
            case FighterAttackState:
                if (_attackMoveEnded) AttackingStateDesicion();
                _attackMoveEnded = false;
                break;

        }

    }

    void Update()
    {
        
    }

    private void NonAttackingStateDesicion()
    {
        string attackMove = decisionMechanism.AttemptAggresiveAciton("Update");
        if(attackMove != "Fail") 
        {
            EventManager.Instance.P2AttackMove?.Invoke(attackMove);
        }
    }

    private void AttackingStateDesicion()
    {
        string attackMove = decisionMechanism.AttemptAggresiveAciton("Fixed");
        if (attackMove != "Fail")
        {
            EventManager.Instance.P2AttackMove?.Invoke(attackMove);
        }
    }

    public void AttackMoveEnded()
    {
        _attackMoveEnded = true;
    }
}
