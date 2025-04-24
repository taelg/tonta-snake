using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericComponentPoolBehavior<T> : MonoBehaviour where T : Component
{
    [Header("Internal")]
    [SerializeField] private int initialSize = 30;
    [SerializeField] private T prefab;

    private readonly List<T> allObjects = new List<T>();
    private readonly List<T> availableObjects = new List<T>();

    private void Awake()
    {
        LoadPoolObjects();
    }

    private void LoadPoolObjects()
    {
        for (int i = 0; i < initialSize; i++)
            AddObjectToPool();
    }

    public T GetNext()
    {
        bool hasObjectAvailable = availableObjects.Count > 0;
        T pooledObject = hasObjectAvailable ? availableObjects[0] : AddObjectToPool();
        availableObjects.Remove(pooledObject);
        pooledObject.gameObject.SetActive(true);
        return pooledObject;
    }

    public List<T> GetAllObjects()
    {
        return allObjects;
    }

    private T AddObjectToPool()
    {
        T pooledObject = Instantiate(prefab, transform);
        pooledObject.gameObject.SetActive(false);
        GetOrAddObjectReturner(pooledObject);
        availableObjects.Add(pooledObject);
        allObjects.Add(pooledObject);
        return pooledObject;
    }

    protected ReturnToPoolBehavior GetOrAddObjectReturner(T pooledObject)
    {
        var returner = pooledObject.GetComponent<ReturnToPoolBehavior>() ?? pooledObject.gameObject.AddComponent<ReturnToPoolBehavior>();
        returner.returnCallback = (component) => ReturnObjectToPool(pooledObject);
        return returner;
    }

    public void ReturnObjectToPool(T obj)
    {
        if (!availableObjects.Contains(obj))
        {
            availableObjects.Add(obj);
            StartCoroutine(DelayedContainerReturn(obj));
        }
    }

    private IEnumerator DelayedContainerReturn(T obj)
    {
        yield return null;
        obj.transform.parent = transform;
    }

}
