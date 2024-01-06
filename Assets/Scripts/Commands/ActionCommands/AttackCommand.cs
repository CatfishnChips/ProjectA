public class AttackCommand : ICommand<AttackCommandArgs>
{
    public void Execute(AttackCommandArgs args)
    {
        switch(args.Player){
            case Player.P1: EventManager.Instance.AttackMove(args.Gesture); break;
            case Player.P2: EventManager.Instance.P2AttackMove(args.Gesture); break;
        }
    }
}
