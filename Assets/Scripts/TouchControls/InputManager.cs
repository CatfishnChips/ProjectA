using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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
    [SerializeField] private float _touchMoveSensitivityA = 1.25f;
    [SerializeField] private float _touchMoveSensitivityB = 3f;

    private bool _isTouching; // Used for once per touch actions.
    private int _touchAID, _touchBID = 12;

    #region Events

    public UnityAction<InputEventParams> OnTouchABegin;
    public UnityAction<InputEventParams> OnTouchAStationary;
    public UnityAction<InputEventParams> OnTouchADrag;
    public UnityAction<InputEventParams> OnTouchAEnd;

    public UnityAction<InputEventParams> OnTouchBBegin;
    public UnityAction<InputEventParams> OnTouchBStationary;
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
                        if (IsPointerOverUIElement(touch, out PointerEventData data, out List<RaycastResult> raycastResults)) return;
                        // Screen Touch Side    // Better implementation maybe?
                        // If first touch occupied either Touch A or Touch B, then instead of looking at the initial touch location
                        // assign next touch to the other touch (Touch A or Touch B).
                        // if (_touchAID == 0 && _touchBID != 0) 
                        // {
                        //     Debug.Log("Touch A is already occupied! Assigning to Touch B.");
                        //     _touchBID = touch.fingerId;
                        // } 
                        // else if (_touchBID == 0 && _touchAID != 0) 
                        // {
                        //     Debug.Log("Touch B is already occupied! Assigning to Touch A.");
                        //     _touchAID = touch.fingerId;
                        // }
                        // else 
                        // {
                        //     if (touch.position.x < Screen.width / 2)
                        //     {
                        //         //Debug.Log("Left Screen! Assigning to Touch A.");
                        //         _touchAID = touch.fingerId;
                        //     }
                        //     else if (touch.position.x > Screen.width / 2) 
                        //     {
                        //         //Debug.Log("Right Screen! Assigning to Touch B.");
                        //         _touchBID = touch.fingerId;
                        //     }
                        // }

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
                        
                        if (touch.fingerId == _touchAID) 
                        {
                            // Touch A
                            OnTouchABegin?.Invoke(new InputEventParams(touch.position, touchWorldPosition, 0f, Vector2.zero));
                        }
                        else if (touch.fingerId == _touchBID) 
                        {
                            // Touch B
                            OnTouchBBegin?.Invoke(new InputEventParams(touch.position, touchWorldPosition, 0f, Vector2.zero));
                        }
                        break;

                        case TouchPhase.Stationary:
                        if (touch.fingerId == _touchAID) 
                        {
                            // Touch A
                            OnTouchAStationary?.Invoke(new InputEventParams(touch.position, touchWorldPosition, 0f, Vector2.zero));
                        }
                        else if (touch.fingerId == _touchBID) 
                        {
                            // Touch B
                            OnTouchBStationary?.Invoke(new InputEventParams(touch.position, touchWorldPosition, 0f, Vector2.zero));
                        }
                        break;

                        case TouchPhase.Moved:
                        // Move Sensitivity Treshold
                        if (touch.fingerId == _touchAID) 
                        {
                            // Touch A
                            if (touch.deltaPosition.magnitude >= _touchMoveSensitivityA)
                            OnTouchADrag?.Invoke(new InputEventParams(touch.position, touchWorldPosition, touchMoveSpeed, touchMoveDelta));
                            else touch.phase = TouchPhase.Stationary;
                        }
                        else if (touch.fingerId == _touchBID) 
                        {
                            // Touch B
                            if (touch.deltaPosition.magnitude >= _touchMoveSensitivityB)
                            OnTouchBDrag?.Invoke(new InputEventParams(touch.position, touchWorldPosition, touchMoveSpeed, touchMoveDelta));
                            else touch.phase = TouchPhase.Stationary;
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
                    //+ " Speed: " + touchMoveSpeed  + " TapCount: " + touch.tapCount);
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

    // PointerEventData and List<RaycastResult> can be pre-initilazed for a non-alloc version.
    public bool IsPointerOverUIElement(Touch touch, out PointerEventData eventData, out List<RaycastResult> raycastResults)
    {
        eventData = new PointerEventData(EventSystem.current);
        eventData.position = touch.position;
        raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData,raycastResults);
        return raycastResults.Count > 0;
    }
}

public struct InputEventParams 
{
    public Vector2 ScreenPosition;
    public Vector2 NormalizedScreenPosition {get{return new Vector2(ScreenPosition.x / Screen.width, ScreenPosition.y / Screen.height);}}
    public Vector3 WorldPosition;
    public float DeltaSpeed;
    public Vector2 Delta;

    public InputEventParams(Vector2 screenPosition, Vector3 worldPosition, float deltaSpeed, Vector2 delta) => 
        (ScreenPosition, WorldPosition, DeltaSpeed, Delta) = (screenPosition, worldPosition, deltaSpeed, delta);
}