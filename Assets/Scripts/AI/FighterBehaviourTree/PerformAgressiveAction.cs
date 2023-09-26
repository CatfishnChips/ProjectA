using TheKiwiCoder;

public class PerformAgressiveAction : ActionNode
{

    private State startMethodReturn;

    protected override void OnStart()
    {
        startMethodReturn = StartMethodReturn();
    }

    protected override void OnStop()
    {
        blackboard.choosenAgressiveAction = null;
    }

    protected override State OnUpdate()
    {
        if(context.selfFSM.CurrentSubState != FighterStates.Attack || blackboard.selfAttackEnded) return State.Success;
        else return startMethodReturn;
    }

    private State StartMethodReturn()
    {
        string choosenAttack = blackboard.choosenAgressiveAction;
        if(choosenAttack == null) return State.Failure; // This is just a safe check if the AI didn't choose to make an attack the tree should not even execute this node.

        EventManager.Instance.P2AttackMove?.Invoke(choosenAttack);
        return State.Running;
    }
}
