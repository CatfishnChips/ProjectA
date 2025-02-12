using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[ExecuteAlways]
public class Hitbox : Boxes, IHitDetector
{
    [Header("Properties")]
    [NotKeyable] [SerializeField] private LayerMask m_layerMask; // Determines which type of colliders this collider can interact with.
    [NotKeyable] [SerializeField] private Color m_openColor, m_collidingColor;

    //private BoxCollider2D[] _colliders = new BoxCollider2D[10];
    
    private Color _color {get { return m_state == ColliderState.Open ? m_openColor : m_collidingColor; }}

    [SerializeField] private HurtboxMask m_hurtboxMask = HurtboxMask.Enemy;
    private IHitResponder m_hitResponder;

    public IHitResponder HitResponder { get => m_hitResponder; set => m_hitResponder = value; } 

    private CollisionData m_collisionData;
    private IHurtbox m_hurtbox;
    private Vector2 m_collisionPoint;

    public void CheckHit(){
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + new Vector3(m_offset.x * Mathf.Sign(transform.right.x), m_offset.y, 0), m_size, 0, m_layerMask);

        m_collisionData = default;
        m_hurtbox = null;

        foreach (Collider2D collider in colliders)
        {
            m_hurtbox = collider.GetComponent<IHurtbox>();
            if (m_hurtbox != null)
            {
                if (m_hurtbox.Active)
                {
                    if (m_hurtboxMask.HasFlag((HurtboxMask)m_hurtbox.Type)) 
                    {
                        // Calculate Collision Point
                        m_collisionPoint = collider.ClosestPoint(transform.position + new Vector3(m_offset.x * Mathf.Sign(transform.right.x), m_offset.y, 0));

                        // Generate Collision Data
                        m_collisionData = new CollisionData
                        {
                            action = m_hitResponder == null ? null : m_hitResponder.Action,
                            hurtbox = m_hurtbox,
                            hitbox = this,
                            collisionPoint = m_collisionPoint
                        };

                        // Validate & Response
                        if (m_collisionData.Validate()) 
                        {
                            m_collisionData.hitbox.HitResponder?.Response(m_collisionData);
                            m_collisionData.hurtbox.HurtResponder?.Response(m_collisionData);

                            return;       
                        }  
                    }
                }
            }
        }
    }

    private void OnDrawGizmos(){
        Gizmos.color = _color;

        if (m_state == ColliderState.Closed) return;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        Gizmos.DrawWireCube(Vector3.zero + new Vector3(m_offset.x, m_offset.y, 0), new Vector3(m_size.x, m_size.y, 0));
    }


#if UNITY_EDITOR
    public override void DrawHandles(Matrix4x4 matrix)
    {
        UnityEditor.Handles.color = _color;

        if (m_state == ColliderState.Closed) return;

        UnityEditor.Handles.matrix = matrix;

        UnityEditor.Handles.DrawWireCube(Vector3.zero + new Vector3(m_offset.x, m_offset.y, 0), new Vector3(m_size.x, m_size.y, 0));
    }

#endif
}

// CheckHit for multiple colliders: 

// Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + new Vector3(m_offset.x, m_offset.y, 0), m_size, 0, m_layerMask);

//         CollisionData _collisionData = null;
//         IHurtbox _hurtbox = null;
//         foreach (Collider2D collider in colliders){
//             _hurtbox = collider.GetComponent<IHurtbox>();
//             if (_hurtbox != null)
//             {
//                 if (_hurtbox.Active)
//                 {
//                     if (m_hurtboxMask.HasFlag((HurtboxMask)_hurtbox.Type)) 
//                     {
//                         // Generate Collision Data
//                         _collisionData = new CollisionData
//                         {
//                             damage = m_hitResponder == null ? 0 : m_hitResponder.Damage,
//                             hurtbox = _hurtbox,
//                             hitDetector = this
//                         };

//                         // Validate & Response
//                         if (_collisionData.Validate()) 
//                         {
//                             _collisionData.hitDetector.HitResponder?.Response(_collisionData);
//                             _collisionData.hurtbox.HurtResponder?.Response(_collisionData);
//                         }         
//                     }
//                 }
//             }
//         }
