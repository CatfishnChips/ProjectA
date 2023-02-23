using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[ExecuteInEditMode]
public class Hitbox : MonoBehaviour, IHitDetector
{
    private int _part;

    [Header("Properties")]
    [SerializeField] private Vector2 m_offset;
    [SerializeField] private Vector2 m_size;
    [NotKeyable] [SerializeField] private LayerMask m_layerMask; // Determines which type of colliders this collider can interact with.
    [DiscreteEvaluation] [SerializeField] private ColliderState m_state;
    [NotKeyable] [SerializeField] private Color m_openColor, m_collidingColor;
    private BoxCollider2D[] _colliders = new BoxCollider2D[10];
    private Color _color {get { return m_state == ColliderState.Open ? m_openColor : m_collidingColor; }}

    [SerializeField] private HurtboxMask m_hurtboxMask = HurtboxMask.Enemy;
    private IHitResponder m_hitResponder;

    public IHitResponder HitResponder { get => m_hitResponder; set => m_hitResponder = value; }

    public void CheckHit(){
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + new Vector3(m_offset.x, m_offset.y, 0), m_size, 0, m_layerMask);

        CollisionData _collisionData = null;
        IHurtbox _hurtbox = null;
        foreach (Collider2D collider in colliders){
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
                            damage = m_hitResponder == null ? 0 : m_hitResponder.Damage,
                            hurtbox = _hurtbox,
                            hitDetector = this
                        };

                        // Validate & Response
                        if (_collisionData.Validate()) 
                        {
                            _collisionData.hitDetector.HitResponder?.Response(_collisionData);
                            _collisionData.hurtbox.HurtResponder?.Response(_collisionData);
                        }         
                    }
                }
            }
        }
    }
    
    private void Awake(){
        // Get the required references here.
    }

    // Using OnTriggerEnter / OnTriggerExit can cause performance issues.
    // May need to find an alternative way to handle collision events.

    public void StartCheckingCollision(){
        m_state = ColliderState.Open;
    }

    public void StopCheckingCollision(){
        m_state = ColliderState.Closed;
    }

    private void Update() {
        if (m_state == ColliderState.Closed) return;

        //if (m_target != null) transform.position = m_target.transform.position;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, m_size, 0, m_layerMask);

        for (int i = 0; i < _colliders.Length; i++) {

        BoxCollider2D aCollider = _colliders[i];
        }
    }

    // Update vs Coroutine
    private IEnumerator CheckForCollision() {
        yield return new WaitForFixedUpdate();
    }

    private void OnDrawGizmos(){
        Gizmos.color = _color;

        if (m_state == ColliderState.Closed) return;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        Gizmos.DrawWireCube(Vector3.zero + new Vector3(m_offset.x, m_offset.y, 0), new Vector3(m_size.x / 2, m_size.y / 2, 0));
    }
}
