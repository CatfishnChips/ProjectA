using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[ExecuteAlways]
public class Hurtbox : MonoBehaviour, IHurtbox
{
    [NotKeyable] [SerializeField] private FighterStateMachine m_owner = null;
    [NotKeyable] [SerializeField] private HurtboxType m_hurtboxType = HurtboxType.Enemy;
    private IHurtResponder m_hurtResponder;

    [DiscreteEvaluation] [SerializeField] private ColliderState m_state = ColliderState.Open;
    [NotKeyable] [SerializeField] private BoxCollider2D m_collider;
    [SerializeField] private Vector2 m_offset;
    [SerializeField] private Vector2 m_size;

    public bool Active { get {return m_state != ColliderState.Closed ? true : false;} }
    public FighterStateMachine Owner { get => m_owner; }
    public Transform Transform { get => transform; }
    public HurtboxType Type { get => m_hurtboxType; }
    public IHurtResponder HurtResponder { get => m_hurtResponder; set => m_hurtResponder = value; }

    private void Awake(){
        m_owner = GetComponentInParent<FighterStateMachine>();
    }

    public bool CheckHit(CollisionData data){
        if (m_hurtResponder == null) 
            Debug.Log("No Responder");

        return true;
    }

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

        if (m_state == ColliderState.Closed) return;
        // Does nothing for now...
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.green;

        if (m_state == ColliderState.Closed) return;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        Gizmos.DrawWireCube(Vector3.zero + new Vector3(m_offset.x, m_offset.y, 0), new Vector3(m_size.x, m_size.y, 0));
    }
}
