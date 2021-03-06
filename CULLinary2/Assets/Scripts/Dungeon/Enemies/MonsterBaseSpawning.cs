using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBaseSpawning : SingletonGeneric<MonsterBaseSpawning>
{
    [SerializeField] private BiomeObjectSpawner biomeObjectSpawner;
    [SerializeField] private List<GameObject> monstersToSpawn;
    [SerializeField] private GameObject parent;
    [SerializeField, Tooltip("Distance to spawn monster away from player and campfire")] private int distFromSafeSpace = 30; // Distance to spawn monster away from player and campfire
    [SerializeField, Tooltip("Distance to spawn monster away from other objects")] private int distFromNormalObstacle = 2; // Distance to spawn monster away from landmarks and other objects
    private int mapSize;
    public Dictionary<MonsterName, int> numOfAliveMonsters = new Dictionary<MonsterName, int>();
    private List<GameObject> mushroomList = new List<GameObject>();
    private bool isMushActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        mapSize = biomeObjectSpawner.GetSize();

        // Initialise numOfAliveMonsters
        foreach (GameObject monster in monstersToSpawn)
        {
            if (monster.GetComponent<MonsterScript>())
            {
                MonsterName name = monster.GetComponent<MonsterScript>().GetMonsterName();
                numOfAliveMonsters.Add(name, 0);
            }
        }

        GameTimer.OnStartNewDay += SpawnBaseMonsters;
        GameTimer.OnStartNight += ActivateMushrooms;
    }

    public void UpdateNumOfAliveMonsters(MonsterName name, int value)
    {
        numOfAliveMonsters[name] += value;
        // Debug.Log("updated num of alive " + name + " to " + numOfAliveMonsters[name]);
    }

    private void ActivateMushrooms()
    {
        isMushActivated = true;
        foreach (GameObject mushroomObj in mushroomList)
        {
            mushroomObj.SetActive(true);
        }
    }

    private void DeactivateMushrooms()
	{
        isMushActivated = false;
        foreach(GameObject mushroomObj in mushroomList)
		{
            mushroomObj.SetActive(false);
        }
	}

    private void SpawnBaseMonsters()
    {
        foreach (GameObject monster in monstersToSpawn)
        {
            MonsterScript monsterScript = monster.GetComponent<MonsterScript>();
            if (!monsterScript)
            {
                continue;
            }

            MonsterName name = monsterScript.GetMonsterName();
            if (!EcosystemManager.GetPopulation(name).IsEnabled())
            {
                // Don't spawn base monsters if population is disabled
                // Debug.Log(name + " is disabled, not spawning base monsters");
                continue;
            }

            int baseSpawningNumber = EcosystemManager.GetPopulation(name).GetBaseSpawningNumber();
            int numCurrentlyAlive = numOfAliveMonsters[name];
            bool isMushroom = name == MonsterName.Mushroom;
            while (numCurrentlyAlive < baseSpawningNumber)
            {
                Vector3 spawnPosition = Vector3.zero;
                bool hasFoundPosition = false;
                // for (int i = 0; i < 30; i++)
                // {
                //     hasFoundPosition = FindRandomPosition(out spawnPosition);
                //     if (hasFoundPosition)
                //     {
                //         break;
                //     }
                // }

                // if (!hasFoundPosition)
                // {
                //     Debug.Log("couldn't find position to spawn");
                //     continue;
                // }
                while (!hasFoundPosition)
                {
                    hasFoundPosition = FindRandomPosition(out spawnPosition);
                }

                GameObject returnedObject = Instantiate(monster, spawnPosition, Quaternion.identity, parent.transform);
                if (isMushroom)
				{
                    //since this is only called on new day, deactivate all newly spawned mushroom
                    mushroomList.Add(returnedObject);
				}
                numCurrentlyAlive++;

                // Spawn monsters in batches if applicable
                int batchSpawning = monsterScript.GetAdditionalSpawning();
                if (batchSpawning != 0)
                {
                    for (int i = 0; i < batchSpawning; i++)
                    {
                        if (numCurrentlyAlive < baseSpawningNumber)
                        {
                            returnedObject = Instantiate(monster, spawnPosition + Vector3.right * (i + 1), Quaternion.identity, parent.transform);
                            if (isMushroom)
                            {
                                mushroomList.Add(returnedObject);
                            }
                            numCurrentlyAlive++;
                        }
                    }
                }
            }

            int numSpawned = numCurrentlyAlive - numOfAliveMonsters[name];
            if (numSpawned > 0) { Debug.Log("base spawning spawned " + numSpawned + " " + name); }

            numOfAliveMonsters[name] = numCurrentlyAlive;
        }
        DeactivateMushrooms();
    }

    private bool FindRandomPosition(out Vector3 result)
    {
        Vector3 randomPos = new Vector3(Random.Range(-mapSize, mapSize), 0, Random.Range(-mapSize, mapSize));
        NavMeshHit hit;
        float searchRadius = 2f;
        if (NavMesh.SamplePosition(randomPos, out hit, searchRadius, NavMesh.AllAreas))
        {
            // result = hit.position;
            // return true;
            if (IsValidSpawnPosition(hit.position))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    private bool IsValidSpawnPosition(Vector3 position)
    {
        // Check distance from player and other objects
        // Collider[] playerCollider = Physics.OverlapSphere(position, distFromSafeSpace, LayerMask.NameToLayer("Player"));
        // if (playerCollider.Length != 0)
        // {
        //     // Too close to player
        //     return false;
        // }

        // Check for collision with objects in the map
        Collider[] colliders = Physics.OverlapSphere(position, distFromNormalObstacle);
        foreach (Collider col in colliders)
        {
            GameObject go = col.gameObject;
            if (go.layer != LayerMask.NameToLayer("Ground"))
            {
                return false;
            }

            // Check distance from campfire
            if (go.tag.Equals("Campfire"))
            {
                Vector3 direction = go.transform.position - position;
                float sqrDist = direction.sqrMagnitude;
                if (sqrDist < Mathf.Pow(distFromSafeSpace, 2))
                {
                    // Too close to a campfire
                    return false;
                }
            }
        }

        return true;
    }

    public void OnDestroy()
    {
        GameTimer.OnStartNewDay -= SpawnBaseMonsters;
        GameTimer.OnStartNight -= ActivateMushrooms;
    }
}
