using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TouchIndicatorController : MonoBehaviour
{
    [SerializeField] private Transform _touchIndicator1, _touchIndicator2;
    [SerializeField] private TextMeshProUGUI _touchText1, _touchText2;
    private InputManager _inputManager = InputManager.Instance;

    private float _touchAHoldTime, _touchBHoldTime;

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
        Vector3 position = new Vector3(worldPosition.x, worldPosition.y, -9f);
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
        Vector3 position = new Vector3(inputEventParams.WorldPosition.x, inputEventParams.WorldPosition.y, -9f);
        _touchIndicator1.transform.position = position;
        _touchText1.SetText("Dragging");
        _touchAHoldTime = 0;
    }

    private void UpdateIndicator1(Vector2 screenPosition, Vector3 worldPosition) 
    {
        Vector3 position = new Vector3(worldPosition.x, worldPosition.y, -9f);
        _touchAHoldTime += Time.deltaTime;
        _touchText1.SetText("Hold - " + (int)_touchAHoldTime);
    }

    private void EnableIndicator2(Vector2 screenPosition, Vector3 worldPosition) 
    {
        Vector3 position = new Vector3(worldPosition.x, worldPosition.y, -9f);
        _touchIndicator2.gameObject.SetActive(true);
        _touchIndicator2.transform.position = position;
        _touchText2.SetText("Began");
        _touchBHoldTime = 0;
    }

    private void DisableIndicator2(InputEventParams inputEventParams) 
    {
        _touchText2.SetText("Ended");
        _touchIndicator2.gameObject.SetActive(false);
    }

    private void MoveIndicator2(InputEventParams inputEventParams) 
    {
        Vector3 position = new Vector3(inputEventParams.WorldPosition.x, inputEventParams.WorldPosition.y, -9f);
        _touchIndicator2.transform.position = position;
        _touchText2.SetText("Dragging");
        _touchBHoldTime = 0;
    }

    private void UpdateIndicator2(Vector2 screenPosition, Vector3 worldPosition) 
    {
        Vector3 position = new Vector3(worldPosition.x, worldPosition.y, -9f);
        _touchBHoldTime += Time.deltaTime;
        _touchText2.SetText("Hold - " + (int)_touchBHoldTime);
    }
}
