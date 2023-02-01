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
    private Vector3 _lastFrameTouchPos;

    public UnityAction<Vector2> OnSwipe;

    private Vector2 _touchEndPosition;

    private Touch _primaryTouch;
    private Touch _touch1;
    private Touch _touch2;

    void Update()
    {
        if (Input.touchCount > 0) 
        {

            if (!isTouching) 
            {
                DrawGesture();
                StartCoroutine(AddLinePoint());
                isTouching = true;
            }

            if (Input.touchCount == 1)
            {
                _primaryTouch = Input.GetTouch(0);

                // Vector3 touchScreenPosition = _primaryTouch.position;
                // touchScreenPosition.z = Camera.main.nearClipPlane;
                // Vector3 touchWorldPosition = Camera.main.ScreenToWorldPoint(_primaryTouch.position);
                Vector3 touchWorldPosition = ConvertToWorldPosition(_primaryTouch.position);

                pos = touchWorldPosition; // Temporary!

                Vector2 touchMoveDirection = (_primaryTouch.rawPosition - _primaryTouch.position).normalized;
                float touchMoveDistanceFromInitialPosition = Vector2.Distance(_primaryTouch.rawPosition, _primaryTouch.position);
                Vector2 touchMoveDelta = _primaryTouch.deltaPosition;
                float touchMoveSpeed = _primaryTouch.deltaPosition.magnitude / _primaryTouch.deltaTime;
                //_primaryTouch.tapCount;

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

                switch (_primaryTouch.phase) 
                {
                    case TouchPhase.Began:
                    
                    break;

                    case TouchPhase.Moved:
                    touchMoveSpeed = _primaryTouch.deltaPosition.magnitude / _primaryTouch.deltaTime;
                    touchMoveDirection = (_primaryTouch.rawPosition - _primaryTouch.position).normalized;
                    break;

                    case TouchPhase.Ended:
                    _touchEndPosition = _primaryTouch.position;

                    OnSwipe.Invoke(touchMoveDirection); // Example of an event invocation.
                    break;
                }

                Debug.Log("Phase: " + _primaryTouch.phase + " StartPos: " + _primaryTouch.rawPosition + " Pos: " + _primaryTouch.position 
                    + " Speed: " + touchMoveSpeed + " Dir: " + touchMoveDirection);

                if (_primaryTouch.rawPosition.x < Screen.width / 2) 
                {

                }
                else if (_primaryTouch.rawPosition.x > Screen.width / 2) 
                {
                    
                }
            }
            else if (Input.touchCount == 2)
            {
                // Touch 1
                _touch1 = Input.GetTouch(0);
                Vector3 touch1WorldPosition = ConvertToWorldPosition(_touch1.position);

                Vector2 touch1MoveDirection = (_touch1.rawPosition - _touch1.position).normalized;
                float touch1MoveDistanceFromInitialPosition = Vector2.Distance(_touch1.rawPosition, _touch1.position);
                Vector2 touch1MoveDelta = _touch1.deltaPosition;
                float touch1MoveSpeed = _touch1.deltaPosition.magnitude / _touch1.deltaTime;

                switch (_touch1.phase) 
                {
                    case TouchPhase.Began:
                    
                    break;

                    case TouchPhase.Moved:
                    touch1MoveSpeed = _primaryTouch.deltaPosition.magnitude / _primaryTouch.deltaTime;
                    touch1MoveDirection = (_primaryTouch.rawPosition - _primaryTouch.position).normalized;
                    break;

                    case TouchPhase.Ended:
                    _touchEndPosition = _primaryTouch.position;

                    OnSwipe.Invoke(touch1MoveDirection); // Example of an event invocation.
                    break;
                }

                // Touch 2
                _touch2 = Input.GetTouch(1);
                Vector3 touch2WorldPosition = ConvertToWorldPosition(_touch2.position);

                Vector2 touch2MoveDirection = (_touch2.rawPosition - _touch2.position).normalized;
                float touch2MoveDistanceFromInitialPosition = Vector2.Distance(_touch2.rawPosition, _touch2.position);
                Vector2 touch2MoveDelta = _touch2.deltaPosition;
                float touch2MoveSpeed = _touch2.deltaPosition.magnitude / _touch2.deltaTime;

                switch (_touch2.phase) 
                {
                    case TouchPhase.Began:
                    
                    break;

                    case TouchPhase.Moved:
                    touch2MoveSpeed = _primaryTouch.deltaPosition.magnitude / _primaryTouch.deltaTime;
                    touch2MoveDirection = (_primaryTouch.rawPosition - _primaryTouch.position).normalized;
                    break;

                    case TouchPhase.Ended:
                    _touchEndPosition = _primaryTouch.position;

                    OnSwipe.Invoke(touch2MoveDirection); // Example of an event invocation.
                    break;
                }

                if (_primaryTouch.rawPosition.x < Screen.width / 2) 
                {

                }
                else if (_primaryTouch.rawPosition.x > Screen.width / 2) 
                {
                    
                }
            }
       
           // Multiple Touches
                foreach (Touch touch in Input.touches) 
                {
                    Vector3 touchScreenPos = touch.position;
                    touchScreenPos.z = Camera.main.nearClipPlane;
                    if (touch.position.x < Screen.width / 2) 
                {
                    Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(touchScreenPos);
                    //Debug.Log("ScreenPos: " + touchScreenPos + " WorldPos: " + touchWorldPos);
                    Debug.DrawLine(Vector3.zero, touchWorldPos, Color.red);
                    pos = touchWorldPos;
                }
                else if (touch.position.x > Screen.width / 2) 
                {
                    Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(touchScreenPos);
                    //Debug.Log("ScreenPos: " + touchScreenPos + " WorldPos: " + touchWorldPos);
                    Debug.DrawLine(Vector3.zero, touchWorldPos, Color.blue);
                    pos = touchWorldPos;
                }     
                //UpdateLineRenderer();  

                float distance = Vector3.Distance(_lastFrameTouchPos, pos);
                //if (distance > _touchMoveTreshold) 
                if (touch.phase == TouchPhase.Moved)
                { 
                    isMoving = true; 
                }
                else 
                {
                    isMoving = false;
                }
                Debug.Log("Distance: " + distance + "IsMoving: " + isMoving);
                _lastFrameTouchPos = pos;
             }
        }
        else 
        {
            isTouching = false;
            StopCoroutine(AddLinePoint());
        }
    }

    private Vector3 ConvertToWorldPosition(Vector2 position) 
    {
        Vector3 touchScreenPosition = _primaryTouch.position;
        touchScreenPosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(_primaryTouch.position);        
    }

    private int positionCount = 0;
    private void UpdateLineRenderer() 
    {   
        _lineRenderer.positionCount = positionCount + 1;
        _lineRenderer.SetPosition(positionCount, new Vector3(pos.x, pos.y, 0));
        positionCount++;
    }

    private void DrawGesture() 
    {
        GameObject obj = Instantiate(new GameObject(), transform.position, Quaternion.identity, transform); // Both Instantiate and new GameObject creates a new gameObject in the scene.
        _lineRenderer = obj.AddComponent<LineRenderer>();
        positionCount = 0;

        // _lineRenderer.widthCurve = _lineCurve;
        // _lineRenderer.material = _lineMaterial;
        // _lineRenderer.positionCount = steps;
    }

    private IEnumerator AddLinePoint() 
    {
        while (true) 
        {
            //if (_lastFrameTouchPos == pos) yield return null;
            
            yield return new WaitForSecondsRealtime(_pointAddInterval);
            if (isMoving) 
            {
                _lineRenderer.positionCount = positionCount + 1;
                _lineRenderer.SetPosition(positionCount, new Vector3(pos.x, pos.y, 0));
                positionCount++;
            }
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(pos, 0.1f);
    }
}
