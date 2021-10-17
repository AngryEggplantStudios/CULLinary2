using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBaseSpawning : MonoBehaviour
{
    [SerializeField] private BiomeObjectSpawner biomeObjectSpawner;
    [SerializeField] private List<GameObject> monstersToSpawn;
    private int mapSize;

    // Start is called before the first frame update
    void Start()
    {
        mapSize = biomeObjectSpawner.GetSize();
        GameTimer.OnStartNewDay += SpawnBaseMonsters;
    }

    private void SpawnBaseMonsters()
    {
        GameObject parent = new GameObject("BaseMonsters");
        parent.transform.position = Vector3.zero;

        foreach (GameObject monster in monstersToSpawn)
        {
            MonsterScript monsterScript = monster.GetComponent<MonsterScript>();
            if (!monsterScript)
            {
                return;
            }
            MonsterName name = monsterScript.GetMonsterName();
            int baseSpawningNumber = EcosystemManager.GetPopulation(name).GetBaseSpawningNumber();
            int numSpawned = 0;
            while (numSpawned < baseSpawningNumber)
            {
                Vector3 spawnPosition = Vector3.zero;
                while (Vector3.Equals(spawnPosition, Vector3.zero))
                {
                    FindRandomPosition(out spawnPosition);
                }

                Instantiate(monster, spawnPosition, Quaternion.identity, parent.transform);
                numSpawned++;

                int batchSpawning = monsterScript.GetAdditionalSpawning();
                // Spawn monsters in batches if applicable
                if (batchSpawning != 0)
                {
                    for (int i = 0; i < batchSpawning; i++)
                    {
                        if (numSpawned < baseSpawningNumber)
                        {
                            Instantiate(monster, spawnPosition + Vector3.right * (i + 1), Quaternion.identity, parent.transform);
                            numSpawned++;
                        }
                    }
                }
            }
        }
    }

    private bool FindRandomPosition(out Vector3 result)
    {
        Vector3 randomPos = new Vector3(Random.Range(-mapSize / 3f, mapSize / 3f), 0, Random.Range(-mapSize / 3f, mapSize / 3f));
        NavMeshHit hit;
        float maxDist = 2f;
        if (NavMesh.SamplePosition(randomPos, out hit, maxDist, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        else
        {
            result = Vector3.zero;
            return false;
        }
    }

    public void OnDestroy()
    {
        GameTimer.OnStartNewDay -= SpawnBaseMonsters;
    }
}
