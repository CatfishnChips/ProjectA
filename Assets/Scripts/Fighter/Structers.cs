using System.Collections.Generic;
using System;
using JetBrains.Annotations;

//To achive a modifiable MoveSet system through inspector.
[Serializable]
public struct ActionAttribution{
    public ActionBase action;
}

//To achive a modifiable Combo system through inspector.
[Serializable]
public struct ComboMove{
    public string comboName;
    public ComboMoveSpecs[] moves;
}

[Serializable]
public class ComboMoveSpecs{
    //public int comboName;
    public int moveNumber;
    public ActionAttack theMove;
    public bool willOverride;
    public ActionAttack moveToOverride;
    public Dictionary<string, ComboMoveSpecs> possibleNextMoves;

    public ComboMoveSpecs(){
        possibleNextMoves = new Dictionary<string, ComboMoveSpecs>();    
    }

}

[Serializable]
public struct FrameEvent
{
    public int Frame;
    public UnityEngine.Events.UnityEvent Event;
}

public struct AttackNamePair{
    public string name;
    public AttackMove attack;
}
