using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FighterStateMachine))]
public class ParticleEffectManager : MonoBehaviour, IObjectPool
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
        return obj;
    }

    #endregion


    private void Start(){
        foreach (PoolableObject poolableObject in PoolableObjects){
            poolableObject.QueueReference = CreatePool(poolableObject.Prefab, poolableObject.Amount);
        }
    }
}
