using UnityEngine;

public readonly struct AttackCommandArgs
{
    public readonly Player Player;
    public readonly string Gesture;
    public AttackCommandArgs(Player player, string gesture){
        Player = player;
        Gesture = gesture;
    }
}

public readonly struct MoveCommandArgs
{
    public readonly Player Player;
    public readonly float Direction;
    public MoveCommandArgs(Player player, float direction){
        Player = player;
        Direction = direction;
    }
}

public readonly struct SwipeCommandArgs
{
    public readonly Player Player;
    public readonly Vector2 Direction;
    public SwipeCommandArgs(Player player, Vector2 direction){
        Player = player;
        Direction = direction;
    }
}

public readonly struct TapCommandArgs
{
    public readonly Player Player;
    public TapCommandArgs(Player player){
        Player = player;
    }
}
