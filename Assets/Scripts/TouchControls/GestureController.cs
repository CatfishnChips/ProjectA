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

    [SerializeField] private float _touchDistaceTresholdToMove = 1f;

    private TouchData _primaryTouch;
    private TouchData _secondaryTouch;

    private void OnEnable() 
    {
        _inputManager.OnTouchBegin += OnTouchBegin;
        _inputManager.OnTouchDrag += OnTouchDrag;
        _inputManager.OnTouchEnd += OnTouchEnd;

        //_inputManager.OnTouchBBegin += 
    }

    private void OnDisable()
    {
        _inputManager.OnTouchBegin -= OnTouchBegin;
        _inputManager.OnTouchDrag -= OnTouchDrag;
        _inputManager.OnTouchEnd -= OnTouchEnd;
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
        UpdateTouch();
    }

    private void OnTouchEnd(int touchID, Vector2 screenPosition, Vector3 worldPosition) 
    {
        RecognizeGesture();
    }

    private void AddTouch() 
    {

    }

    private void UpdateTouch() 
    {

    }

    private void RecognizeGesture() 
    {

    }

    private void RemoveTouch() 
    {

    }
}

public struct TouchData
{
    public int FingerID;
    public ScreenSide ScreenSide;
    public float DeltaSpeed;
    public float TimeOnScreen;
}

[System.Serializable]
public enum ScreenSide 
{
    Left,
    Right
}

[System.Serializable]
public enum TouchState
{
    Tap,
    Hold,
    Drag
}
