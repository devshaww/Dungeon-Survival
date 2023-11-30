using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    [SerializeField]
    private Pool[] poolArray = null;
    private Transform objectPoolTransform;
    private Dictionary<int, Queue<Component>> poolDictionary = new Dictionary<int, Queue<Component>>();

    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
        public string componentType;
    }

	private void Start()
	{
        objectPoolTransform = gameObject.transform;

        for (int i = 0; i < poolArray.Length; i++)
        {
            CreatePool(poolArray[i].prefab, poolArray[i].poolSize, poolArray[i].componentType);
        }
	}

    private void CreatePool(GameObject prefab, int poolSize, string componentType)
    {
        int poolKey = prefab.GetInstanceID();

        string prefabName = prefab.name;

        GameObject parent = new GameObject(prefabName + "Anchor");

        parent.transform.SetParent(objectPoolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<Component>());
			for (int i = 0; i < poolSize; i++)
			{
                GameObject obj = Instantiate(prefab, parent.transform);
                obj.SetActive(false);
                poolDictionary[poolKey].Enqueue(obj.GetComponent(System.Type.GetType(componentType)));
			}
		}

    }

    public Component ReuseComponent(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            Component componentToReuse = GetComponentFromPool(poolKey);
            ResetObject(position, rotation, componentToReuse, prefab);

            return componentToReuse;
        }
        else
        {
            Debug.Log("No Object pool for " + prefab);
            return null;
        }
    }

    private Component GetComponentFromPool(int poolKey)
    {
        Component component = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(component);

        if (component.gameObject.activeSelf)
        {
            component.gameObject.SetActive(false);
        }

        return component;
	}

    private void ResetObject(Vector3 position, Quaternion rotation, Component componentToReuse, GameObject prefab)
    {
        componentToReuse.transform.position = position;
		componentToReuse.transform.rotation = rotation;
        componentToReuse.gameObject.transform.localScale = prefab.transform.localScale;
	}
}
