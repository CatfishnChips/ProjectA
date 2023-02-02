using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    #region Singleton

    public static InputManager Instance;

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

    [SerializeField] private LineRenderer _lineRenderer;
    private Vector3 pos;
    private bool isTouching;
    private bool isMoving;
    [SerializeField] private float _pointAddInterval = 0.1f;
    [SerializeField] private float _touchMoveTreshold = 0.1f;
    [SerializeField] private float _touchHoldTreshold = 0.5f;

    public UnityAction<int, Vector2, Vector3> OnTouchBegin;
    public UnityAction<int, Vector2, Vector3> OnTouchDrag; // Add move speed?
    public UnityAction<int, Vector2, Vector3> OnTouchEnd;
    public UnityAction<int, Vector2, Vector3> OnTouchTap; // Add tap count!
    public UnityAction<int, Vector2, Vector3> OnTouchHold; // Add delta time!

    public UnityAction<Vector2, Vector3> OnTouchABegin;
    public UnityAction<Vector2, Vector3> OnTouchADrag;
    public UnityAction<Vector2, Vector3> OnTouchAEnd;

    public UnityAction<Vector2, Vector3> OnTouchBBegin;
    public UnityAction<Vector2, Vector3> OnTouchBDrag;
    public UnityAction<Vector2, Vector3> OnTouchBEnd;

    private Touch _primaryTouch;
    private Touch _secondaryTouch;
    private int _primaryTouchID;

    private int _touchAID, _touchBID = 12;

    void Update()
    {
        if (Input.touchCount > 0) 
        {

            if (!isTouching) 
            {
                //DrawGesture();
                //StartCoroutine(AddLinePoint());
                isTouching = true;
            }

            if (Input.touchCount == 3)
            {
                _primaryTouch = Input.GetTouch(0);
                _primaryTouchID = _primaryTouch.fingerId;

                Vector3 touchWorldPosition = ConvertToWorldPosition(_primaryTouch.position);

                pos = touchWorldPosition; // Temporary!

                Vector2 touchMoveDirection = (_primaryTouch.rawPosition - _primaryTouch.position).normalized;
                float touchMoveDistanceFromInitialPosition = Vector2.Distance(_primaryTouch.rawPosition, _primaryTouch.position);
                Vector2 touchMoveDelta = _primaryTouch.deltaPosition;
                float touchMoveSpeed = _primaryTouch.deltaPosition.magnitude / _primaryTouch.deltaTime;
                //_primaryTouch.tapCount;

                switch (_primaryTouch.phase) 
                {
                    case TouchPhase.Began:
                    OnTouchBegin?.Invoke(_primaryTouchID, _primaryTouch.position, touchWorldPosition);
                    break;

                    case TouchPhase.Stationary:


                    if (_primaryTouch.deltaTime >= _touchHoldTreshold * Time.deltaTime) 
                    {
                        OnTouchHold?.Invoke(_primaryTouchID, _primaryTouch.position, touchWorldPosition);
                    }
                    else 
                    {
                        OnTouchTap?.Invoke(_primaryTouchID, _primaryTouch.position, touchWorldPosition);
                    }

                    break;

                    case TouchPhase.Moved:
                    touchMoveSpeed = _primaryTouch.deltaPosition.magnitude / _primaryTouch.deltaTime;
                    touchMoveDirection = (_primaryTouch.rawPosition - _primaryTouch.position).normalized;

                    OnTouchDrag?.Invoke(_primaryTouchID, _primaryTouch.position, touchWorldPosition);
                    break;

                    case TouchPhase.Ended:
                    OnTouchEnd?.Invoke(_primaryTouchID, _primaryTouch.position, touchWorldPosition);
                    
                    //if (touchMoveSpeed > 10f) // Touch Move Treshold for Swipe Motion // Instead of invoking the event here, send to shape to the gesture recognition and invoke it there.
                    //OnSwipe.Invoke(touchMoveDirection); // Example of an event invocation.
                    break;
                }

                Debug.Log("Phase: " + _primaryTouch.phase + " StartPos: " + _primaryTouch.rawPosition + " Pos: " + _primaryTouch.position 
                    + " Speed: " + touchMoveSpeed + " Dir: " + touchMoveDirection + " TapCount: " + _primaryTouch.tapCount);
            }
            else
            {
                // Multiple Touches
                foreach (Touch touch in Input.touches) 
                {
                    Vector3 touchWorldPosition = ConvertToWorldPosition(touch.position);

                    pos = touchWorldPosition; // Temporary!

                    Vector2 touchMoveDirection = (touch.position - touch.position).normalized;
                    float touchMoveDistanceFromInitialPosition = Vector2.Distance(touch.rawPosition, touch.position);
                    Vector2 touchMoveDelta = touch.deltaPosition;
                    float touchMoveSpeed = touch.deltaPosition.magnitude / touch.deltaTime;
                    //touch.tapCount;

                    switch (touch.phase) 
                    {
                        case TouchPhase.Began:

                        // Screen Touch Side    // Fix here!
                        if (_touchAID == touch.fingerId) 
                        {
                            _touchBID = touch.fingerId;
                        } 
                        else if (_touchBID == touch.fingerId) 
                        {
                            _touchAID = touch.fingerId;
                        }
                        else 
                        {
                            if (touch.position.x < Screen.width / 2)
                            {
                                _touchAID = touch.fingerId;
                            }
                            else if (touch.position.x > Screen.width / 2) 
                            {   
                                _touchBID = touch.fingerId;
                            }
                        }
                        
                        if (touch.fingerId == _touchAID) 
                        {
                            // Touch A
                            OnTouchABegin?.Invoke(touch.position, touchWorldPosition);
                        }
                        else if (touch.fingerId == _touchBID) 
                        {
                            // Touch B
                            OnTouchBBegin?.Invoke(touch.position, touchWorldPosition);
                        }
                        //OnTouchBegin?.Invoke(touch.fingerId, touch.rawPosition, touchWorldPosition);
                        break;

                        case TouchPhase.Stationary:

                        if (touch.fingerId == _touchAID) 
                        {
                            // Touch A
                            OnTouchABegin?.Invoke(touch.position, touchWorldPosition);
                        }
                        else if (touch.fingerId == _touchBID) 
                        {
                            // Touch B
                            OnTouchBBegin?.Invoke(touch.position, touchWorldPosition);
                        }

                        if (touch.deltaTime >= _touchHoldTreshold * Time.deltaTime) 
                        {
                            OnTouchHold?.Invoke(touch.fingerId, touch.position, touchWorldPosition);
                        }
                        else 
                        {
                            OnTouchTap?.Invoke(touch.fingerId, touch.position, touchWorldPosition);
                        }
                        break;

                        case TouchPhase.Moved:
                        if (touch.fingerId == _touchAID) 
                        {
                            // Touch A
                            OnTouchADrag?.Invoke(touch.position, touchWorldPosition);
                        }
                        else if (touch.fingerId == _touchBID) 
                        {
                            // Touch B
                            OnTouchBDrag?.Invoke(touch.position, touchWorldPosition);
                        }
                        //OnTouchDrag?.Invoke(touch.fingerId, _primaryTouch.position, touchWorldPosition);
                        break;

                        case TouchPhase.Ended:
                        if (touch.fingerId == _touchAID) 
                        {
                            // Touch A
                            OnTouchAEnd?.Invoke(touch.position, touchWorldPosition);
                            _touchAID = 12; // Some arbitrary value used to reset the ID.
                        }
                        else if (touch.fingerId == _touchBID) 
                        {
                            // Touch B
                            OnTouchBEnd?.Invoke(touch.position, touchWorldPosition);
                            _touchBID = 12; // Some arbitrary value used to reset the ID.
                        }
                        //OnTouchEnd?.Invoke(touch.fingerId, touch.position, touchWorldPosition);       
                        //if (touchMoveSpeed > 10f) // Touch Move Treshold for Swipe Motion // Instead of invoking the event here, send to shape to the gesture recognition and invoke it there.
                        //OnSwipe.Invoke(touchMoveDirection); // Example of an event invocation.
                        break;
                    }

                    Debug.Log("ID: " + touch.fingerId + " Phase: " + touch.phase + " RawPos: " + touch.rawPosition + " Pos: " + touch.position 
                    + " Speed: " + touchMoveSpeed + " Dir: " + touchMoveDirection + " TapCount: " + touch.tapCount);
                }
            }   
        }
        else 
        {
            isTouching = false;
            //StopCoroutine(AddLinePoint());
        }
    }

    private Vector3 ConvertToWorldPosition(Vector2 position) 
    {
        Vector3 touchScreenPosition = _primaryTouch.position;
        touchScreenPosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(_primaryTouch.position);        
    }
}

