using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureController : MonoBehaviour
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

    private InputManager _inputManager = InputManager.Instance;

    [SerializeField] private float _tapTime; // Time needed for distinguishing Tap from Hold.
    [SerializeField] private float _slowTapTime; // 
    [SerializeField] private float _holdTime; // Time needed for the touch to register as Hold. Used to distinguish Tap from Hold.

    private TouchData _touchA, _touchB;

    private void Start() 
    {
        _inputManager.OnTouchBegin += OnTouchBegin;
        _inputManager.OnTouchDrag += OnTouchDrag;
        _inputManager.OnTouchEnd += OnTouchEnd;

        _inputManager.OnTouchABegin += OnTouchABegin;
        _inputManager.OnTouchAStationary += OnTouchAStationary;
        _inputManager.OnTouchADrag += OnTouchADrag;
        _inputManager.OnTouchAEnd += OnTouchAEnd;

        _inputManager.OnTouchBBegin += OnTouchBBegin;
        _inputManager.OnTouchBStationary += OnTouchBStationary;
        _inputManager.OnTouchBDrag += OnTouchBDrag;
        _inputManager.OnTouchBEnd += OnTouchBEnd;

        //_inputManager.OnTouchBBegin += 
    }

    private void OnDisable()
    {
        _inputManager.OnTouchBegin -= OnTouchBegin;
        _inputManager.OnTouchDrag -= OnTouchDrag;
        _inputManager.OnTouchEnd -= OnTouchEnd;

        _inputManager.OnTouchABegin -= OnTouchABegin;
        _inputManager.OnTouchAStationary -= OnTouchAStationary;
        _inputManager.OnTouchADrag -= OnTouchADrag;
        _inputManager.OnTouchAEnd -= OnTouchAEnd;

        _inputManager.OnTouchBBegin -= OnTouchBBegin;
        _inputManager.OnTouchBStationary -= OnTouchBStationary;
        _inputManager.OnTouchBDrag -= OnTouchBDrag;
        _inputManager.OnTouchBEnd -= OnTouchBEnd;
    }

    private void OnTouchBegin(int touchID, Vector2 screenPosition, Vector3 worldPosition) 
    {
        // Screen Touch Side
        if (screenPosition.x < Screen.width / 2) 
        {
            EventManager.Instance.LeftSideTap.Invoke();
        }
        else if (screenPosition.x > Screen.width / 2) 
        {
            EventManager.Instance.RightSideTap.Invoke();
        }
    }

    private void OnTouchDrag(int touchID, Vector2 screenPosition, Vector3 worldPosition) 
    {
        
    }

    private void OnTouchEnd(int touchID, Vector2 screenPosition, Vector3 worldPosition) 
    {
        RecognizeGesture();
    }

    #region Touch A

    private void OnTouchABegin(Vector2 screenPosition, Vector3 worldPosition) 
    {
        _touchA = new TouchData();
        _touchA.InitialScreenPosition = screenPosition;
        _touchA.InitialWorldPosition = worldPosition;
    }

    private void OnTouchAStationary(Vector2 screenPosition, Vector3 worldPosition)
    {

    }

    private void OnTouchADrag(InputDragEventParams inputEventDragParams) 
    {

    }

    private void OnTouchAEnd(Vector2 screenPosition, Vector3 worldPosition) 
    {
        float distance = Vector2.Distance(_touchA.InitialScreenPosition, screenPosition);
        Vector2 direction = (_touchA.InitialScreenPosition - screenPosition).normalized;
    }

    #endregion

    #region Touch B

    private void OnTouchBBegin(Vector2 screenPosition, Vector3 worldPosition) 
    {
        _touchB = new TouchData();
        _touchB.InitialScreenPosition = screenPosition;
        _touchB.InitialWorldPosition = worldPosition;
    }

    private void OnTouchBStationary(Vector2 screenPosition, Vector3 worldPosition)
    {

    }

    private void OnTouchBDrag(InputDragEventParams inputEventDragParams) 
    {

    }

    private void OnTouchBEnd(Vector2 screenPosition, Vector3 worldPosition) 
    {
        float distance = Vector2.Distance(_touchB.InitialScreenPosition, screenPosition);
        Vector2 direction = (_touchB.InitialScreenPosition - screenPosition).normalized;
    }

    #endregion

    //public Gesture test = new Gesture(new Point[4] , "Swipe");
    public DollarRecognizer _recognizer = new DollarRecognizer();

    public void RecordGesture(string Name) 
    {
        //_recognizer.SavePattern(Name, new IEnumerable<Vector2> points)
    }

    private void RecognizeGesture() 
    {
        //_recognizer.Recognize();
    }
}

[System.Serializable]
public struct TouchData
{
    public Vector2 InitialScreenPosition;
    public Vector3 InitialWorldPosition;
    public float DeltaSpeed;
    public float TimeOnScreen;

    public TouchData(Vector2 initScreenPos, Vector3 initWorldPos, float deltaSpeed, float timeOnScreen) => 
        (InitialScreenPosition, InitialWorldPosition, DeltaSpeed, TimeOnScreen) = (initScreenPos, initWorldPos, deltaSpeed, timeOnScreen);
}

[System.Serializable]
public enum TouchState
{
    Tap,
    Hold,
    Drag
}
