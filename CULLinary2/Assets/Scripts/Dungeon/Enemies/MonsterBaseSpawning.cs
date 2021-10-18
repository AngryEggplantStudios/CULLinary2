using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBaseSpawning : SingletonGeneric<MonsterBaseSpawning>
{
    [SerializeField] private BiomeObjectSpawner biomeObjectSpawner;
    [SerializeField] private List<GameObject> monstersToSpawn;
    [SerializeField] private GameObject parent;
    private int mapSize;
    public Dictionary<MonsterName, int> numOfAliveMonsters = new Dictionary<MonsterName, int>{ // num of alive monsters from base spawning only
        {MonsterName.Corn, 0},
        {MonsterName.Potato, 0},
        {MonsterName.Eggplant, 0},
    };

    // Start is called before the first frame update
    void Start()
    {
        mapSize = biomeObjectSpawner.GetSize();
        GameTimer.OnStartNewDay += SpawnBaseMonsters;
    }

    public void UpdateNumOfAliveMonsters(MonsterName name, int value)
    {
        numOfAliveMonsters[name] += value;
        Debug.Log("updated num of alive " + name + " to " + numOfAliveMonsters[name]);
    }

    private void SpawnBaseMonsters()
    {
        foreach (GameObject monster in monstersToSpawn)
        {
            MonsterScript monsterScript = monster.GetComponent<MonsterScript>();
            if (!monsterScript)
            {
                return;
            }

            MonsterName name = monsterScript.GetMonsterName();
            int baseSpawningNumber = EcosystemManager.GetPopulation(name).GetBaseSpawningNumber();
            int numCurrentlyAlive = numOfAliveMonsters[name];

            while (numCurrentlyAlive < baseSpawningNumber)
            {
                Vector3 spawnPosition = Vector3.zero;
                while (Vector3.Equals(spawnPosition, Vector3.zero))
                {
                    FindRandomPosition(out spawnPosition);
                }

                Instantiate(monster, spawnPosition, Quaternion.identity, parent.transform);
                numCurrentlyAlive++;

                int batchSpawning = monsterScript.GetAdditionalSpawning();
                // Spawn monsters in batches if applicable
                if (batchSpawning != 0)
                {
                    for (int i = 0; i < batchSpawning; i++)
                    {
                        if (numCurrentlyAlive < baseSpawningNumber)
                        {
                            Instantiate(monster, spawnPosition + Vector3.right * (i + 1), Quaternion.identity, parent.transform);
                            numCurrentlyAlive++;
                        }
                    }
                }
            }

            numOfAliveMonsters[name] = numCurrentlyAlive;
        }
    }

    private bool FindRandomPosition(out Vector3 result)
    {
        Vector3 randomPos = new Vector3(Random.Range(-mapSize / 4f, mapSize / 4f), 0, Random.Range(-mapSize / 4f, mapSize / 4f));
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
