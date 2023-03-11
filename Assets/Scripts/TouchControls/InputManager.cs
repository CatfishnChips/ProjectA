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

    [Header("Settings")]
    [SerializeField] private float _touchMoveSensitivity = 1.25f;

    private bool _isTouching; // Used for once per touch actions.
    private int _touchAID, _touchBID = 12;

    #region Events

    public UnityAction<Vector2, Vector3> OnTouchABegin;
    public UnityAction<Vector2, Vector3> OnTouchAStationary;
    public UnityAction<InputEventParams> OnTouchADrag;
    public UnityAction<InputEventParams> OnTouchAEnd;

    public UnityAction<Vector2, Vector3> OnTouchBBegin;
    public UnityAction<Vector2, Vector3> OnTouchBStationary;
    public UnityAction<InputEventParams> OnTouchBDrag;
    public UnityAction<InputEventParams> OnTouchBEnd;

    #endregion

    private void Start() 
    {
        _touchAID = 12;
        _touchBID = 12;
    }

    void Update()
    {
        if (Input.touchCount > 0) 
        {
            // Once per touch actions.
            if (!_isTouching) 
            {
                _isTouching = true;
            }

            if (Input.touchCount <= 2) // This if statement should be better implemented.
            {
                // Multiple Touches
                for (int i = 0; i < Input.touchCount; i++) 
                {
                    Touch touch = Input.GetTouch(i);
                    Vector3 touchWorldPosition = ConvertToWorldPosition(touch.position);

                    Vector2 touchMoveDelta = touch.deltaPosition;
                    float touchMoveSpeed = touch.deltaPosition.magnitude / touch.deltaTime;
                    int touchTapCount = touch.tapCount; // Currently not being used!

                    switch (touch.phase) 
                    {
                        case TouchPhase.Began:

                        // Screen Touch Side    // Better implementation maybe?
                        // If first touch occupied either Touch A or Touch B, then instead of looking at the initial touch location
                        // assign next touch to the other touch (Touch A or Touch B).
                        if (_touchAID == 0 && _touchBID != 0) 
                        {
                            Debug.Log("Touch A is already occupied! Assigning to Touch B.");
                            _touchBID = touch.fingerId;
                        } 
                        else if (_touchBID == 0 && _touchAID != 0) 
                        {
                            Debug.Log("Touch B is already occupied! Assigning to Touch A.");
                            _touchAID = touch.fingerId;
                        }
                        else 
                        {
                            if (touch.position.x < Screen.width / 2)
                            {
                                //Debug.Log("Left Screen! Assigning to Touch A.");
                                _touchAID = touch.fingerId;
                            }
                            else if (touch.position.x > Screen.width / 2) 
                            {
                                //Debug.Log("Right Screen! Assigning to Touch B.");
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
                        break;

                        case TouchPhase.Stationary:
                        if (touch.fingerId == _touchAID) 
                        {
                            // Touch A
                            OnTouchAStationary?.Invoke(touch.position, touchWorldPosition);
                        }
                        else if (touch.fingerId == _touchBID) 
                        {
                            // Touch B
                            OnTouchBStationary?.Invoke(touch.position, touchWorldPosition);
                        }
                        break;

                        case TouchPhase.Moved:
                        // Move Sensitivity Treshold
                        if (touch.phase == TouchPhase.Moved && touch.deltaPosition.magnitude < _touchMoveSensitivity) 
                        {
                            touch.phase = TouchPhase.Stationary;
                        }
                        else
                        {
                            if (touch.fingerId == _touchAID) 
                            {
                                // Touch A
                                OnTouchADrag?.Invoke(new InputEventParams(touch.position, touchWorldPosition, touchMoveSpeed, touchMoveDelta));
                            }
                            else if (touch.fingerId == _touchBID) 
                            {
                                // Touch B
                                OnTouchBDrag?.Invoke(new InputEventParams(touch.position, touchWorldPosition, touchMoveSpeed, touchMoveDelta));
                            }
                        }                   
                        break;

                        case TouchPhase.Ended:
                        if (touch.fingerId == _touchAID) 
                        {
                            // Touch A
                            OnTouchAEnd?.Invoke(new InputEventParams(touch.position, touchWorldPosition, touchMoveSpeed, touchMoveDelta));
                            _touchAID = 12; // Some arbitrary value used to reset the ID.
                        }
                        else if (touch.fingerId == _touchBID) 
                        {
                            // Touch B
                            OnTouchBEnd?.Invoke(new InputEventParams(touch.position, touchWorldPosition, touchMoveSpeed, touchMoveDelta));
                            _touchBID = 12; // Some arbitrary value used to reset the ID.
                        }
                        break;
                    }
                    //Debug.Log("ID: " + touch.fingerId + " Phase: " + touch.phase + " RawPos: " + touch.rawPosition + " Pos: " + touch.position 
                    //+ " Speed: " + touchMoveSpeed + " Dir: " + touchMoveDirection + " TapCount: " + touch.tapCount);
                }
            }   
        }
        else 
        {
            _isTouching = false;
        }
    }

    private Vector3 ConvertToWorldPosition(Vector2 position) 
    {
        Vector3 touchScreenPosition = position;
        touchScreenPosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(position);        
    }
}

public struct InputEventParams 
{
    public Vector2 ScreenPosition;
    public Vector3 WorldPosition;
    public float DeltaSpeed;
    public Vector2 Delta;

    public InputEventParams(Vector2 screenPosition, Vector3 worldPosition, float deltaSpeed, Vector2 delta) => 
        (ScreenPosition, WorldPosition, DeltaSpeed, Delta) = (screenPosition, worldPosition, deltaSpeed, delta);
}