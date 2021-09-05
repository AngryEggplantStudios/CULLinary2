using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public int seed;
    public int size;
    public float minY;
    public Transform origin;
    public Spawnable[] spawnables;
    
    RaycastHit hit;
    List<GameObject> spawnedObjects = new List<GameObject>();
    List<GameObject> parents = new List<GameObject>();

    public void SpawnObjects()
    {
        DestroyObjects();
        Random.InitState(seed);

        foreach (Spawnable spawnable in spawnables)
        {
            GameObject parent = new GameObject(spawnable.name);
            parent.transform.SetParent(origin);
            parent.transform.localScale = Vector3.one;
            parents.Add(parent);

            for (int i = 0; i < spawnable.density; i++)
            {
                // Assign random location
                Vector3 pos = new Vector3(Random.Range(-size/2f, size/2f), 100, Random.Range(-size/2f, size/2f));
                pos += origin.transform.position;

                // Check validity
                if (Physics.Raycast(pos, Vector3.down, out hit) && hit.point.y >= minY)
                {
                    // Spawn Object
                    GameObject spawnedObject = Instantiate(spawnable.prefabs[Random.Range(0, spawnable.prefabs.Length)], parent.transform);
                    spawnedObject.transform.position = hit.point;
                    spawnedObject.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                    spawnedObjects.Add(spawnedObject);
                }
            }
        }

        //Debug.Log(spawnedObjects.Count + " objects spawned");
    }

    public void DestroyObjects()
    {
        //Debug.Log(spawnedObjects.Count + " objects destroyed");

        foreach (GameObject parent in parents)
        {
            DestroyImmediate(parent);
        }
        spawnedObjects = new List<GameObject>();
    }
}

[System.Serializable]
public struct Spawnable
{
    public string name;
    public GameObject[] prefabs;
    public int density;
}