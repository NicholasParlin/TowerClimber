using System.Collections.Generic;
using UnityEngine;

// This is a powerful, reusable Object Pooler system.
public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPooler Instance { get; private set; }

    [Header("Pool Definitions")]
    public List<Pool> pools;

    private Dictionary<string, Queue<GameObject>> _poolDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    private void Start()
    {
        _poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectQueue.Enqueue(obj);
            }
            _poolDictionary.Add(pool.tag, objectQueue);
        }
    }

    public GameObject GetFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(tag)) return null;

        // Dequeue an object, activate it, and set its properties.
        GameObject objectToSpawn = _poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Put the object back at the end of the queue to be reused later.
        _poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    /// <summary>
    /// Deactivates an object and effectively returns it to the pool for reuse.
    /// </summary>
    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!_poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
            Destroy(objectToReturn); // Fallback to destroying if the pool doesn't exist.
            return;
        }

        // Just deactivate the object. Since we are using a Queue and re-enqueueing on get,
        // we don't need to do anything else here. The object is already in the queue, ready to be reused.
        objectToReturn.SetActive(false);
    }
}