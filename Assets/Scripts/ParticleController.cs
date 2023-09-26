using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour, IPoolableObject
{
    #region IPoolableObject Interface

    private Queue<GameObject> m_queueReference;
    public Queue<GameObject> QueueReference {get {return m_queueReference;} set{m_queueReference = value;}}

    public void EnqueueObject(GameObject obj, Queue<GameObject> reference){
        ObjectPoolingManager.Instance.EnqueueObject(obj, reference);
    }

    #endregion

    private ParticleSystem m_particleSystem;

    private void Awake(){
        m_particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleSystemStopped(){
        EnqueueObject(gameObject, QueueReference);
    }
}
