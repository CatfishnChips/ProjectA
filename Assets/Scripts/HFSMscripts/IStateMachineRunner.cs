using UnityEngine;

public interface IStateMachineRunner
{
    public void SwitchState(StateMachineBaseState state);
}
