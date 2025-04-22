using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePoolBehavior : MonoBehaviour
{
    [Header("Internal")]
    [SerializeField] private int initialSize = 30;
    [SerializeField] private GameObject prefab;

    private readonly List<GameObject> availableObjects = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            AddObjectToPool();
        }
    }

    public GameObject GetNext()
    {
        GameObject pooledObject;

        if (availableObjects.Count > 0)
        {
            pooledObject = availableObjects[0];
            availableObjects.RemoveAt(0);
        }
        else
        {
            pooledObject = AddObjectToPool();
        }

        pooledObject.SetActive(true);
        return pooledObject;
    }

    private GameObject AddObjectToPool()
    {
        GameObject pooledObject = Instantiate(prefab, transform);
        pooledObject.SetActive(false);

        var returner = pooledObject.GetComponent<ReturnToPool>();
        if (returner == null)
        {
            returner = pooledObject.AddComponent<ReturnToPool>();
        }
        returner.pool = this;

        availableObjects.Add(pooledObject);
        return pooledObject;
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        if (!availableObjects.Contains(obj))
        {
            availableObjects.Add(obj);
            StartCoroutine(DelayedContainerReturn(obj));
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
        [HideInInspector] public SimplePoolBehavior pool;

        void OnDisable()
        {
            if (gameObject.scene.isLoaded)
            {
                pool.ReturnObjectToPool(gameObject);
            }
        }
    }

}
