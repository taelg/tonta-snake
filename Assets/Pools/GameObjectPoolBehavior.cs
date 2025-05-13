using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolBehavior : MonoBehaviour
{
    [Header("Internal")]
    [SerializeField] private int initialSize = 30;
    [SerializeField] private GameObject prefab;

    private readonly List<GameObject> allObjects = new List<GameObject>();
    private readonly List<GameObject> availableObjects = new List<GameObject>();

    private void Awake()
    {
        LoadPoolObjects();
    }

    private void LoadPoolObjects()
    {
        for (int i = 0; i < initialSize; i++)
            AddObjectToPool();
    }

    public GameObject GetPooledObject()
    {
        bool hasObjectAvailable = availableObjects.Count > 0;
        GameObject pooledObject = hasObjectAvailable ? availableObjects[0] : AddObjectToPool();
        availableObjects.Remove(pooledObject);
        pooledObject.SetActive(true);
        return pooledObject;
    }

    private GameObject AddObjectToPool()
    {
        GameObject pooledObject = Instantiate(prefab, transform);
        pooledObject.SetActive(false);
        var returner = GetOrAddObjectReturner(pooledObject);
        returner.pool = this;
        availableObjects.Add(pooledObject);
        allObjects.Add(pooledObject);
        return pooledObject;
    }

    private ReturnToPool GetOrAddObjectReturner(GameObject pooledObject)
    {
        var returner = pooledObject.GetComponent<ReturnToPool>();
        return pooledObject.GetComponent<ReturnToPool>() ?? pooledObject.AddComponent<ReturnToPool>();
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        if (!availableObjects.Contains(obj))
        {
            availableObjects.Add(obj);
            StartCoroutine(DelayedContainerReturn(obj));
        }
    }

    public void ResetAllObjects()
    {
        foreach (var obj in allObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                obj.transform.parent = transform;
            }
        }
    }

    private IEnumerator DelayedContainerReturn(GameObject obj)
    {
        yield return null;
        obj.transform.parent = transform;
    }

    // Aux class to return objects to pool.
    private class ReturnToPool : MonoBehaviour
    {
        [HideInInspector] public GameObjectPoolBehavior pool;

        void OnDisable()
        {
            if (gameObject.scene.isLoaded)
            {
                pool.ReturnObjectToPool(gameObject);
            }
        }
    }

}
