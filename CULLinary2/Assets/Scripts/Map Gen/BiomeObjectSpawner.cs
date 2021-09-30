using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeObjectSpawner : MonoBehaviour
{
    [SerializeField] private int size = 2000;
    [SerializeField] private float minY = 8;
    [SerializeField] private Spawnable[] spawnables;
    private Transform origin;

    private RaycastHit hit;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<GameObject> parents = new List<GameObject>();

    void Awake()
    {
        origin = this.transform.parent.transform;
    }

    public IEnumerator SpawnObjects(int seed)
    {
        DestroyObjects();
        Random.InitState(seed);

        foreach (Spawnable spawnable in spawnables)
        {
            // Debug.Log("Spawning " + spawnable.name + "...");
            GameObject parent = new GameObject(spawnable.name);
            parent.transform.SetParent(origin);
            parent.transform.localScale = Vector3.one;
            parents.Add(parent);

            for (int i = 0; i < spawnable.density; i++)
            {
                // Assign random location
                Vector3 pos = new Vector3(Random.Range(-size / 2f, size / 2f), 100, Random.Range(-size / 2f, size / 2f));
                pos += origin.transform.position;

                // Check validity
                //      On the ground
                //      Above sea level
                //      Non intersecting
                if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground"))
                        && hit.point.y >= minY
                        && !IsIntersecting(hit.point, spawnable.intersectRadius))
                {
                    // Spawn Object
                    GameObject spawnedObject = Instantiate(spawnable.prefabs[Random.Range(0, spawnable.prefabs.Length)], parent.transform);
                    spawnedObject.transform.position = hit.point;
                    spawnedObject.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                    spawnedObjects.Add(spawnedObject);

                    // Only wait if intersections are important
                    if (spawnable.intersectRadius != 0 && !spawnable.removeCollider)
                    {
                        yield return null;
                    }
                }
            }
            SpawnableHelper helper = parent.AddComponent<SpawnableHelper>() as SpawnableHelper;
            helper.removeCollider = spawnable.removeCollider;
            yield return null;
        }
        // Debug.Log(spawnedObjects.Count + " objects spawned");
    }

    public void DestroyObjects()
    {
        // Debug.Log(spawnedObjects.Count + " objects destroyed");

        foreach (GameObject parent in parents)
        {
            DestroyImmediate(parent);
        }
        spawnedObjects = new List<GameObject>();
    }

    bool IsIntersecting(Vector3 position, float obstacleCheckRadius)
    {
        if (obstacleCheckRadius <= 0) { return false; }

        Collider[] colliders = Physics.OverlapSphere(position, obstacleCheckRadius);

        foreach (Collider col in colliders)
        {
            if(col.gameObject.layer != LayerMask.NameToLayer("Ground"))
            {
                return true;
            }
        }

        return false;
    }
}
