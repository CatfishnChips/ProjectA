// using System.Collections.Generic;
// using UnityEngine;

// public class GestureStateMachine : MonoBehaviour, IStateMachineRunner, IInputInvoker
// {
//     #region Singleton

//     public static GestureStateMachine Instance;

//     private void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }
//         Instance = this;
//     }

//     #endregion

//     protected StateMachineBaseState _currentState;
//     protected GestureStateFactory _stateFactory;
//     protected InputEvents _inputEvents;

//     [Header("Swipe Settings")]
//     [SerializeField] private float _swipeDistance; // Minimum distance required to register as a swipe. // Can also use Screen.width and Screen.height.
//     [SerializeField] private float _swipeSpeed; // How fast the finger should move to register a gsture as a swipe. 
//     [SerializeField] private float _holdThreshold; // Minimum amount of time required to register a stationary touch as holding.

//     public float SwipeDistance { get => _swipeDistance; }
//     public float SwipeSpeed { get => _swipeSpeed; }
//     public float HoldThreshold { get => _holdThreshold; }

//     private TouchInput<bool> _touchBegin = new TouchInput<bool>(false, 0, 0, InputTypes.Hold, SubInputTypes.None);
//     private TouchInput<bool> _touchEnd = new TouchInput<bool>(false, 0, 0, InputTypes.Hold, SubInputTypes.None);
//     private TouchContinuousInput<int> _touchStationary = new TouchContinuousInput<int>(0, 0, 0, InputTypes.Drag, SubInputTypes.None);
//     private TouchContinuousInput<bool> _touchDrag = new TouchContinuousInput<bool>(false, 0, 0, InputTypes.Hold, SubInputTypes.None);
//     protected List<ITouchInput> _inputsList;

//     private TouchData _touchA, _touchB;

//     private void Start() 
//     {
//         _currentState = new GestureNoTouchState(this, _stateFactory);

//         _touchA = new TouchData();
//         _touchB = new TouchData();

//         TouchInputReader.Instance.OnTouchABegin += _touchA.OnTouchBegin;
//         TouchInputReader.Instance.OnTouchAStationary += _touchA.OnTouchStationary;
//         TouchInputReader.Instance.OnTouchADrag += _touchA.OnTouchDrag;
//         TouchInputReader.Instance.OnTouchAEnd += _touchA.OnTouchEnd;

//         TouchInputReader.Instance.OnTouchBBegin += _touchB.OnTouchBegin;
//         TouchInputReader.Instance.OnTouchBStationary += _touchB.OnTouchStationary;
//         TouchInputReader.Instance.OnTouchBDrag += _touchB.OnTouchDrag;
//         TouchInputReader.Instance.OnTouchBEnd += _touchB.OnTouchEnd;
//     }

//     private void OnDisable()
//     {
//         TouchInputReader.Instance.OnTouchABegin -= _touchA.OnTouchBegin;
//         TouchInputReader.Instance.OnTouchAStationary -= _touchA.OnTouchStationary;
//         TouchInputReader.Instance.OnTouchADrag -= _touchA.OnTouchDrag;
//         TouchInputReader.Instance.OnTouchAEnd -= _touchA.OnTouchEnd;

//         TouchInputReader.Instance.OnTouchBBegin -= _touchB.OnTouchBegin;
//         TouchInputReader.Instance.OnTouchBStationary -= _touchB.OnTouchStationary;
//         TouchInputReader.Instance.OnTouchBDrag -= _touchB.OnTouchDrag;
//         TouchInputReader.Instance.OnTouchBEnd -= _touchB.OnTouchEnd;
//     }

//     private void Update() 
//     {
//         _currentState.UpdateStates();
//     }

//     public InputEvents GetInputEvents()
//     {
//         return _inputEvents;
//     }

//     public void SetInputEvents(InputEvents inputEvents)
//     {
//         _inputEvents = inputEvents;
//         Debug.Log("Gersture no pattern: " + _inputEvents.ToString());
//     }

