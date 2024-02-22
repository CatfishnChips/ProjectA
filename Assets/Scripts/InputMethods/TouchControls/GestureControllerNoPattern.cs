using System;
using UnityEngine;

public class GestureControllerNoPattern : MonoBehaviour, IInputInvoker
{
    #region Singleton

    public static GestureControllerNoPattern Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #endregion

    protected InputEvents _inputEvents;

    [Header("Swipe Settings")]
    [SerializeField] private float _minSwipeDistance; // Minimum distance required to register as a swipe. // Can also use Screen.width and Screen.height.
    [SerializeField] private float _minSwipeSpeed; // How fast the finger should move to register a gsture as a swipe. 
    [SerializeField] private float _swipeTimeLimit;
    [SerializeField] private float _holdThreshold; // Minimum amount of time required to register a stationary touch as holding.
    [SerializeField] private float _joystickDeadzone;

    public float SwipeDistance { get => _minSwipeDistance; }
    public float SwipeSpeed { get => _minSwipeSpeed; }
    public float SwipeTimeLimit { get => _swipeTimeLimit; set => _swipeTimeLimit = value; }
    public float HoldThreshold { get => _holdThreshold; }
    public float JoystickDeadzone { get => _joystickDeadzone; }

    private TouchData _touchA, _touchB;

    private void Start() 
    {
        _touchA = new VariantTouchData();
        _touchB = new TouchData();

        TouchInputReader.Instance.OnTouchABegin += _touchA.OnTouchBegin;
        TouchInputReader.Instance.OnTouchAStationary += _touchA.OnTouchStationary;
        TouchInputReader.Instance.OnTouchADrag += _touchA.OnTouchDrag;
        TouchInputReader.Instance.OnTouchAEnd += _touchA.OnTouchEnd;

        TouchInputReader.Instance.OnTouchBBegin += _touchB.OnTouchBegin;
        TouchInputReader.Instance.OnTouchBStationary += _touchB.OnTouchStationary;
        TouchInputReader.Instance.OnTouchBDrag += _touchB.OnTouchDrag;
        TouchInputReader.Instance.OnTouchBEnd += _touchB.OnTouchEnd;
    }

    private void OnDisable()
    {
        TouchInputReader.Instance.OnTouchABegin -= _touchA.OnTouchBegin;
        TouchInputReader.Instance.OnTouchAStationary -= _touchA.OnTouchStationary;
        TouchInputReader.Instance.OnTouchADrag -= _touchA.OnTouchDrag;
        TouchInputReader.Instance.OnTouchAEnd -= _touchA.OnTouchEnd;

        TouchInputReader.Instance.OnTouchBBegin -= _touchB.OnTouchBegin;
        TouchInputReader.Instance.OnTouchBStationary -= _touchB.OnTouchStationary;
        TouchInputReader.Instance.OnTouchBDrag -= _touchB.OnTouchDrag;
        TouchInputReader.Instance.OnTouchBEnd -= _touchB.OnTouchEnd;
    }

    private void Update() 
    {
        if(_touchA.IsActive && _touchB.IsActive) {
            _inputEvents.OnHold?.Invoke(ScreenSide.LeftNRight);
        }
        else if(_touchA.Type == TouchType.Hold) {
            _inputEvents.OnHold?.Invoke(ScreenSide.Left);
        }
        else if(_touchB.Type == TouchType.Hold) {
            _inputEvents.OnHold?.Invoke(ScreenSide.Right);
        }

        if(_touchA.Type == TouchType.Drag){
            _inputEvents.OnDrag?.Invoke(ScreenSide.Left, _touchA.DragDirection);
        }

        if(_touchA.Type == TouchType.Tap && _touchB.Type == TouchType.Tap) 
        {
            _inputEvents.OnTap?.Invoke(ScreenSide.LeftNRight);
        }
        else if(_touchA.Type == TouchType.Tap) 
        {
            _inputEvents.OnTap?.Invoke(ScreenSide.Left);
        }
        else if(_touchB.Type == TouchType.Tap) 
        {
            _inputEvents.OnTap?.Invoke(ScreenSide.Right);
        }

        if(_touchA.Type == TouchType.Swipe && _touchB.Type == TouchType.Swipe) 
        {
            _inputEvents.OnSwipe?.Invoke(ScreenSide.LeftNRight, _touchA.SwipeDirection, _touchB.SwipeDirection);
        }
        else if(_touchA.Type == TouchType.Swipe) 
        {
            _inputEvents.OnSwipe?.Invoke(ScreenSide.Left, _touchA.SwipeDirection, GestureDirections.None);
        }
        else if(_touchB.Type == TouchType.Swipe) 
        {
            _inputEvents.OnSwipe?.Invoke(ScreenSide.Right, GestureDirections.None,  _touchB.SwipeDirection);
        }

        if(!_touchA.IsActive){
            _touchA.Type = TouchType.None;
        }

        if(!_touchB.IsActive){
            _touchB.Type = TouchType.None;
        }

    }

    public InputEvents GetInputEvents()
    {
        return _inputEvents;
    }

    public void SetInputEvents(InputEvents inputEvents)
    {
        _inputEvents = inputEvents;
    }

    public bool IsActiveAndEnabled()
    {
        return isActiveAndEnabled;
    }
}

public class TouchData{

    public Vector2 InitialScreenPosition;
    public Vector3 InitialWorldPosition;
    public Vector2 DragStartScreenPosition;
    public float DragTime; 
    public float TimeOnScreen;
    public float Speed;
    public float HoldTime;
    public bool HasMoved;
    public bool IsActive;
    public GestureDirections SwipeDirection;
    public GestureDirections DragDirection;
    public TouchState State;
    public TouchType Type;

