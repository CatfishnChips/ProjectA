using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[ExecuteAlways]
public class Hitbox : MonoBehaviour, IHitDetector
{
    [Header("Properties")]
    [SerializeField] private Vector2 m_offset;
    [SerializeField] private Vector2 m_size;
    [NotKeyable] [SerializeField] private LayerMask m_layerMask; // Determines which type of colliders this collider can interact with.
    [DiscreteEvaluation] [SerializeField] private ColliderState m_state;
    [NotKeyable] [SerializeField] private Color m_openColor, m_collidingColor;
    //private BoxCollider2D[] _colliders = new BoxCollider2D[10];
    
    private Color _color {get { return m_state == ColliderState.Open ? m_openColor : m_collidingColor; }}

    [SerializeField] private HurtboxMask m_hurtboxMask = HurtboxMask.Enemy;
    private IHitResponder m_hitResponder;

    public IHitResponder HitResponder { get => m_hitResponder; set => m_hitResponder = value; } 

    public Vector2 Offset {get => m_offset;}
    public Vector2 Size {get => m_size;}
    public bool Active { get {return m_state != ColliderState.Closed ? true : false;} }

    public void CheckHit(){
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + new Vector3(m_offset.x * Mathf.Sign(transform.right.x), m_offset.y, 0), m_size, 0, m_layerMask);

        CollisionData _collisionData = null;
        IHurtbox _hurtbox = null;
        foreach (Collider2D collider in colliders)
        {
            _hurtbox = collider.GetComponent<IHurtbox>();
            if (_hurtbox != null)
            {
                if (_hurtbox.Active)
                {
                    if (m_hurtboxMask.HasFlag((HurtboxMask)_hurtbox.Type)) 
                    {
                        // Generate Collision Data
                        _collisionData = new CollisionData
                        {
                            action = m_hitResponder == null ? null : m_hitResponder.Action,
                            hurtbox = _hurtbox,
                            hitDetector = this
                        };

                        // Validate & Response
                        if (_collisionData.Validate()) 
                        {
                            _collisionData.hitDetector.HitResponder?.Response(_collisionData);
                            _collisionData.hurtbox.HurtResponder?.Response(_collisionData);

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
