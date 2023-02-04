using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;

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

    [SerializeField] private float _tapTime; // Time needed for distinguishing Tap from Hold.
    [SerializeField] private float _slowTapTime; // 
    [SerializeField] private float _holdTime; // Time needed for the touch to register as Hold. Used to distinguish Tap from Hold.

    [SerializeField] private float _scoreTreshold = 0.25f; // Least amount of score required for a gesture to be recognized.
    [SerializeField] private float _pointAddInterval;
    private float _pointAddTimer;
    [SerializeField] private List<Vector2> _pointList;

    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TextMeshProUGUI _nameText, _scoreText;

    private TouchData _touchA, _touchB;

    private void Start() 
    {
        InputManager.Instance.OnTouchBegin += OnTouchBegin;
        InputManager.Instance.OnTouchDrag += OnTouchDrag;
        InputManager.Instance.OnTouchEnd += OnTouchEnd;

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
        InputManager.Instance.OnTouchBegin -= OnTouchBegin;
        InputManager.Instance.OnTouchDrag -= OnTouchDrag;
        InputManager.Instance.OnTouchEnd -= OnTouchEnd;

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
        if (_pointAddTimer > 0) 
        {
            _pointAddTimer -= Time.deltaTime;
        }
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

        _pointList.Clear();
        _pointAddTimer = _pointAddInterval;
        _pointList.Add(_touchB.InitialScreenPosition);
    }

    private void OnTouchBStationary(Vector2 screenPosition, Vector3 worldPosition)
    {

    }

    private void OnTouchBDrag(InputDragEventParams inputEventDragParams) 
    {
        if (_pointAddTimer <= 0) 
        {
            _pointList.Add(inputEventDragParams.ScreenPosition);
            _pointAddTimer = _pointAddInterval;
        }
    }

    private void OnTouchBEnd(Vector2 screenPosition, Vector3 worldPosition) 
    {
        float distance = Vector2.Distance(_touchB.InitialScreenPosition, screenPosition);
        Vector2 direction = (_touchB.InitialScreenPosition - screenPosition).normalized;

        _pointList.Add(screenPosition);

        RecognizeGesture();
    }

    #endregion

    //public Gesture test = new Gesture(new Point[4] , "Swipe");
    public DollarRecognizer _recognizer = new DollarRecognizer();
    //public GestureIO _readWrite = new GestureIO();

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

    private void RecognizeGesture() 
    {
        for (int i = 0; i < _pointList.Count; i++)
        {
            if (i-1 >= 0)
            Debug.DrawLine(new Vector3(_pointList[i-1].x, _pointList[i-1].y, 10), new Vector3(_pointList[i].x, _pointList[i].y, 10), Color.green, 3);
            Debug.Log("Added Line");
        }
        DollarRecognizer.Result result = _recognizer.Recognize(_pointList);
        if (result.Match == null) return;
        _nameText.text = result.Match.Name.ToString();
        _scoreText.text = result.Score.ToString();     
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
