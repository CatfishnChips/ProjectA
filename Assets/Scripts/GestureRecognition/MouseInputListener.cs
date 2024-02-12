using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MouseInputListener : MonoBehaviour
{
    public Vector3 mouseWorldPos;
    public TMP_Text mouseInfoText;
    DrawLine drawLine;

    bool isDrawing;

    public UnityAction StartedDrawing;
    public UnityAction CurrentlyDrawing;
    public UnityAction FinishedDrawing;

    void Start()
    {
        drawLine = transform.GetComponent<DrawLine>();
        isDrawing = false;

        Debug.Log(Screen.width);
    }

    void Update()
    {

        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        string mouseInfo = $"Mouse World Position: ({mouseWorldPos.x}, {mouseWorldPos.y}) \nMouse Screen Position: ({Input.mousePosition.x}, {Input.mousePosition.y}) \nScreen size: {Screen.width}";

        mouseInfoText.text = mouseInfo;

        if(Input.mousePosition.x > Screen.width / 4)
        {
            if(Input.GetMouseButtonDown(0))
            {
                drawLine.StartLine();
                isDrawing = true;
                StartedDrawing?.Invoke();
            }
            else if(Input.GetMouseButtonUp(0))
            {
                drawLine.FinishLine();
                isDrawing = false;
                FinishedDrawing?.Invoke();
            }

            if(isDrawing) CurrentlyDrawing?.Invoke();
        }
    }
}
