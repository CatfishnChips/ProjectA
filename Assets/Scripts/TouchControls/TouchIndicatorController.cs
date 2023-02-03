using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TouchIndicatorController : MonoBehaviour
{
    [SerializeField] private Transform _touchIndicator1, _touchIndicator2;
    [SerializeField] private TextMeshProUGUI _touchText1, _touchText2;
    [SerializeField] private Transform _trailObject1, _trailObject2;
    private TrailRenderer _trail1, _trail2;
    private InputManager _inputManager = InputManager.Instance;

    private float _touchAHoldTime, _touchBHoldTime;

    private void Awake() 
    {
        _trail1 = _trailObject1.GetComponent<TrailRenderer>();
        _trail2 = _trailObject2.GetComponent<TrailRenderer>();
    }

    private void OnDisable() 
    {
        InputManager.Instance.OnTouchABegin -= EnableIndicator1;
        InputManager.Instance.OnTouchAEnd -= DisableIndicator1;
        InputManager.Instance.OnTouchADrag -= MoveIndicator1;
        InputManager.Instance.OnTouchAStationary -= UpdateIndicator1;

        InputManager.Instance.OnTouchBBegin -= EnableIndicator2;
        InputManager.Instance.OnTouchBEnd -= DisableIndicator2;
        InputManager.Instance.OnTouchBDrag -= MoveIndicator2;
        InputManager.Instance.OnTouchBStationary -= UpdateIndicator2;
    }

    void Start()
    {
        InputManager.Instance.OnTouchABegin += EnableIndicator1;
        InputManager.Instance.OnTouchAEnd += DisableIndicator1;
        InputManager.Instance.OnTouchADrag += MoveIndicator1;
        InputManager.Instance.OnTouchAStationary += UpdateIndicator1;

        InputManager.Instance.OnTouchBBegin += EnableIndicator2;
        InputManager.Instance.OnTouchBEnd += DisableIndicator2;
        InputManager.Instance.OnTouchBDrag += MoveIndicator2;
        InputManager.Instance.OnTouchBStationary += UpdateIndicator2;


        _touchIndicator1.gameObject.SetActive(false);
        _touchIndicator2.gameObject.SetActive(false);
    }

    private void EnableIndicator1(Vector2 screenPosition, Vector3 worldPosition) 
    {   
        _touchIndicator1.gameObject.SetActive(true);
        _touchIndicator1.transform.position = screenPosition;
        _touchText1.SetText("Began");
        _touchAHoldTime = 0;
        _trailObject1.position = screenPosition;
        _trail1.emitting = true;
    }

    private void DisableIndicator1(Vector2 screenPosition, Vector3 worldPosition) 
    {
       _touchText1.SetText("Ended");
       _touchIndicator1.gameObject.SetActive(false);
       _trail1.emitting = false;
    }

    private void MoveIndicator1(InputDragEventParams inputEventDragParams) 
    {
        _touchIndicator1.transform.position = inputEventDragParams.ScreenPosition;;
        _touchText1.SetText("Dragging");
        _touchAHoldTime = 0;
        _trailObject1.position = inputEventDragParams.ScreenPosition;
    }

    private void UpdateIndicator1(Vector2 screenPosition, Vector3 worldPosition) 
    {
        _touchAHoldTime += Time.deltaTime;
        _touchText1.SetText("Hold - " + (int)_touchAHoldTime);
        _trailObject1.position = screenPosition;
    }

    private void EnableIndicator2(Vector2 screenPosition, Vector3 worldPosition) 
    {
        _touchIndicator2.gameObject.SetActive(true);
        _touchIndicator2.transform.position = screenPosition;
        _touchText2.SetText("Began");
        _touchBHoldTime = 0;
        _trailObject2.position = screenPosition;
        _trail1.emitting = true;
    }

    private void DisableIndicator2(Vector2 screenPosition, Vector3 worldPosition) 
    {
        _touchText2.SetText("Ended");
        _touchIndicator2.gameObject.SetActive(false);
        _trail2.emitting = false;
    }

    private void MoveIndicator2(InputDragEventParams inputEventDragParams) 
    {
        _touchIndicator2.transform.position = inputEventDragParams.ScreenPosition;
        _touchText2.SetText("Dragging");
        _touchBHoldTime = 0;
        _trailObject2.position = inputEventDragParams.ScreenPosition;
    }

    private void UpdateIndicator2(Vector2 screenPosition, Vector3 worldPosition) 
    {
        _touchBHoldTime += Time.deltaTime;
        _touchText2.SetText("Hold - " + (int)_touchBHoldTime);
        _trailObject2.position = screenPosition;
    }
}
