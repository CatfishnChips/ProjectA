using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DrawLine : MonoBehaviour
{
    public GameObject linePrefab;
    GameObject currentActiveLine;
    public MouseInputListener mouseInputListener;
    Coroutine drawCoroutine;

    public void StartLine()
    {
        if(currentActiveLine != null)
        {
            Destroy(currentActiveLine);
        }
        if(drawCoroutine != null)
        {
            StopCoroutine(drawCoroutine);
        }

        drawCoroutine = StartCoroutine(Draw());
    }

    public void FinishLine()
    {
        StopCoroutine(drawCoroutine);
    }

    IEnumerator Draw()
    {
        currentActiveLine = Instantiate(linePrefab);
        LineRenderer line = currentActiveLine.GetComponent<LineRenderer>();
        line.positionCount = 0;

        while(true)
        {
            Vector3 position = mouseInputListener.mouseWorldPos;
            position.z = 0;
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, position);
            yield return null;
        }
    }
}
