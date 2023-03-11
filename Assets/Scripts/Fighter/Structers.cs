using System.Collections.Generic;
using System;

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


