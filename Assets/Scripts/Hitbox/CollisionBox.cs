using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[ExecuteAlways]
public class CollisionBox : MonoBehaviour
{
    [DiscreteEvaluation] [SerializeField] private ColliderState m_state = ColliderState.Open;
    [NotKeyable] [SerializeField] private BoxCollider2D m_collider;
    [SerializeField] private Vector2 m_offset;
    [SerializeField] private Vector2 m_size;

    private void Start(){
        if (m_collider != null){
            m_collider.offset = m_offset;
            m_collider.size = m_size;
        } 
    }

    private void Update(){
        if (m_collider != null){
            m_collider.offset = m_offset;
            m_collider.size = m_size;
        } 

        if (m_state == ColliderState.Closed)
        m_collider.enabled = false;
        else 
        m_collider.enabled = true;
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.yellow;

        if (m_state == ColliderState.Closed) return;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        Gizmos.DrawWireCube(Vector3.zero + new Vector3(m_offset.x, m_offset.y, 0), new Vector3(m_size.x, m_size.y, 0));
    }
}
