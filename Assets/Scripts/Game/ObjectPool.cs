using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    public List<GameObject> objects;
    public GameObject prefab;

    public ObjectPool(int size)
    {
        objects = new List<GameObject>(size);

    }

    public void PreInstantiate(int number)
    {
        for(int i=0; i<number; ++i)
        {
            GameObject obj = UnityEngine.Object.Instantiate(prefab);
            if (obj)
            {
                objects.Add(obj);
                obj.SetActive(false);
            }
        }
    }

    public GameObject Pop()
    {
        for(int i=0; i<objects.Count; ++i)
        {
            if (!objects[i].activeInHierarchy)
                return objects[i];
        }
        // if we get here an object was not found
            GameObject obj = UnityEngine.Object.Instantiate(prefab);
            objects.Add(obj);
            obj.SetActive(true);
        Debug.LogError("Object pool ran out of objects!");
            return obj;

    }

    public void Push(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void DestroyObjects()
    {
        foreach (GameObject obj in objects)
            Object.Destroy(obj);
        objects.Clear();
    }
}
