using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour, IObjectPool
{
    #region  IObjectPool Interface

    [SerializeField] List<PoolableObject> m_poolableObjects = new List<PoolableObject>();
    public List<PoolableObject> PoolableObjects { get => m_poolableObjects; }

    public Queue<GameObject> CreatePool(GameObject prefab, int amount)
    {
        Queue<GameObject> queue = ObjectPoolingManager.Instance.CreatePool(prefab, amount);
        return queue;
    }

    public GameObject DequeueObject(GameObject prefab, Queue<GameObject> reference){
        var obj = ObjectPoolingManager.Instance.DequeueObject(prefab, reference);
        obj.transform.position = m_spawnLocation.position;
        return obj;
    }

    #endregion

    [SerializeField] private Transform m_spawnLocation;

    private void Awake(){
        foreach (PoolableObject poolableObject in PoolableObjects){
            poolableObject.QueueReference = CreatePool(poolableObject.Prefab, poolableObject.Amount);
        }
    }
    
    private void Start(){
        
    }
}
