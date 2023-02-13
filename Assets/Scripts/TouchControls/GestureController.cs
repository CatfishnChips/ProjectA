using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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
    
    //[Header("Touch Settings")]
    //[SerializeField] private float _tapTime; // Time needed for distinguishing Tap from Hold. !!! Currently not used !!!
    //[SerializeField] private float _slowTapTime; // !!! Currently not used !!!
    //[SerializeField] private float _holdTime; // Time needed for the touch to register as Hold. Used to distinguish Tap from Hold.

    #region Touch A Variables

    [Header("Swipe Settings")]
    [SerializeField] private float _swipeDistance; // Minimum distance required to register as a swipe. // Can also use Screen.width and Screen.height.
    [SerializeField] private float _swipeTimeout; // Maximum time a touch can be on-screen before not registering as a swipe.  
    [SerializeField] private float _swipeDelta; // Minimum speed value before not registering as a swipe.
    private bool _isSwipe; // Is input a swipe or not?

    // Alternative version of isSwipe but requires the TouchData implementation to change.
    // private bool isSwipe {
    //     get 
    //     {
    //         return true;
    //     }
    // }

    [Header("Joystick Settings")]
    [SerializeField] private float _sensitivity; // Active area of the Joystick.
    private Vector2 _virtualJoystick; 
    private float _deltaVectorX;

    #endregion

    #region Touch B Variables

    [Header("Gesture Settings")]
    [SerializeField] private float _scoreTreshold = 0.25f; // Least amount of score required for a gesture to be recognized.
    [SerializeField] private float _pointAddInterval;
    private float _pointAddTimer;
    private List<Vector2> _pointList = new List<Vector2>();
    private DollarRecognizer _recognizer = new DollarRecognizer();

    [Header("UI References")]
    [SerializeField] private TMP_InputField _nameInput; // Temporary
    [SerializeField] private TextMeshProUGUI _nameText, _scoreText; // Temporary

    private TouchData _touchA, _touchB;

    #endregion

    private void Start() 
    {
        InputManager.Instance.OnTouchABegin += OnTouchABegin;
        InputManager.Instance.OnTouchAStationary += OnTouchAStationary;
        InputManager.Instance.OnTouchADrag += OnTouchADrag;
        InputManager.Instance.OnTouchAEnd += OnTouchAEnd;

        InputManager.Instance.OnTouchBBegin += OnTouchBBegin;
        InputManager.Instance.OnTouchBStationary += OnTouchBStationary;
        InputManager.Instance.OnTouchBDrag += OnTouchBDrag;
        InputManager.Instance.OnTouchBEnd += OnTouchBEnd;

        ReadGesture();
    }

    private void OnDisable()
    {
        InputManager.Instance.OnTouchABegin -= OnTouchABegin;
        InputManager.Instance.OnTouchAStationary -= OnTouchAStationary;
        InputManager.Instance.OnTouchADrag -= OnTouchADrag;
        InputManager.Instance.OnTouchAEnd -= OnTouchAEnd;

        InputManager.Instance.OnTouchBBegin -= OnTouchBBegin;
        InputManager.Instance.OnTouchBStationary -= OnTouchBStationary;
        InputManager.Instance.OnTouchBDrag -= OnTouchBDrag;
        InputManager.Instance.OnTouchBEnd -= OnTouchBEnd;
    }

    private void Update() 
    {
        // Temporary (to limit point adding to a time interval)
        if (_pointAddTimer > 0) 
        {
            _pointAddTimer -= Time.deltaTime;
        }
    }

    #region Touch A

    private void OnTouchABegin(Vector2 screenPosition, Vector3 worldPosition) 
    {
        _touchA = new TouchData();
        _touchA.InitialScreenPosition = screenPosition;
        _touchA.InitialWorldPosition = worldPosition;

        // Swipe
        _isSwipe = true;

        // Joystick
        _virtualJoystick = Vector2.zero;
    }

    private void OnTouchAStationary(Vector2 screenPosition, Vector3 worldPosition)
    {
        _touchA.TimeOnScreen += Time.deltaTime;

        // Swipe
        //_isSwipe = false;
        if (!_isSwipe) EventManager.Instance.Walk?.Invoke(_deltaVectorX);

        //Joystick
        if (!_isSwipe)
        Debug.Log("Stationary - Joystick DeltaDistance: " + _deltaVectorX);
    }

    private void OnTouchADrag(InputEventParams inputEventDragParams) 
    {
        _touchA.TimeOnScreen += Time.deltaTime;
        
        // Swipe
        if (_isSwipe && inputEventDragParams.Delta.magnitude < _swipeDelta) _isSwipe = false;

        // Joystick
        if (!_isSwipe) 
        {
            _virtualJoystick.x += inputEventDragParams.Delta.x;
            _virtualJoystick.x = Mathf.Clamp(_virtualJoystick.x, -_sensitivity, _sensitivity);

            float deltaDistanceX = _virtualJoystick.x / _sensitivity; // Convert the distance to be within -1 and 1. Surely this can be done more efficiently.

            _deltaVectorX = deltaDistanceX;
            Debug.Log("Moving - Jostick Distance: " + _virtualJoystick.x + " DeltaDistance: " + deltaDistanceX);
            EventManager.Instance.Walk?.Invoke(_deltaVectorX);
        }
    }

    private void OnTouchAEnd(InputEventParams inputEventParams) 
    {
        float distance = Vector2.Distance(_touchA.InitialScreenPosition, inputEventParams.ScreenPosition);
        Vector2 direction = (_touchA.InitialScreenPosition - inputEventParams.ScreenPosition).normalized;

        // Joystick
        _deltaVectorX = 0;
        EventManager.Instance.Walk?.Invoke(_deltaVectorX);

        // Swipe
        if (distance < _swipeDistance) return;
        if (_touchA.TimeOnScreen >= _swipeTimeout) return;
        if (inputEventParams.Delta.magnitude < _swipeDelta) return;
        if (!_isSwipe) return;
        EventManager.Instance.Dash?.Invoke(direction);

        Debug.Log("Swipe! " + direction);
    }

    #endregion

    #region Touch B

    private void OnTouchBBegin(Vector2 screenPosition, Vector3 worldPosition) 
    {
        _touchB = new TouchData();
        _touchB.InitialScreenPosition = screenPosition;
        _touchB.InitialWorldPosition = worldPosition;

        _pointList.Clear();
        _pointAddTimer = _pointAddInterval;
        _pointList.Add(_touchB.InitialScreenPosition);
    }

    private void OnTouchBStationary(Vector2 screenPosition, Vector3 worldPosition)
    {

    }

    private void OnTouchBDrag(InputEventParams inputEventDragParams) 
    {
        if (_pointAddTimer <= 0) 
        {
            _pointList.Add(inputEventDragParams.ScreenPosition);
            _pointAddTimer = _pointAddInterval;
        }
    }

    private void OnTouchBEnd(InputEventParams inputEventParams) 
    {
        float distance = Vector2.Distance(_touchB.InitialScreenPosition, inputEventParams.ScreenPosition);
        Vector2 direction = (_touchB.InitialScreenPosition - inputEventParams.ScreenPosition).normalized;

        _pointList.Add(inputEventParams.ScreenPosition);

        RecognizeGesture(out string Name, out float Score);

        if (Score < _scoreTreshold) return; 

        switch(Name) 
        {
            case "Uppercut":

            break;

            case "Punch":

            break;
        }
    }

    #endregion

    public void RecordGesture() 
    {   
        string name = _nameInput.text;
        _recognizer.SavePattern(name, _pointList);
    }

    public void WriteGesture() 
    {
        string name = _nameInput.text;
        DollarRecognizer.Unistroke unistroke = _recognizer.SavePattern(name, _pointList);
        string gestureName = _nameInput.text;
        string fileName = string.Format("{0}/{1}-{2}.xml", Application.persistentDataPath, gestureName, DateTime.Now.ToFileTime());
        GestureIO.WriteGesture(unistroke, gestureName, fileName);
    }

    private void ReadGesture() 
    {
        //Load pre-made gestures
		TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("Gestures/");
		foreach (TextAsset gestureXml in gesturesXml)
			_recognizer.AddToLibrary(GestureIO.ReadGestureFromXML(gestureXml.text));
            //Add loaded gestures to library.
    }

    private void RecognizeGesture(out string Name, out float Score) 
    {
        for (int i = 0; i < _pointList.Count; i++)
        {
            if (i-1 >= 0)
            Debug.DrawLine(new Vector3(_pointList[i-1].x, _pointList[i-1].y, 10), new Vector3(_pointList[i].x, _pointList[i].y, 10), Color.green, 3);
        }
        DollarRecognizer.Result result = _recognizer.Recognize(_pointList);

        if (result.Match != null) {
            Name = result.Match.Name;
            Score = result.Score;

            _nameText.text = result.Match.Name.ToString();
            _scoreText.text = result.Score.ToString();
        }
        else 
        {
            Name = "";
            Score = 0;
        }
     
    }
}

public struct TouchData
{
    public Vector2 InitialScreenPosition;
    public Vector3 InitialWorldPosition;
    public float TimeOnScreen;

    public TouchData(Vector2 initScreenPos, Vector3 initWorldPos, float timeOnScreen) => 
        (InitialScreenPosition, InitialWorldPosition, TimeOnScreen) = (initScreenPos, initWorldPos, timeOnScreen);
}

// public enum TouchState
// {
//     Tap,
//     Hold,
//     Drag
// }
