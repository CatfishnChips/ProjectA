using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public abstract class Boxes : MonoBehaviour
{
    [SerializeField] protected Vector2 m_offset;
    [SerializeField] protected Vector2 m_size;
    [DiscreteEvaluation] [SerializeField] protected ColliderState m_state;

    public abstract void DrawHandles(Matrix4x4 matrix);
}
