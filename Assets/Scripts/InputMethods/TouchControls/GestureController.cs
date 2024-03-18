using System;
using UnityEngine;

public class GestureController : MonoBehaviour, IInputInvoker
{
    #region Singleton

    public static GestureController Instance;

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
    [SerializeField] private float _doubleGestureTimeLimit;
    [SerializeField] private float _holdThreshold; // Minimum amount of time required to register a stationary touch as holding.
    [SerializeField] private bool _concurrentTap; // If the input listener will listen for double tap.
    [SerializeField] private float _concurrentTapTimeLimit; // The amount time that will be waited for the second tap input.
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

    int debugIndex = 0;
    private void Update() 
    {
        if(_touchA.Type == TouchType.ForceHold && _touchB.Type == TouchType.ForceHold) {
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

        if(_touchA.Type == TouchType.Tap) 
        {
            if(_concurrentTap){
                if(_touchB.Type == TouchType.Tap){
                    InvokeInput(_inputEvents.OnDirectInputGesture, InputGestures.TapD);
                } 
                else if(_touchA.doubleTapTimeCounter <= _concurrentTapTimeLimit){
                    _touchA.keepOpen = true;
                    _touchA.doubleTapTimeCounter += Time.deltaTime;
                }
                else{
                    InvokeInput(_inputEvents.OnDirectInputGesture, InputGestures.TapL);
                    _touchA.keepOpen = false;
                    _touchA.doubleTapTimeCounter = 0;
                }
            }
            else{
                InvokeInput(_inputEvents.OnDirectInputGesture, InputGestures.TapL);
            }
        }
        else if(_touchB.Type == TouchType.Tap) 
        {
            if(_concurrentTap){
                if(_touchA.Type == TouchType.Tap){
                    InvokeInput(_inputEvents.OnDirectInputGesture, InputGestures.TapD);
                } 
                else if(_touchB.doubleTapTimeCounter <= _concurrentTapTimeLimit){
                    _touchB.keepOpen = true;
                    _touchB.doubleTapTimeCounter += Time.deltaTime;
                }
                else{
                    InvokeInput(_inputEvents.OnDirectInputGesture, InputGestures.TapR);
                    _touchB.keepOpen = false;
                    _touchB.doubleTapTimeCounter = 0;
                }
            }
            else{
                InvokeInput(_inputEvents.OnDirectInputGesture, InputGestures.TapL);
            }
        }

        if(_touchA.Type == TouchType.Swipe) 
        {
            GestureDirections swipeDirection = _touchA.SwipeDirection;
            if(_touchB.IsActive){
                if(_touchB.Type != TouchType.None){
                    Debug.Log("Frame: " + debugIndex + " " + GetSwipeInputGesture(swipeDirection, ScreenSide.Left));
                    InvokeInput(_inputEvents.OnDirectInputGesture, GetSwipeInputGesture(swipeDirection, ScreenSide.Left));
                }
                else{
                    if(_touchA.doubleSwipeTimeCounter <= _swipeTimeLimit){
                        _touchA.keepOpen = true;
                        _touchA.doubleSwipeTimeCounter += Time.deltaTime;
                    }
                }
                Debug.Log(_touchA.doubleSwipeTimeCounter + " / " + _swipeTimeLimit);
            }
            else{
                if(_touchB.Type == TouchType.Swipe && (_touchB.SwipeDirection == _touchA.SwipeDirection)){
                    Debug.Log("Frame: " + debugIndex + " " + GetSwipeInputGesture(swipeDirection, ScreenSide.LeftNRight));
                    InvokeInput(_inputEvents.OnDirectInputGesture, GetSwipeInputGesture(swipeDirection, ScreenSide.LeftNRight));
                }
                else{
                    Debug.Log("Frame: " + debugIndex + " " + GetSwipeInputGesture(swipeDirection, ScreenSide.Left));
                    InvokeInput(_inputEvents.OnDirectInputGesture, GetSwipeInputGesture(swipeDirection, ScreenSide.Left));
                }
            }
        }
        else if(_touchB.Type == TouchType.Swipe) 
        {
            GestureDirections swipeDirection = _touchB.SwipeDirection;
            if(_touchA.IsActive){
                if(_touchA.Type != TouchType.None){
                    Debug.Log("Frame: " + debugIndex + " " + GetSwipeInputGesture(swipeDirection, ScreenSide.Right));
                    InvokeInput(_inputEvents.OnDirectInputGesture, GetSwipeInputGesture(swipeDirection, ScreenSide.Right));
                }
                else{
                    if(_touchB.doubleSwipeTimeCounter <= _swipeTimeLimit){
                        _touchB.keepOpen = true;
                        _touchB.doubleSwipeTimeCounter += Time.deltaTime;
                    }
                }
                Debug.Log(_touchB.doubleSwipeTimeCounter + " / " + _swipeTimeLimit);
            }
            else{
                if(_touchA.Type == TouchType.Swipe && (_touchA.SwipeDirection == _touchB.SwipeDirection)){
                    Debug.Log("Frame: " + debugIndex + " " + GetSwipeInputGesture(swipeDirection, ScreenSide.LeftNRight));
                    InvokeInput(_inputEvents.OnDirectInputGesture, GetSwipeInputGesture(swipeDirection, ScreenSide.LeftNRight));
                }
                else{
                    Debug.Log("Frame: " + debugIndex + " " + GetSwipeInputGesture(swipeDirection, ScreenSide.Right));
                    InvokeInput(_inputEvents.OnDirectInputGesture, GetSwipeInputGesture(swipeDirection, ScreenSide.Right));
                }
            }
        }

        if(!_touchA.IsActive && !_touchA.keepOpen){
            if(_touchA.Type != TouchType.None) Debug.Log("Frame: " + debugIndex + " Touch A type reset.");
            _touchA.Type = TouchType.None;
        }

        if(!_touchB.IsActive && !_touchB.keepOpen){
            if(_touchB.Type != TouchType.None) Debug.Log("Frame: " + debugIndex + " Touch B type reset.");
            _touchB.Type = TouchType.None;
        }
        debugIndex++;
    }

    private void InvokeInput(Action<InputGestures> action, InputGestures gesture){
        _touchA.keepOpen = false;
        _touchA.doubleSwipeTimeCounter = 0;
        _touchB.keepOpen = false;
        _touchB.doubleSwipeTimeCounter = 0;
        action?.Invoke(gesture);
    }

    private InputGestures GetSwipeInputGesture(GestureDirections gestureDirection, ScreenSide side){
        if(side == ScreenSide.Right){
            if(gestureDirection == GestureDirections.Up) return InputGestures.SwipeUpR;
            else if(gestureDirection == GestureDirections.Down) return InputGestures.SwipeDownR;
            else if(gestureDirection == GestureDirections.Right) return InputGestures.SwipeRightR;
            else if(gestureDirection == GestureDirections.Left) return InputGestures.SwipeLeftR;
        }
        else if(side == ScreenSide.Left){
            if(gestureDirection == GestureDirections.Up) return InputGestures.SwipeUpL;
            else if(gestureDirection == GestureDirections.Down) return InputGestures.SwipeDownL;
            else if(gestureDirection == GestureDirections.Right) return InputGestures.SwipeRightL;
            else if(gestureDirection == GestureDirections.Left) return InputGestures.SwipeLeftL;
        }
        else if(side == ScreenSide.LeftNRight){
            if(gestureDirection == GestureDirections.Up) return InputGestures.SwipeUpD;
            else if(gestureDirection == GestureDirections.Down) return InputGestures.SwipeDownD;
            else if(gestureDirection == GestureDirections.Right) return InputGestures.SwipeRightD;
            else if(gestureDirection == GestureDirections.Left) return InputGestures.SwipeLeftD;
        }
        
        return InputGestures.None;
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
    public bool keepOpen = false;
    public float doubleSwipeTimeCounter = 0;
    public float doubleTapTimeCounter = 0;
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

        if(HoldTime > GestureController.Instance.HoldThreshold) {
            Type = TouchType.Hold;  
        }
        else{
            Type = TouchType.ForceHold;  
        }
    }

    public virtual void OnTouchDrag(InputEventParams inputEventDragParams) 
    {  
        TimeOnScreen += Time.deltaTime;

        if(inputEventDragParams.DeltaSpeed < GestureController.Instance.SwipeSpeed)
        {
            Type = TouchType.ForceHold;

            if(TimeOnScreen > GestureController.Instance.HoldThreshold){
                Type = TouchType.Hold;
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

                bool isSwipe = (distance / DragTime >= GestureController.Instance.SwipeSpeed) && (distance > GestureController.Instance.SwipeDistance);

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
        keepOpen = false;
        doubleSwipeTimeCounter = 0;
        doubleTapTimeCounter = 0;
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
        
        if(HasMoved && TimeOnScreen > GestureController.Instance.SwipeTimeLimit){
            if(Math.Abs(InitialScreenPosition.x - inputEventParams.ScreenPosition.x) > GestureController.Instance.JoystickDeadzone)
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
            if(HoldTime > GestureController.Instance.HoldThreshold) {
                Type = TouchType.Hold;  
            }
        }
        
    }

    public override void OnTouchDrag(InputEventParams inputEventDragParams)
    {
        if(inputEventDragParams.DeltaSpeed < GestureController.Instance.SwipeDistance || Type == TouchType.Hold || TimeOnScreen > GestureController.Instance.SwipeTimeLimit){
            Type = TouchType.Drag;
        }

        if(Type == TouchType.Drag){
            if(Math.Abs(InitialScreenPosition.x - inputEventDragParams.ScreenPosition.x) > GestureController.Instance.JoystickDeadzone)
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
