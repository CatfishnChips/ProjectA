using System;
using System.Collections.Generic;
using UnityEngine;

public enum InputTypes
{
    Tap,
    Slide,
    Hold,
    Drag,
    Gesture,
    Jump,
    Dash,
    Dodge,
    Multitouch
}

public enum SubInputTypes
{
    Jump = InputTypes.Jump,
    Dash = InputTypes.Dash,
    Dodge = InputTypes.Dodge,
    Multitouch = InputTypes.Multitouch,
    None
}

public interface ITouchInput
{
    void OnTick();
    void OnLateTick();
    void Reset();
}

public class TouchInput<T> : ITouchInput 
{
    protected bool _isActive;
    protected bool _isListening;
    protected bool _isValueRead;
    protected T _value;
    protected T _defaultValue;
    protected T _holderValue;
    protected int _listenedOnFrame;
    protected int _bufferFrameCounter;
    protected int _delayFrameCounter;

    protected InputTypes _inputType;
    protected SubInputTypes _subInputType;

    public TouchInput(T value, InputTypes inputType, SubInputTypes subInputType)
    {
        _value = value;
        _defaultValue = _value;
        _holderValue = _value; 
        _inputType = inputType;
        _subInputType = subInputType;
        _listenedOnFrame = 0;
        _bufferFrameCounter = 0;
        _delayFrameCounter = 0;
        _isListening = true;
        _isActive = false;
        _isValueRead = false;
    }

    public virtual void Write(T value)
    {
        _isActive = true;
        if(_isListening) _holderValue = value;
    }

    public virtual T Read()
    {
        _isValueRead = !_value.Equals(_defaultValue);
        T result = _value;
        return result;
    }

    public virtual void OnTick()
    {
        if(!_isActive) return;

        if(_delayFrameCounter < GameManager.Instance.Config.InputDelay) return;

        _value = _holderValue;

        if(_bufferFrameCounter > GameManager.Instance.Config.InputBuffer)
        {
            Reset();
        }
    }

    public virtual void OnLateTick()
    {
        if(_isValueRead) Reset();
    }

    public virtual void Deactivate()
    {
        _isActive = false;
    }

    public virtual void setValue(T value)
    {
        _value = value;
    }

    public virtual void Reset()
    {
        _value = _defaultValue;
        _holderValue = _defaultValue;
        _listenedOnFrame = 0;
        _bufferFrameCounter = 0;
        _delayFrameCounter = 0;
        _isListening = true;
        _isValueRead = false;
        Deactivate();
    }

    public override string ToString()
    {
        return "Input Type: " + _inputType;
    }
}

public class TouchQueueInput<T, U> : TouchInput<T>
{
    private Queue<U> _actionQueue;
    private bool _isActionRead;

    public TouchQueueInput(T value, InputTypes inputType, SubInputTypes subInputType) : base(value, inputType, subInputType)
    {
        _actionQueue = new Queue<U>();
        _isActionRead = false;        
    }

    public void Write(T value, U action)
    {
        _isActive = true;
        if(_isListening)
        {
            _isListening = false;
            _holderValue = value;
            _actionQueue.Enqueue(action);
        }
    }

    public U ReadAction()
    {
        if(_actionQueue.Count == 0){
            return default(U);
        }
        else{
            _isActionRead = true;
            return _actionQueue.Peek();
        } 
    }

    public U PeekAction()
    {
        return _actionQueue.Peek();
    }

    public override void OnTick()
    {
        if(!_isActive) return;

        Debug.Log("Delay is: " + _delayFrameCounter + "/" + InputManager.Instance.InputDelay + " On Frame: " + GameSimulator.Instance.TickCount);

        if(_delayFrameCounter < InputManager.Instance.InputDelay){
            _delayFrameCounter++;
            return;
        } 

        _value = _holderValue;

        Debug.Log("Buffer varaibale is: " + _bufferFrameCounter + "/" + InputManager.Instance.InputBuffer);

        if(_bufferFrameCounter > InputManager.Instance.InputBuffer)
        {
            if(_actionQueue.Count == 1) Reset();
            else {
                _actionQueue.Dequeue();
                _isListening = true;
                _bufferFrameCounter = 0;
                _delayFrameCounter = 0;
            }
        }

        _bufferFrameCounter++;
    }

    public override void OnLateTick()
    {
        if(_isActionRead) 
        {
            Debug.Log("Action was read so trying to dequeue");
            _actionQueue.Dequeue();
            _isActionRead = false;
            Reset();
        }
    }

}

public class TouchContinuousInput<T> : TouchInput<T>
{
    public TouchContinuousInput(T value, InputTypes inputType, SubInputTypes subInputType) : base(value, inputType, subInputType){}
    private bool newInputRegistered;

    public override void Write(T value)
    {
        if(!_holderValue.Equals(_value)) newInputRegistered = true;
        _isActive = true;
        if(_isListening) _holderValue = value;
         _delayFrameCounter = 0;
    }

    public override T Read()
    {
        return _value;
    }

    public override void OnTick()
    {
        if(newInputRegistered)
        {
            if(_delayFrameCounter < GameManager.Instance.Config.InputDelay) return;
            _value = _holderValue;
        }
    }
}
