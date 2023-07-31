using UnityEngine;

[ExecuteAlways]
public class ConstraintRotation2D : MonoBehaviour
{
    private int m_direction;
    void LateUpdate()
    {
        m_direction = (int)Mathf.Sign(transform.parent.forward.x);
        if (m_direction > 0)
        transform.rotation = Quaternion.Euler(0, 0, 0);
        else 
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