// private int positionCount = 0;
//     private void UpdateLineRenderer() 
//     {   
//         _lineRenderer.positionCount = positionCount + 1;
//         _lineRenderer.SetPosition(positionCount, new Vector3(pos.x, pos.y, 0));
//         positionCount++;
//     }

//     private void DrawGesture() 
//     {
//         GameObject obj = Instantiate(new GameObject(), transform.position, Quaternion.identity, transform); // Both Instantiate and new GameObject creates a new gameObject in the scene.
//         _lineRenderer = obj.AddComponent<LineRenderer>();
//         positionCount = 0;

//         // _lineRenderer.widthCurve = _lineCurve;
//         // _lineRenderer.material = _lineMaterial;
//         // _lineRenderer.positionCount = steps;
//     }

//     private IEnumerator AddLinePoint() 
//     {
//         while (true) 
//         {
//             //if (_lastFrameTouchPos == pos) yield return null;
            
//             yield return new WaitForSecondsRealtime(_pointAddInterval);
//             if (isMoving) 
//             {
//                 _lineRenderer.positionCount = positionCount + 1;
//                 _lineRenderer.SetPosition(positionCount, new Vector3(pos.x, pos.y, 0));
//                 positionCount++;
//             }
//         }
//     }

//     private void OnDrawGizmos() 
//     {
//         Gizmos.color = Color.green;
//         Gizmos.DrawSphere(pos, 0.1f);
//     }

