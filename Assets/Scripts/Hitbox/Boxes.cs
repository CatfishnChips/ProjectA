using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[ExecuteAlways]
public abstract class Boxes : MonoBehaviour
{
    [SerializeField] protected Vector2 m_offset;
    [SerializeField] protected Vector2 m_size;
    [DiscreteEvaluation] [SerializeField] protected ColliderState m_state;

    public Vector2 Offset { get => m_offset; }
    public Vector2 Size { get => m_size; }
    public bool Active { get {return m_state != ColliderState.Closed ? true : false;} }
    public Transform Transform { get => transform; }

    public virtual void DrawHandles(Matrix4x4 matrix) { }
}
