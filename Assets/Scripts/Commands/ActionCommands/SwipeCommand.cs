public class SwipeCommand : ICommand<SwipeCommandArgs>
{
    public void Execute(SwipeCommandArgs args)
    {
        switch(args.Player){
            case Player.P1: EventManager.Instance.Swipe(args.Direction); break;
            case Player.P2: EventManager.Instance.P2Swipe(args.Direction); break;
        }
    }
}
