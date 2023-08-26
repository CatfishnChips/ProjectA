using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolingManager : MonoBehaviour
{
   #region Singleton

    public static ObjectPoolingManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #endregion

    // Note: Alternatively Unity's own pooling system can be used instead. UnityEngine.Pool

    [SerializeField] private List<Queue<GameObject>> _poolableObjectQueue = new List<Queue<GameObject>>();
    private List<ObjectPool<GameObject>> _pools = new List<ObjectPool<GameObject>>();

    public Queue<GameObject> CreatePool(GameObject prefab, int amount){
        Queue<GameObject> queue = new Queue<GameObject>();
        _poolableObjectQueue.Add(queue);

        for (int a = 0; a < amount; a++)
        {
            var obj = Instantiate(prefab, transform, false);
            obj.GetComponent<IPoolableObject>().QueueReference = queue;
            obj.SetActive(false);
            queue.Enqueue(obj);
        }

        return queue;
    }

    public void CreatePool2(GameObject prefab, int amount){
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreateObject, GetObject, ReleaseObject, DestroyObject, false, amount);
        
        _pools.Add(pool);
    }

    private GameObject CreateObject(){
        GameObject obj = Instantiate(gameObject, transform, false);
        return obj;
    }

    private void GetObject(GameObject obj){
        obj.SetActive(true);
    }

    private void ReleaseObject(GameObject obj){
        obj.SetActive(false);
    }

    private void DestroyObject(GameObject obj){
        Destroy(obj);
    }

    public void EnqueueObject(GameObject obj, Queue<GameObject> reference)
    {
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localEulerAngles = Vector3.zero;

        obj.gameObject.SetActive(false);
        reference.Enqueue(obj);
        //Debug.Log("ObjectPoolingManager - Object queued: " + obj.name);
    }

    public GameObject DequeueObject(GameObject prefab, Queue<GameObject> reference)
    {
        // If there are no remaining poolable objects, initialize more.
        if (reference.Count <= 0) {
            var obj = Instantiate(prefab, transform, false);
            obj.GetComponent<IPoolableObject>().QueueReference = reference;
            obj.SetActive(false);
            reference.Enqueue(obj);
        }

        var deQueuedPoolObject = reference.Dequeue();
        //Debug.Log("ObjectPoolingManager - Object de-queued: " + deQueuedPoolObject.name);
        deQueuedPoolObject.SetActive(true);
        return deQueuedPoolObject;
    }
}
