using UnityEngine;

public class FighterAttackState : FighterBaseState
{
    private int _startUpTime;
    private int _activeTime;
    private int _recoveryTime;
    private int _currentFrame = 0;

    public FighterAttackState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory, 
    int startUpTime, int activeTime, int recoveryTime):base(currentContext, fighterStateFactory){
        _stateName = "Attack";
        _startUpTime = startUpTime;
        _activeTime = activeTime;
        _recoveryTime = recoveryTime;
    }

    public override void CheckSwitchState()
    {
        if(_currentFrame >= (_startUpTime + _activeTime + _recoveryTime)){
            SwitchState(_factory.Grounded());
        }
    }

    public override void EnterState()
    {
        Debug.Log("ENTERED ATTACK STATE");
    }

    public override void ExitState()
    {
        Debug.Log("EXITED ATTACK STATE");
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState(){
        if (_currentFrame <= _startUpTime){
            Debug.Log("StartUp Phase");
        }
        else if (_currentFrame > _startUpTime && _currentFrame <= _startUpTime + _activeTime){
            Debug.Log("Active Phase");
        }
        else if(_currentFrame > _startUpTime + _activeTime && _currentFrame <= _startUpTime + _activeTime + _recoveryTime){
            Debug.Log("Recovery Phase");
        }
        CheckSwitchState();
        Debug.Log(_currentFrame);
        _currentFrame++;
    }
}
