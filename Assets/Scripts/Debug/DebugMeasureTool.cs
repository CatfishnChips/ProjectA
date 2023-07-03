using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMeasureTool : MonoBehaviour
{
    [SerializeField] private int m_unit;

    private void OnDrawGizmos(){
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + m_unit, transform.position.y, transform.position.z));
    }
}
