using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    private Vector3 pos;
    private bool isTouching;
    private bool isMoving;
    [SerializeField] private float _pointAddInterval = 0.1f;
    [SerializeField] private float _touchMoveTreshold = 0.1f;
    private Vector3 _lastFrameTouchPos;

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

            // Single Touch
            // Touch touch = Input.GetTouch(0);
            // Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            // touchPos.z = 0;

            //pos = touchPos;

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