    public TouchData()
    {
        State = TouchState.None;
        Type = TouchType.None;
    }

    public void OnTouchBegin(InputEventParams inputEventParams) 
    {
        Reset();
        InitialScreenPosition = inputEventParams.ScreenPosition;
        InitialWorldPosition = inputEventParams.WorldPosition;

        HasMoved = false;
        HoldTime = 0f;

        IsActive = true;
    }

    public virtual void OnTouchStationary(InputEventParams inputEventParams)
    {
        if(State != TouchState.Stationary) 
        {
            State = TouchState.Stationary;
        }

        TimeOnScreen += Time.deltaTime;
        HoldTime += Time.deltaTime;

        if(HoldTime > GestureControllerNoPattern.Instance.HoldThreshold) {
            Type = TouchType.Hold;  
        }
    }

    public virtual void OnTouchDrag(InputEventParams inputEventDragParams) 
    {  
        TimeOnScreen += Time.deltaTime;

        if(TimeOnScreen > GestureControllerNoPattern.Instance.HoldThreshold && inputEventDragParams.DeltaSpeed < GestureControllerNoPattern.Instance.SwipeSpeed)
        {
            Type = TouchType.Hold;  
        }

        if(State != TouchState.Move) 
        {
            State = TouchState.Move;
            DragStartScreenPosition = inputEventDragParams.ScreenPosition;
            DragTime = 0f;
        }
        
        HasMoved = true;
        TimeOnScreen += Time.deltaTime;
        DragTime += Time.deltaTime;
    }

    public virtual void OnTouchEnd(InputEventParams inputEventParams) 
    {
        TouchState previousState = State;

        float distance;
        Vector2 direction;

        switch(previousState)
        {
            case TouchState.Stationary:
                if(Type != TouchType.Hold) Type = TouchType.Tap;
                break;

            case TouchState.Move:
                distance = Vector2.Distance(DragStartScreenPosition, inputEventParams.ScreenPosition);
                direction = (inputEventParams.ScreenPosition - DragStartScreenPosition).normalized;

                bool isSwipe = (distance / DragTime >= GestureControllerNoPattern.Instance.SwipeSpeed) && (distance > GestureControllerNoPattern.Instance.SwipeDistance);

                if(isSwipe) 
                {                    
                    if(direction.y > 0.716) SwipeDirection = GestureDirections.Up;
                    else if(direction.y < -0.716) SwipeDirection = GestureDirections.Down;
                    else if(direction.x < -0.716) SwipeDirection = GestureDirections.Left;
                    else if(direction.x > 0.716) SwipeDirection = GestureDirections.Right;

                    Type = TouchType.Swipe;
                }
                break;
        }


        IsActive = false;
    }

    public void Reset()
    {
        InitialScreenPosition = Vector2.zero;
        InitialWorldPosition = Vector3.zero;
        DragStartScreenPosition = Vector2.zero;
        DragTime = 0f; 
        TimeOnScreen = 0f;
        HoldTime = 0f;
        HasMoved = false;
        IsActive = false;
        State = TouchState.None;
    }

    public void ReleaseTouchKeepData()
    {
        IsActive = false;
        State = TouchState.None;
    }
}

public class VariantTouchData : TouchData{

    public override void OnTouchStationary(InputEventParams inputEventParams)
    {
        if(State != TouchState.Stationary) 
        {
            HoldTime = 0;
            State = TouchState.Stationary;
        }

        TimeOnScreen += Time.deltaTime;
        HoldTime += Time.deltaTime;
        
        if(HasMoved && TimeOnScreen > GestureControllerNoPattern.Instance.SwipeTimeLimit){
            if(Math.Abs(InitialScreenPosition.x - inputEventParams.ScreenPosition.x) > GestureControllerNoPattern.Instance.JoystickDeadzone)
            {
                Type = TouchType.Drag;
                if(inputEventParams.ScreenPosition.x - InitialScreenPosition.x > 0){DragDirection = GestureDirections.Right;}
                else{DragDirection = GestureDirections.Left;}
            }
            else{
                DragDirection = GestureDirections.None;
            }
        }
        else{
            if(HoldTime > GestureControllerNoPattern.Instance.HoldThreshold) {
                Type = TouchType.Hold;  
            }
        }
        
    }

    public override void OnTouchDrag(InputEventParams inputEventDragParams)
    {
        if(inputEventDragParams.DeltaSpeed < GestureControllerNoPattern.Instance.SwipeDistance || Type == TouchType.Hold || TimeOnScreen > GestureControllerNoPattern.Instance.SwipeTimeLimit){
            Type = TouchType.Drag;
        }

        if(Type == TouchType.Drag){
            if(Math.Abs(InitialScreenPosition.x - inputEventDragParams.ScreenPosition.x) > GestureControllerNoPattern.Instance.JoystickDeadzone)
            {
                Type = TouchType.Drag;
                if(inputEventDragParams.ScreenPosition.x - InitialScreenPosition.x > 0){DragDirection = GestureDirections.Right;}
                else{DragDirection = GestureDirections.Left;}
            }
            else{
                DragDirection = GestureDirections.None;
            }
        }
        

        if(State != TouchState.Move) 
        {
            State = TouchState.Move;
            DragStartScreenPosition = inputEventDragParams.ScreenPosition;
            DragTime = 0f;
        }
        
        HasMoved = true;
        TimeOnScreen += Time.deltaTime;
        DragTime += Time.deltaTime;
    }

    public override void OnTouchEnd(InputEventParams inputEventParams)
    {
        if(Type == TouchType.Drag) {
            IsActive = false;
            return;
        }
        base.OnTouchEnd(inputEventParams);
    }

}
