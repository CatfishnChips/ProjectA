using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DebugUITransform : MonoBehaviour
{
    [SerializeField] [ReadOnly] private Vector3 m_worldPosition;

    private void Update(){
        m_worldPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
}
