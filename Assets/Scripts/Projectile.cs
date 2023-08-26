using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolableObject
{   
    #region IPoolableObject Interface

    private Queue<GameObject> m_queueReference;
    public Queue<GameObject> QueueReference {get {return m_queueReference;} set{m_queueReference = value;}}

    public void EnqueueObject(GameObject obj, Queue<GameObject> reference){
        ObjectPoolingManager.Instance.EnqueueObject(obj, reference);
    }

    #endregion

    [Header("Settings")]
    [SerializeField] private int m_duration; // In Frames
    [SerializeField] private float m_speed;
    [SerializeField] private Vector2 m_direction;
    private ActionAttack m_action;
    private HitResponder m_hitResponder;
    private Rigidbody2D m_rigidbody;

    public ActionAttack Action {get{return m_action;} set{m_action = value;}}
    public Vector2 Direction {get{return m_direction;} set{m_direction = value;}}
    public HitResponder HitResponder {get{return m_hitResponder;}}

    private void Awake(){
        if (TryGetComponent(out Rigidbody2D rigidbody)) m_rigidbody = rigidbody;
        if (TryGetComponent(out HitResponder hitResponder)) m_hitResponder = hitResponder;
    }

    private void OnEnable(){
        if(m_hitResponder) m_hitResponder.HitResponse += OnHit;
        if(m_hitResponder && m_action) m_hitResponder.UpdateData(m_action);
        //Debug.Log("Script: Projectile - OnEnable - Time: " + Time.timeSinceLevelLoad);
    }

    private void OnDisable(){
        if(m_hitResponder) m_hitResponder.HitResponse -= OnHit;
    }

    private void OnHit(CollisionData data){
        EnqueueObject(gameObject, QueueReference);
    }
    
    private void Start(){
        
    }

    private void FixedUpdate(){
        m_rigidbody.velocity = m_direction * m_speed * Time.fixedDeltaTime;
    }
}
