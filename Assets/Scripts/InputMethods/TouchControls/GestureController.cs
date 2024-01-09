using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GestureController : InputInvoker
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
    [SerializeField] private float _sensitivity = 200f; // Active area of the Joystick.
    [SerializeField] private float _deadzone = 50f; // Minimum distance needed before registering the input.
    private Vector2 _virtualJoystick; 
    private float _deltaVectorX;

    #endregion

    #region Touch B Variables

    [Header("Gesture Settings")]
    [SerializeField] private float _gestureTimeout = 1f; // Screen time before touch losing the ability to register a gesture.
    [SerializeField] private float _scoreTreshold = 0.25f; // Least amount of score required for a gesture to be recognized.
    [SerializeField] private float _holdTime = 0.25f; // Minimum amount of time required to register a stationary touch as holding.
    [SerializeField] private float _pointAddInterval; // Currently not used. Only limit add interval if there is performance problems.
    private bool _isTouchBActive;
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
        TouchInputReader.Instance.OnTouchABegin += OnTouchABegin;
        TouchInputReader.Instance.OnTouchAStationary += OnTouchAStationary;
        TouchInputReader.Instance.OnTouchADrag += OnTouchADrag;
        TouchInputReader.Instance.OnTouchAEnd += OnTouchAEnd;

        TouchInputReader.Instance.OnTouchBBegin += OnTouchBBegin;
        TouchInputReader.Instance.OnTouchBStationary += OnTouchBStationary;
        TouchInputReader.Instance.OnTouchBDrag += OnTouchBDrag;
        TouchInputReader.Instance.OnTouchBEnd += OnTouchBEnd;

        ReadGesture();
    }

    private void OnDisable()
    {
        TouchInputReader.Instance.OnTouchABegin -= OnTouchABegin;
        TouchInputReader.Instance.OnTouchAStationary -= OnTouchAStationary;
        TouchInputReader.Instance.OnTouchADrag -= OnTouchADrag;
        TouchInputReader.Instance.OnTouchAEnd -= OnTouchAEnd;

        TouchInputReader.Instance.OnTouchBBegin -= OnTouchBBegin;
        TouchInputReader.Instance.OnTouchBStationary -= OnTouchBStationary;
        TouchInputReader.Instance.OnTouchBDrag -= OnTouchBDrag;
        TouchInputReader.Instance.OnTouchBEnd -= OnTouchBEnd;
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

    private void OnTouchABegin(InputEventParams inputEventParams) 
    {
        _touchA = new TouchData();
        _touchA.InitialScreenPosition = inputEventParams.NormalizedScreenPosition;
        _touchA.InitialWorldPosition = inputEventParams.WorldPosition;

        _touchA.HasMoved = false;
        _touchA.HoldTime = 0f;

        // Swipe
        _isSwipe = true;

        // Joystick
        _virtualJoystick = Vector2.zero;
    }

    private void OnTouchAStationary(InputEventParams inputEventParams)
    {
        _touchA.TimeOnScreen += Time.deltaTime;
        _touchA.HoldTime += Time.deltaTime;

        if(_touchA.HoldTime > _holdTime){
            _inputEvents.OnHoldA?.Invoke(true);
        }
        else{
            _inputEvents.OnHoldA?.Invoke(false);
        }

        // Swipe
        //_isSwipe = false;
        if (!_isSwipe) _inputEvents.Move?.Invoke(_deltaVectorX);

        //Joystick
        //if (!_isSwipe)
        //Debug.Log("Stationary - Joystick DeltaDistance: " + _deltaVectorX);
    }

    private void OnTouchADrag(InputEventParams inputEventDragParams) 
    {   
        _touchA.HasMoved = true;
        _touchA.TimeOnScreen += Time.deltaTime;
        _touchA.HoldTime = 0f;

        _inputEvents.OnHoldA?.Invoke(false);
        
        // Swipe
        if (_isSwipe && inputEventDragParams.Delta.magnitude < _swipeDelta) _isSwipe = false;

        // Joystick
        if (!_isSwipe) 
        {
            _virtualJoystick.x += inputEventDragParams.Delta.x;
            _virtualJoystick.x = Mathf.Clamp(_virtualJoystick.x, -_sensitivity, _sensitivity);

            float deltaDistanceX = _virtualJoystick.x / _sensitivity;

            _deltaVectorX = deltaDistanceX;
            //Debug.Log("DeltaVector: " + _deltaVectorX + " JoystickX: " + _virtualJoystick.x);
            if (Mathf.Abs(_virtualJoystick.x) <= _deadzone) _deltaVectorX = 0f;
            _inputEvents.Move?.Invoke(_deltaVectorX);

            //Debug.Log("Moving - Jostick Distance: " + _virtualJoystick.x + " DeltaDistance: " + deltaDistanceX);     
        }
    }

    private void OnTouchAEnd(InputEventParams inputEventParams) 
    {
        //Thinking on implementing a more complex class to handle data and give precise result (for a move to be recorded as swipe, release of the finger shouldn't be waited)
        float distance = Vector2.Distance(_touchA.InitialScreenPosition, inputEventParams.ScreenPosition);
        Vector2 direction = (_touchA.InitialScreenPosition - inputEventParams.NormalizedScreenPosition).normalized;

        // Joystick
        _deltaVectorX = 0;
        _inputEvents.Move?.Invoke(_deltaVectorX);

        _inputEvents.OnHoldA?.Invoke(false);

        if (distance < _swipeDistance) _isSwipe = false;
        if (_touchA.TimeOnScreen >= _swipeTimeout) _isSwipe = false;
        if (inputEventParams.Delta.magnitude < _swipeDelta) _isSwipe = false;

        
        if (_isSwipe){
            // Swipe
            _inputEvents.Swipe?.Invoke(direction);
            //Debug.Log("Swipe! " + direction);
        }
        
        if(!_touchA.HasMoved){
            // Tap
            _inputEvents.OnTap?.Invoke();
        }
    }

    #endregion

    #region Touch B

    private void OnTouchBBegin(InputEventParams inputEventParams) 
    {
        _touchB = new TouchData();

        _touchB.HasMoved = false;
        _isTouchBActive = true;
        _touchB.HoldTime = 0f;

        _touchB.InitialScreenPosition = inputEventParams.NormalizedScreenPosition;
        _touchB.InitialWorldPosition = inputEventParams.WorldPosition;

        _pointList.Clear();
        _pointAddTimer = _pointAddInterval;
        _pointList.Add(_touchB.InitialScreenPosition);
    }

    private void OnTouchBStationary(InputEventParams inputEventParams)
    {
        _touchB.TimeOnScreen += Time.deltaTime;
        _touchB.HoldTime += Time.deltaTime;

        if (_touchB.HoldTime > _holdTime){

            if (_touchB.TimeOnScreen <= _gestureTimeout){
                if (!_isTouchBActive) return;
                _isTouchBActive = false;
                RecognizeGesture(out string Name, out float Score);

                if (Score < _scoreTreshold) return; 

                _inputEvents.AttackMove?.Invoke(Name);
            } 
            _inputEvents.OnHoldB?.Invoke(true);
        }
        else{
            _inputEvents.OnHoldB?.Invoke(false);
        }

        if (_pointAddTimer <= 0) 
        {
            _pointList.Add(inputEventParams.NormalizedScreenPosition);
            _pointAddTimer = _pointAddInterval;
        }
    }

    private void OnTouchBDrag(InputEventParams inputEventDragParams) 
    {   
        _touchB.HasMoved = true;
        _touchB.TimeOnScreen += Time.deltaTime;
        _touchB.HoldTime = 0f;

        _inputEvents.OnHoldB?.Invoke(false);

        if (_pointAddTimer <= 0) 
        {
            _pointList.Add(inputEventDragParams.NormalizedScreenPosition);
            _pointAddTimer = _pointAddInterval;
        }

        if (_touchB.TimeOnScreen >= _gestureTimeout){
            if (!_isTouchBActive) return;
            _isTouchBActive = false;
            RecognizeGesture(out string Name, out float Score);

            if (Score < _scoreTreshold) return; 
            _inputEvents.AttackMove?.Invoke(Name);
        } 
    }

    private void OnTouchBEnd(InputEventParams inputEventParams) 
    {
        //float distance = Vector2.Distance(_touchB.InitialScreenPosition, inputEventParams.ScreenPosition);
        //Vector2 direction = (_touchB.InitialScreenPosition - inputEventParams.ScreenPosition).normalized;
        _inputEvents.OnHoldB?.Invoke(false);

        if (!_isTouchBActive) return;

        if (_touchB.HasMoved){
            _pointList.Add(inputEventParams.NormalizedScreenPosition);

            RecognizeGesture(out string Name, out float Score);

            if (Score < _scoreTreshold) return; 
            Debug.Log("Gesture controller tried to invoke the event.");
            _inputEvents.AttackMove?.Invoke(Name);
        }
        else{
            string name = "Tap";
            Debug.Log("Gesture controller tried to invoke the event.");
            _inputEvents.AttackMove?.Invoke(name);
        }
    }

    #endregion

    public void RecordGesture() 
    {   
        string name = _nameInput.text;
        _recognizer.SavePattern(name, _pointList);

        // DEBUG
        string debugLog = $"{name} has been recorded as;";
        for(int i = 0; i < _pointList.Count; i++){
            debugLog += $"\n Point {i}: "  + _pointList[i].ToString();
        }
        Debug.Log(debugLog);
    }

    public void WriteGesture() 
    {
        string name = _nameInput.text;
        DollarRecognizer.Unistroke unistroke = _recognizer.SavePattern(name, _pointList);
        string gestureName = _nameInput.text;
        string fileName = string.Format("{0}/{1}-{2}.xml", Application.persistentDataPath, gestureName, DateTime.Now.ToFileTime());
        //GestureIO.WriteGesture(unistroke, gestureName, fileName);
        GestureIO.WriteGesture(_pointList.ToArray(), gestureName, fileName);

        // DEBUG
        string debugLog = $"{gestureName} has been written as;";
        for(int i = 0; i < unistroke.Points.Length; i++){
            debugLog += $"\n Point {i}: "  + unistroke.Points[i].ToString();
        }
        Debug.Log(debugLog);
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

        // DEBUG
        string debugLog = $"Following points have been used to recognize the gesture {Name}";
        for(int i = 0; i < _pointList.Count; i++){
            debugLog += $"\n Point {i}: "  + _pointList[i].ToString();
        }
        //Debug.Log(debugLog);
    }

}

public struct TouchData
{
    public Vector2 InitialScreenPosition;
    public Vector3 InitialWorldPosition;
    public float TimeOnScreen;
    public float HoldTime;
    public bool HasMoved;
    public TouchState State;

    public TouchData(Vector2 initScreenPos, Vector3 initWorldPos, float timeOnScreen, float holdTime, bool hasMoved, TouchState state) => 
        (InitialScreenPosition, InitialWorldPosition, TimeOnScreen, HoldTime, HasMoved, State) = (initScreenPos, initWorldPos, timeOnScreen, holdTime, hasMoved, state);
}

public enum TouchState
{
    Tap,
    Hold,
    Drag,
    Swipe
}
