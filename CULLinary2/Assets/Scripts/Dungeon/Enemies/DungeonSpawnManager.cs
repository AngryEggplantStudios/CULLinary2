using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSpawnManager : SingletonGeneric<DungeonSpawnManager>
{
    private static List<GameObject> monsterSpawners;
    public delegate void SpawnEnemiesDelegate();
    public static event SpawnEnemiesDelegate OnSpawnEnemies;

    public IEnumerator GetSpawners()
    {
        // Get spawners and update spawning caps and spawning numbers
        GameObject[] spawnObjectsArray = GameObject.FindGameObjectsWithTag("MonsterSpawn");
        monsterSpawners = new List<GameObject>(spawnObjectsArray);

        GameTimer.OnStartNewDay += UpdateSpawnersAndTriggerSpawning;
        yield return null;
    }

    public static bool IsOverpopulated(MonsterName name)
    {
        Population pop = EcosystemManager.GetPopulation(name);
        return pop.IsOverpopulated();
    }

    public static void UpdateSpawnersAndTriggerSpawning()
    {
        // updates every day
        UpdateLocalSpawnCaps();
        UpdateSpawnAmount();
        // make sure spawning happens only after spawners are updated
        OnSpawnEnemies?.Invoke();
    }

    private static void UpdateLocalSpawnCaps()
    {
        foreach (MonsterName name in MonsterName.GetValues(typeof(MonsterName)))
        {
            Population pop = EcosystemManager.GetPopulation(name);
            if (!pop.IsEnabled()) // Don't have to update population if is disabled
            {
                // Debug.Log("update local spawn caps: " + name + " is disabled");
                continue;
            }

            List<GameObject> spawners = GetSpawnersByName(name);
            if (spawners.Count == 0)
            {
                continue;
            }
            int spawningAmtForSpawners = pop.GetCurrentNumber() - pop.GetBaseSpawningNumber();
            int localSpawnCap = Mathf.FloorToInt(spawningAmtForSpawners / spawners.Count);
            if (localSpawnCap < 1 && pop.GetLevel() != PopulationLevel.Extinct)
            {
                localSpawnCap = 1;
            }

            for (int i = 0; i < spawners.Count; i++)
            {
                GameObject spawner = spawners[i];
                if (localSpawnCap > spawningAmtForSpawners || i == spawners.Count - 1)
                {
                    // population number not enough for localSpawnCap or is the last spawner
                    localSpawnCap = spawningAmtForSpawners;
                }

                MonsterSpawn monsterSpawn = spawner.GetComponent<MonsterSpawn>();
                monsterSpawn.SetSpawnCap(localSpawnCap);

                spawningAmtForSpawners -= localSpawnCap;
                // Debug.Log(string.Format("spawn cap for {0} set to {1}", name, localSpawnCap));
            }

            Debug.Log("update local spawn caps for " + name + " to " + localSpawnCap);
        }
    }

    private static void UpdateSpawnAmount()
    {
        foreach (MonsterName name in MonsterName.GetValues(typeof(MonsterName)))
        {
            Population pop = EcosystemManager.GetPopulation(name);
            if (!pop.IsEnabled()) // Don't have to update population if is disabled
            {
                continue;
            }


            List<GameObject> spawners = GetSpawnersByName(name);
            foreach (GameObject spawner in spawners)
            {
                // set spawning numbers range
                MonsterSpawn monsterSpawn = spawner.GetComponent<MonsterSpawn>();
                int[] spawnAmtRange = GetSpawnAmountRange(name);
                int minEnemies = spawnAmtRange[0];
                int maxEnemies = spawnAmtRange[1];
                monsterSpawn.SetMinSpawn(minEnemies);
                monsterSpawn.SetMaxSpawn(maxEnemies);
                Debug.Log("update spawn amount for " + name + " to " + minEnemies + " - " + maxEnemies);
            }
        }
    }

    public GameObject GetSpawnerNearestTo(Vector3 givenPosition, MonsterName name)
    {
        // get spawner for a type of monster that is nearest to givenPosition, using squared distance
        GameObject nearestSpawner = null;
        float nearestDistSqr = Mathf.Infinity;
        List<GameObject> spawners = GetSpawnersByName(name);

        foreach (GameObject spawner in spawners)
        {
            Vector3 directionToSpawner = spawner.transform.position - givenPosition;
            float sqrDist = directionToSpawner.sqrMagnitude;
            if (sqrDist < nearestDistSqr)
            {
                nearestDistSqr = sqrDist;
                nearestSpawner = spawner;
            }
        }

        return nearestSpawner;
    }

    private static int[] GetSpawnAmountRange(MonsterName name)
    {
        // returns [min, max]
        Population pop = EcosystemManager.GetPopulation(name);
        PopulationLevel popLevel = pop.GetLevel();
        int minSpawnAmount = 0;
        int maxSpawnAmount = 0;
        switch (popLevel)
        {
            case PopulationLevel.Overpopulated:
                minSpawnAmount = 5;
                maxSpawnAmount = 8;
                break;
            case PopulationLevel.Normal:
                minSpawnAmount = 3;
                maxSpawnAmount = 4;
                break;
            case PopulationLevel.Vulnerable:
                minSpawnAmount = 2;
                maxSpawnAmount = 3;
                break;
            case PopulationLevel.Endangered:
                minSpawnAmount = 1;
                maxSpawnAmount = 2;
                break;
            case PopulationLevel.Extinct:
                minSpawnAmount = 0;
                maxSpawnAmount = 0;
                break;
        }

        return new int[] { minSpawnAmount, maxSpawnAmount };
    }

    public static List<GameObject> GetSpawnersByName(MonsterName name)
    {
        List<GameObject> spawners = new List<GameObject>();
        if (monsterSpawners != null)
        {
            foreach (GameObject monsterSpawn in monsterSpawners)
            {
                if (GetMonsterName(monsterSpawn) == name)
                {
                    spawners.Add(monsterSpawn);
                }
            }
        }
        return spawners;
    }

    public static int GetNumSpawners(MonsterName name)
    {
        return GetSpawnersByName(name).Count;
    }

    private static MonsterName GetMonsterName(GameObject monsterSpawnGO)
    {
        MonsterSpawn monsterSpawn = monsterSpawnGO.GetComponent<MonsterSpawn>();
        return monsterSpawn.GetMonsterName();
    }

    void OnDestroy()
    {
        GameTimer.OnStartNewDay -= UpdateSpawnersAndTriggerSpawning;
    }

}
