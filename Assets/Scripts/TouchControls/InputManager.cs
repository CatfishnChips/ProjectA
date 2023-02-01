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


    private Vector2 _touchEndPosition;

    private Touch _primaryTouch;
    private Touch _touch1;
    private Touch _touch2;


    // Update is called once per frame
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

                Vector3 touchScreenPosition = _primaryTouch.position;
                touchScreenPosition.z = Camera.main.nearClipPlane;
                Vector3 touchWorldPosition = Camera.main.ScreenToWorldPoint(_primaryTouch.position);

                pos = touchWorldPosition; // Temporary!

                Vector2 touchMoveDirection = (_primaryTouch.rawPosition - _primaryTouch.position).normalized;
                float touchMoveDistanceFromInitialPosition = Vector2.Distance(_primaryTouch.rawPosition, _primaryTouch.position);
                Vector2 touchMoveDelta = _primaryTouch.deltaPosition;
                float touchMoveSpeed = _primaryTouch.deltaPosition.magnitude / _primaryTouch.deltaTime;
                //_primaryTouch.tapCount;

                // Instead of using Touch's own states, making our own states might be better.
                // Unity handles this automatically!
                if (_primaryTouch.deltaPosition != Vector2.zero) 
                {
                    _primaryTouch.phase = TouchPhase.Stationary;
                }
                else
                {
                    _primaryTouch.phase = TouchPhase.Moved;
                }

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

                    EventManager.Instance.OnSwipe.Invoke(touchMoveDirection); // Example of an event invocation.
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
