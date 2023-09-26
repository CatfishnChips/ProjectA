using TheKiwiCoder;

public class CheckSelfState : ActionNode
{
    public FighterStates fighterState;

    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        if(fighterState.HasFlag(context.selfFSM.CurrentSubState)) return State.Success;
        else return State.Failure;
    }
}
