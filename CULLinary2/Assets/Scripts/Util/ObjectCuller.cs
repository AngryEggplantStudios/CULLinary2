using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCuller : SingletonGeneric<ObjectCuller>
{
    public float activeRadius = 100;
    public float inactiveRadius = 150;
    public Transform playerTransform;

    List<GameObject> cullableObjects = new List<GameObject>();

    void Start()
    {
        if (activeRadius > inactiveRadius)
        {
            Debug.LogError("activeRadius should be smaller than inactiveRadius!");
        }
    }

    public void Add(GameObject obj)
    {
        cullableObjects.Add(obj);
    }

    public void Remove(GameObject obj)
    {
        cullableObjects.Remove(obj);
    }

    void Update()
    {
        if (playerTransform == null) return;

        foreach (GameObject obj in cullableObjects)
        {
            if (obj.activeInHierarchy)
            {
                if (IsFar(obj.transform.position))
                {
                    obj.SetActive(false);
                }
            }
            else
            {
                if (IsNear(obj.transform.position))
                {
                    obj.SetActive(true);
                }
            }
        }
    }

    bool IsNear(Vector3 target)
    {
        Vector3 delta = playerTransform.position - target;
        float sqredMagnitude = Vector3.SqrMagnitude(delta);
        return sqredMagnitude < activeRadius * activeRadius;
    }

    bool IsFar(Vector3 target)
    {
        Vector3 delta = playerTransform.position - target;
        float sqredMagnitude = Vector3.SqrMagnitude(delta);
        return sqredMagnitude > inactiveRadius * inactiveRadius;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerTransform.position, inactiveRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerTransform.position, activeRadius);
    }
}