//     public bool IsActiveAndEnabled()
//     { 
//         return isActiveAndEnabled; 
//     }

//     public void SwitchState(StateMachineBaseState state)
//     {
//         _currentState = state;
//     }
// }

// public class TouchData{

//     public Vector2 InitialScreenPosition;
//     public Vector3 InitialWorldPosition;
//     public Vector2 DragStartScreenPosition;
//     public float DragTime; 
//     public float TimeOnScreen;
//     public float Speed;
//     public float HoldTime;
//     public bool HasMoved;
//     public bool IsActive;
//     public SwipeDirections SwipeDirection;
//     public TouchState State;
//     public TouchType Type;

//     public TouchData()
//     {
//         State = TouchState.None;
//         Type = TouchType.None;
//     }

//     public void OnTouchBegin(InputEventParams inputEventParams) 
//     {
//         Reset();
//         InitialScreenPosition = inputEventParams.NormalizedScreenPosition;
//         InitialWorldPosition = inputEventParams.WorldPosition;

//         HasMoved = false;
//         HoldTime = 0f;

//         IsActive = true;
//     }

//     public void OnTouchStationary(InputEventParams inputEventParams)
//     {
//         if(State != TouchState.Stationary) 
//         {
//             State = TouchState.Stationary;
//         }

//         TimeOnScreen += Time.deltaTime;
//         HoldTime += Time.deltaTime;

//         if(HoldTime > GestureStateMachine.Instance.HoldThreshold) {
//             Type = TouchType.Hold;  
//         }
//     }

//     public void OnTouchDrag(InputEventParams inputEventDragParams) 
//     {  
//         TimeOnScreen += Time.deltaTime;

//         if(TimeOnScreen > GestureStateMachine.Instance.HoldThreshold && inputEventDragParams.DeltaSpeed < GestureStateMachine.Instance.SwipeSpeed)
//         {
//             Type = TouchType.Hold;  
//         }

//         if(State != TouchState.Drag) 
//         {
//             State = TouchState.Drag;
//             DragStartScreenPosition = inputEventDragParams.ScreenPosition;
//             DragTime = 0f;
//         }
        
//         HasMoved = true;
//         TimeOnScreen += Time.deltaTime;
//         DragTime += Time.deltaTime;
//     }

//     public void OnTouchEnd(InputEventParams inputEventParams) 
//     {
//         TouchState previousState = State;

//         float distance;
//         Vector2 direction;

//         switch(previousState)
//         {
//             case TouchState.Stationary:
//                 if(Type != TouchType.Hold) Type = TouchType.Tap;
//                 break;

//             case TouchState.Drag:
//                 distance = Vector2.Distance(DragStartScreenPosition, inputEventParams.ScreenPosition);
//                 direction = (inputEventParams.ScreenPosition - DragStartScreenPosition).normalized;

//                 bool isSwipe = (distance / DragTime >= GestureStateMachine.Instance.SwipeSpeed) && (distance > GestureStateMachine.Instance.SwipeDistance);

//                 if(isSwipe) 
//                 {                    
//                     if(direction.y > 0.716) SwipeDirection = SwipeDirections.Up;
//                     else if(direction.y < -0.716) SwipeDirection = SwipeDirections.Down;
//                     else if(direction.x < -0.716) SwipeDirection = SwipeDirections.Left;
//                     else if(direction.x > 0.716) SwipeDirection = SwipeDirections.Right;

//                     Type = TouchType.Swipe;
//                 }
//                 break;
//         }


//         IsActive = false;
//     }

//     public void Reset()
//     {
//         InitialScreenPosition = Vector2.zero;
//         InitialWorldPosition = Vector3.zero;
//         DragStartScreenPosition = Vector2.zero;
//         DragTime = 0f; 
//         TimeOnScreen = 0f;
//         HoldTime = 0f;
//         HasMoved = false;
//         IsActive = false;
//         State = TouchState.None;
//     }

//     public void ReleaseTouchKeepData()
//     {
//         IsActive = false;
//         State = TouchState.None;
//     }
// }


