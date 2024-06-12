using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TouchIndicatorController : MonoBehaviour
{   
    [Header("Line Renderer")]
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private AnimationCurve _lineCurve;
    private GameObject _lineObject;
    private LineRenderer _lineRenderer;
    private int _pointCount;

    [Header("References")]
    [SerializeField] private Transform _touchIndicator1, _touchIndicator2;
    [SerializeField] private TextMeshProUGUI _touchText1, _touchText2;
    private TouchInputReader _inputManager = TouchInputReader.Instance;

    private float _touchAHoldTime, _touchBHoldTime;

    private void OnDisable() 
    {
        TouchInputReader.Instance.OnTouchABegin -= EnableIndicator1;
        TouchInputReader.Instance.OnTouchAEnd -= DisableIndicator1;
        TouchInputReader.Instance.OnTouchADrag -= MoveIndicator1;
        TouchInputReader.Instance.OnTouchAStationary -= UpdateIndicator1;

        TouchInputReader.Instance.OnTouchBBegin -= EnableIndicator2;
        TouchInputReader.Instance.OnTouchBEnd -= DisableIndicator2;
        TouchInputReader.Instance.OnTouchBDrag -= MoveIndicator2;
        TouchInputReader.Instance.OnTouchBStationary -= UpdateIndicator2;
    }

    void Start()
    {
        TouchInputReader.Instance.OnTouchABegin += EnableIndicator1;
        TouchInputReader.Instance.OnTouchAEnd += DisableIndicator1;
        TouchInputReader.Instance.OnTouchADrag += MoveIndicator1;
        TouchInputReader.Instance.OnTouchAStationary += UpdateIndicator1;

        TouchInputReader.Instance.OnTouchBBegin += EnableIndicator2;
        TouchInputReader.Instance.OnTouchBEnd += DisableIndicator2;
        TouchInputReader.Instance.OnTouchBDrag += MoveIndicator2;
        TouchInputReader.Instance.OnTouchBStationary += UpdateIndicator2;


        _touchIndicator1.gameObject.SetActive(false);
        _touchIndicator2.gameObject.SetActive(false);
    }

    private void EnableIndicator1(InputEventParams inputEventParams) 
    {   
        Vector3 position = new Vector3(inputEventParams.ScreenPosition.x, inputEventParams.ScreenPosition.y, -9f);
        _touchIndicator1.gameObject.SetActive(true);
        _touchIndicator1.transform.position = position;
        _touchText1.SetText("Began");
        _touchAHoldTime = 0;
    }

    private void DisableIndicator1(InputEventParams inputEventParams) 
    {
       _touchText1.SetText("Ended");
       _touchIndicator1.gameObject.SetActive(false);
    }

    private void MoveIndicator1(InputEventParams inputEventParams) 
    {   
        Vector3 position = new Vector3(inputEventParams.ScreenPosition.x, inputEventParams.ScreenPosition.y, -9f);
        _touchIndicator1.transform.position = position;
        _touchText1.SetText("Dragging");
        _touchAHoldTime = 0;
    }

    private void UpdateIndicator1(InputEventParams inputEventParams) 
    {
        Vector3 position = new Vector3(inputEventParams.ScreenPosition.x, inputEventParams.ScreenPosition.y, -9f);
        _touchAHoldTime += Time.deltaTime;
        _touchText1.SetText("Hold - " + (int)_touchAHoldTime);
    }

    private void EnableIndicator2(InputEventParams inputEventParams) 
    {
        Vector3 position = new Vector3(inputEventParams.ScreenPosition.x, inputEventParams.ScreenPosition.y, -9f);
        _touchIndicator2.gameObject.SetActive(true);
        _touchIndicator2.transform.position = position;
        _touchText2.SetText("Began");
        _touchBHoldTime = 0;

        EnableLine();
        AddPoint(new Vector3(inputEventParams.WorldPosition.x, inputEventParams.WorldPosition.y, -9f));
    }

    private void DisableIndicator2(InputEventParams inputEventParams) 
    {
        _touchText2.SetText("Ended");
        _touchIndicator2.gameObject.SetActive(false);

        DisableLine();
    }

    private void MoveIndicator2(InputEventParams inputEventParams) 
    {
        Vector3 position = new Vector3(inputEventParams.ScreenPosition.x, inputEventParams.ScreenPosition.y, -9f);
        _touchIndicator2.transform.position = position;
        _touchText2.SetText("Dragging");
        _touchBHoldTime = 0;

        AddPoint(new Vector3(inputEventParams.WorldPosition.x, inputEventParams.WorldPosition.y, -9f));
    }

    private void UpdateIndicator2(InputEventParams inputEventParams) 
    {
        Vector3 position = new Vector3(inputEventParams.ScreenPosition.x, inputEventParams.ScreenPosition.y, -9f);
        _touchBHoldTime += Time.deltaTime;
        _touchText2.SetText("Hold - " + (int)_touchBHoldTime);
    }

    #region Line Renderer

    private void EnableLine(){
         if (!_lineObject){
            _lineObject = new GameObject();
            //_lineObject.transform.SetParent(transform);
            //_lineObject.transform.localPosition = Vector3.zero;
            //_lineObject.transform.localScale = Vector3.one;
            _lineRenderer = _lineObject.AddComponent<LineRenderer>();
            _lineRenderer.sortingOrder = 8;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.widthCurve = _lineCurve;
            _lineRenderer.material = _lineMaterial;
            _lineRenderer.numCornerVertices = 5;
            _lineRenderer.numCapVertices = 5;
        } 
        _lineObject.SetActive(true);
        _pointCount = 0;
        _lineRenderer.positionCount = _pointCount;
    }

    private void AddPoint(Vector3 position){
        _pointCount++;
        _lineRenderer.positionCount = _pointCount;
        _lineRenderer.SetPosition(_pointCount - 1, position); 
    }

    private void DisableLine(){
        // Show a fading drawing of the gesture drawn.
        // Maybe make it slowly move towards the camera or grow in size while fading.
        // And also moving downwards to clear the screen like damage numbers.
        _lineObject.SetActive(false);
    }

    #endregion
}