// // Touch 1
                // _primaryTouch = Input.GetTouch(0);
                // _primaryTouchID = _primaryTouch.fingerId;
                // Vector3 touch1WorldPosition = ConvertToWorldPosition(_primaryTouch.position);

                // Vector2 touch1MoveDirection = (_primaryTouch.rawPosition - _primaryTouch.position).normalized;
                // float touch1MoveDistanceFromInitialPosition = Vector2.Distance(_primaryTouch.rawPosition, _primaryTouch.position);
                // Vector2 touch1MoveDelta = _primaryTouch.deltaPosition;
                // float touch1MoveSpeed = _primaryTouch.deltaPosition.magnitude / _primaryTouch.deltaTime;

                // switch (_primaryTouch.phase) 
                // {
                //     case TouchPhase.Began:
                    
                //     break;

                //     case TouchPhase.Moved:
                //     touch1MoveSpeed = _primaryTouch.deltaPosition.magnitude / _primaryTouch.deltaTime;
                //     touch1MoveDirection = (_primaryTouch.rawPosition - _primaryTouch.position).normalized;
                //     break;

                //     case TouchPhase.Ended:
                //     _touchEndPosition = _primaryTouch.position;

                //     //OnSwipe.Invoke(touch1MoveDirection); // Example of an event invocation.
                //     break;
                // }

                // // Touch 2
                // _secondaryTouch = Input.GetTouch(1);
                // _secondaryTouchID = _secondaryTouch.fingerId;
                // Vector3 touch2WorldPosition = ConvertToWorldPosition(_secondaryTouch.position);

                // Vector2 touch2MoveDirection = (_secondaryTouch.rawPosition - _secondaryTouch.position).normalized;
                // float touch2MoveDistanceFromInitialPosition = Vector2.Distance(_secondaryTouch.rawPosition, _secondaryTouch.position);
                // Vector2 touch2MoveDelta = _secondaryTouch.deltaPosition;
                // float touch2MoveSpeed = _secondaryTouch.deltaPosition.magnitude / _secondaryTouch.deltaTime;

                // switch (_secondaryTouch.phase) 
                // {
                //     case TouchPhase.Began:
                    
                //     break;

                //     case TouchPhase.Moved:
                //     touch2MoveSpeed = _primaryTouch.deltaPosition.magnitude / _primaryTouch.deltaTime;
                //     touch2MoveDirection = (_primaryTouch.rawPosition - _primaryTouch.position).normalized;
                //     break;

                //     case TouchPhase.Ended:
                //     _touchEndPosition = _primaryTouch.position;

                //     //OnSwipe.Invoke(touch2MoveDirection); // Example of an event invocation.
                //     break;
                // }

                // // Determine Touch Screen Side
                // if (_primaryTouch.rawPosition.x < Screen.width / 2) 
                // {
                //     _primaryTouchLeftSide = true;
                //     _secondaryTouchLeftSide = false;
                // }
                // else if (_primaryTouch.rawPosition.x > Screen.width / 2) 
                // {
                //     _primaryTouchLeftSide = false;
                //     _secondaryTouchLeftSide = true;
                // }

// Vector3 touchScreenPosition = _primaryTouch.position;
                // touchScreenPosition.z = Camera.main.nearClipPlane;
                // Vector3 touchWorldPosition = Camera.main.ScreenToWorldPoint(_primaryTouch.position);

// Instead of using Touch's own states, making our own states might be better.
                // Unity handles this automatically!
                // if (_primaryTouch.deltaPosition != Vector2.zero) 
                // {
                //     _primaryTouch.phase = TouchPhase.Stationary;
                // }
                // else
                // {
                //     _primaryTouch.phase = TouchPhase.Moved;
                // }
