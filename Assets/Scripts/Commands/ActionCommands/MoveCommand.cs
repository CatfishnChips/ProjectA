public class MoveCommand : ICommand<MoveCommandArgs>
{
    public void Execute(MoveCommandArgs args)
    {
        switch(args.Player){
            case Player.P1: EventManager.Instance.Move(args.Direction); break;
            case Player.P2: EventManager.Instance.P2Move(args.Direction); break;
        }
    }
}
