using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeObjectSpawner : MonoBehaviour
{
    [SerializeField] private int size = 2000;
    [SerializeField] private float minY = 8;
    [SerializeField] private Spawnable[] spawnables;
    [SerializeField] private Landmark[] landmarks;
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

        GameObject landmarkParent = new GameObject("Landmarks");
        landmarkParent.transform.SetParent(origin);
        landmarkParent.transform.localScale = Vector3.one;
        parents.Add(landmarkParent);
        foreach (Landmark landmark in landmarks)
        {
            for (int i = 0; i < 100; i++) // 100 tries
            {
                // Assign random location and rotation
                float radius = size / 2f - landmark.size;
                Vector3 pos = new Vector3(Random.Range(-radius, radius), 100, Random.Range(-radius, radius));
                pos += origin.transform.position;
                Vector3 center = pos;
                Vector3 eulerAngles = new Vector3(0, Random.Range(0, 360), 0);

                // Calculate important vectors
                float theta = eulerAngles.y * Mathf.Deg2Rad;
                Vector3 xVector = new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta));
                Vector3 zVector = new Vector3(Mathf.Cos(theta + Mathf.PI / 2), 0, Mathf.Sin(theta + Mathf.PI / 2));

                // Check validity by raycasting every m^2
                //      On the ground
                //      Above sea level
                bool CheckValidity()
                {
                    for (int x = -landmark.size; x <= landmark.size; x++)
                    {
                        for (int z = -landmark.size; z <= landmark.size; z++)
                        {
                            if (!Physics.Raycast(pos + xVector * x + zVector * z, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground"))
                                    || hit.point.y < minY)
                            {
                                return false;
                            }

                            if (x == 0 && z == 0)
                            {
                                center = hit.point;
                            }
                        }
                    }
                    return true;
                }

                if (!CheckValidity())
                {
                    if (i == 99)
                    {
                        Debug.LogWarning("No space for landmark " + landmark.name + " to spawn!");
                    }
                    continue;
                }

                // Spawn Object
                GameObject spawnedObject = Instantiate(landmark.prefab, landmarkParent.transform);
                spawnedObject.transform.position = center;
                spawnedObject.transform.eulerAngles = eulerAngles;
                spawnedObjects.Add(spawnedObject);
                break;
            }

            yield return null;
        }

        foreach (Spawnable spawnable in spawnables)
        {
            if (!spawnable.isSpawnable)
            {
                continue;
            }
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
                    if (!spawnable.ignoreSelfIntersection)
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
            if (col.gameObject.layer != LayerMask.NameToLayer("Ground"))
            {
                return true;
            }
        }

        return false;
    }

    public int GetSize()
    {
        return size;
    }
}
