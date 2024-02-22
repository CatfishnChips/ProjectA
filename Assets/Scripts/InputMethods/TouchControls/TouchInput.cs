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
        _isValueRead = !_value.Equals(_defaultValue); // If the value is different than default when it's read only then we consider it read.
        return _value;
    }

    public virtual void OnTick()
    {
        if(!_isActive) return;

        if(_delayFrameCounter < InputManager.Instance.InputDelay){
            _delayFrameCounter++;
            return;
        }

        _value = _holderValue;

        if(_bufferFrameCounter > InputManager.Instance.InputBuffer)
        {
            Reset();
        }
        else{
            _bufferFrameCounter++;
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

public class TouchQueueInput<T> : TouchInput<T>
{
    private class QueueableContent
    {
        public T Content;
        public int DelayFrameCounter;
        public int BufferFrameCounter;
        public bool OnBufferQueue;
        public bool OnDelayQueue;

        public QueueableContent(){
            DelayFrameCounter = 0;
            BufferFrameCounter = 0;
            OnBufferQueue = false;
            OnDelayQueue = false;
        }

        public void Set(T content){
            Content = content;
            OnDelayQueue = true;
        }

        public void Reset(){
            Content = default(T);
            DelayFrameCounter = 0;
            BufferFrameCounter = 0;
            OnDelayQueue = false;
            OnBufferQueue = false;
        }
    }

    private QueueableContent[] _contentHolder;
    private List<QueueableContent> _contentQueue;
    private bool _isContentRead;

    public TouchQueueInput(InputTypes inputType, SubInputTypes subInputType) : base(default(T), inputType, subInputType)
    {
        _contentHolder = new QueueableContent[15];
        for(int i = 0; i < _contentHolder.Length; i++) _contentHolder[i] = new QueueableContent();

        _contentQueue = new List<QueueableContent>();
        _isContentRead = false;        
    }

    public override void Write(T content)
    {
        _isActive = true;
        for(int i = 0; i < _contentHolder.Length; i++){
            if(!_contentHolder[i].OnDelayQueue && !_contentHolder[i].OnBufferQueue){
                _contentHolder[i].Set(content);
                break;
            } 
        }
    }

    public new bool Read()
    {
        return _contentQueue.Count > 0;
    }

    public T ReadContent()
    {
        if(_contentQueue.Count == 0){
            return default(T);
        }
        else{
            _isContentRead = true;
            return _contentQueue[0].Content;
        } 
    }

    public T PeekContent()
    {
        return _contentQueue[0].Content;
    }

    public override void OnTick()
    {
        if(!_isActive) return;

        for(int i = 0; i < _contentHolder.Length; i++)
        {
            if(_contentHolder[i].OnDelayQueue)
            {
                if(_contentHolder[i].DelayFrameCounter >= InputManager.Instance.InputDelay)
                {
                    _contentQueue.Add(_contentHolder[i]);
                    _contentHolder[i].OnDelayQueue = false;
                    _contentHolder[i].OnBufferQueue = true;
                }
                _contentHolder[i].DelayFrameCounter++;

                //if(queuedContent) _contentHolder[i].Reset();
            }
        }
    }

    public override void OnLateTick()
    {
        Debug.Log("Late tick is called. Frame is: " + GameSimulator.Instance.TickCount);
        Debug.Log(_inputType);
        if(_isContentRead) 
        {
            ActionAttack aa = _contentQueue[0].Content as ActionAttack;
            _contentQueue[0].Reset();
            _contentQueue.RemoveAt(0);
            _isContentRead = false;
        }

        int i = 0;
        while (i < _contentQueue.Count)
        {
            _contentQueue[i].BufferFrameCounter++;
            if(_contentQueue[i].BufferFrameCounter > InputManager.Instance.InputBuffer){
                _contentQueue[0].Reset();
                _contentQueue.RemoveAt(0);
            }
            else{
                i++;
            }
        }
    }

}

public class TouchContinuousInput<T> : TouchInput<T>
{
    public TouchContinuousInput(T value, InputTypes inputType, SubInputTypes subInputType) : base(value, inputType, subInputType){}
    private bool _newInputRegistered;
    private bool _writtenToInput = false;

    public override void Write(T value)
    {
        //Debug.Log("Written to input. Frame is: " + GameSimulator.Instance.TickCount);
        _writtenToInput = true;
        _newInputRegistered = !value.Equals(_value);
        if(_newInputRegistered) _delayFrameCounter = 0; 
        _isActive = true;
        _holderValue = value;
    }

    public override T Read()
    {
        return _value;
    }

    public override void OnTick()
    {
        if(!_isActive) return;

        if(_newInputRegistered)
        {
            //Debug.Log(_delayFrameCounter + " / " + InputManager.Instance.InputDelay);
            if(_delayFrameCounter < InputManager.Instance.InputDelay){
                _delayFrameCounter++;
                return;
            } 
            _value = _holderValue;
        }
    }

    public override void OnLateTick()
    {
        //Debug.Log("Late tick is called. Frame is: " + GameSimulator.Instance.TickCount);
        if(!_writtenToInput) Reset();
        _writtenToInput = false;
    }
}
