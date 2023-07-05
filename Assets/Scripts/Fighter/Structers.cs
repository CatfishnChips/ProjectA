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

[Serializable]
public struct FrameEvent
{
    public int Frame;
    public UnityEngine.Events.UnityEvent Event;
}

[Serializable]
public struct Config
{
    public int InputDelay;
    public int InputBuffer;
}

#region Input Classes

public class Input<T>
{
    protected T _value;
    private int _frame;
    public T Value {get{return _value;} set{_value = value;}}
    public int Frame {get{return _frame;} set{_frame = value;}}
    public Input(T value){
        _value = value;
        _frame = 0;
    }
}

public class ContinuousInput<T> : Input<T>
{
    private T _targetValue;
    public T TargetValue {get{return _targetValue;} set{_targetValue = value;}}
    public ContinuousInput(T value) : base(value){
        _targetValue = _value;
    }
}

public class QueueInput<T, U> : Input<T>
{
    private Queue<U> _queue;
    public Queue<U> Queue {get{return _queue;}}
    public QueueInput(T value) : base(value){
        _queue = new Queue<U>();
    }
}

#endregion