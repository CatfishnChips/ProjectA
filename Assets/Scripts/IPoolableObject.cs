using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolableObject
{   
    Queue<GameObject> QueueReference {get; set;}

    virtual void EnqueueObject(GameObject obj, Queue<GameObject> reference){
        ObjectPoolingManager.Instance.EnqueueObject(obj, reference);
    }
}

public interface IObjectPool
{
    List<PoolableObject> PoolableObjects {get;}

    virtual Queue<GameObject> CreatePool(GameObject prefab, int amount)
    {
        Queue<GameObject> queue = ObjectPoolingManager.Instance.CreatePool(prefab, amount);
        return queue;
    }

    virtual GameObject DequeueObject(GameObject prefab, Queue<GameObject> reference){
        var obj = ObjectPoolingManager.Instance.DequeueObject(prefab, reference);
        return obj;
    }
}

[System.Serializable]
public class PoolableObject
{
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private int m_amount;
    private Queue<GameObject> m_queueReference;

    public GameObject Prefab {get => m_prefab;}
    public int Amount {get => m_amount;}
    public Queue<GameObject> QueueReference {get{return m_queueReference;}  set{m_queueReference = value;}}
}
