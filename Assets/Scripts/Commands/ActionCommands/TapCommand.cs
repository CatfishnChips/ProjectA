public class TapCommand : ICommand<TapCommandArgs>
{
    public void Execute(TapCommandArgs args)
    {
        switch(args.Player){
            case Player.P1: EventManager.Instance.OnTap(); break;
            case Player.P2: EventManager.Instance.P2OnTap(); break;
        }
    }
}
