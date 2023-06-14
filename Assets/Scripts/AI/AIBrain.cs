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

    // Move Event'i -1 ile 1 arasında bir float bekliyor.

    // Start is called before the first frame update
    void Start()
    {
        _ctxSelf = transform.GetComponent<FighterStateMachine>();

        GameObject enemyGO = GameObject.FindWithTag("Player");
        _tfEnemy = enemyGO.GetComponent<Transform>();
        _ctxEnemy = enemyGO.GetComponent<FighterStateMachine>();

        decisionMechanism = new AIDecisionMechanism(difficultySettings, _ctxSelf, _ctxEnemy);

        _stateChange = false;

    }

    void FixedUpdate()
    {

        _currentState = _ctxSelf.CurrentState.GetType();

        if(_currentState != _previousState) _stateChange = true;

        _previousState = _currentState;
        
        switch(_ctxSelf.CurrentState)
        {
            case FighterGroundedState:
                NonAttackingStateDesicion();
                break;
        }

    }

    void Update()
    {
        
    }

    private void NonAttackingStateDesicion()
    {
        string attackMove = decisionMechanism.PerformAggresiveAciton();
        if(attackMove != "Fail") 
        {
            EventManager.Instance.P2AttackMove?.Invoke(attackMove);
        }
    }
}
