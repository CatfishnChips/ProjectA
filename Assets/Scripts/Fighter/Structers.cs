using System.Collections.Generic;
using System;
using UnityEngine.Events;

//To achive a modifiable MoveSet system through inspector.
[Serializable]
public struct ActionAttribution{
    public FighterBaseState action;
}

[Serializable]
public struct InputAttackAttribution
{
    public InputGestures inputGesture;
    public FighterAttackState actionFighterAttack;
}

[Serializable]
public struct AttackActionAttribution{
    public FighterAttackState fighterAttack;
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
    public FighterAttackState theMove;
    public bool willOverride;
    public FighterAttackState moveToOverride;
    public Dictionary<string, ComboMoveSpecs> possibleNextMoves;

    public ComboMoveSpecs(){
        possibleNextMoves = new Dictionary<string, ComboMoveSpecs>();    
    }

}

public delegate void FrameEventFunction(FighterStateMachine ctx, ActionAttack state);

public struct FrameEvent
{   
    private int m_frame;
    private FrameEventFunction m_event;
    public int Frame {get => m_frame;}
    public FrameEvent(int frame, FrameEventFunction func){
        m_frame = frame;
        m_event = func;
    }
    public void Event(FighterStateMachine ctx, ActionAttack state) { m_event(ctx, state); }
}

public delegate void FrameEventFunction_Spirit(SpiritController ctx);

public struct FrameEvent_Spirit
{
    private int m_frame;
    private FrameEventFunction_Spirit m_event;
    public int Frame {get => m_frame;}
    public FrameEvent_Spirit(int frame, FrameEventFunction_Spirit func){
        m_frame = frame;
        m_event = func;
    }
    public void Event(SpiritController ctx) { m_event(ctx); }
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
    protected T _defaultValue;
    public Input(T value){
        _value = value;
        _frame = 0;
        _defaultValue = value;
    }

    public virtual void Reset(){
        _value = _defaultValue;
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

    public override void Reset()
    {
        base.Reset();
        _targetValue = _defaultValue;
    }
}

public class QueueInput<T, U> : Input<T>
{
    private Queue<U> _queue;
    public Queue<U> Queue {get{return _queue;}}
    public QueueInput(T value) : base(value){
        _queue = new Queue<U>();
    }

    public override void Reset()
    {
        base.Reset();
        Queue.Clear();
    }
}

#endregion