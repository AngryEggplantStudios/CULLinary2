using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSpawnManager : MonoBehaviour
{
    private static List<GameObject> dungeonSpawns;

    void Awake()
    {
        GameObject[] spawnObjectsArray = GameObject.FindGameObjectsWithTag("EnemySpawn");
        dungeonSpawns = new List<GameObject>(spawnObjectsArray);
    }

    void Start()
    {
        UpdateSpawners();
    }

    void Update()
    {
        CheckForExtinctPopulations();
    }

    private void CheckForExtinctPopulations()
    {
        // check if any population is extinct and update accordingly
        foreach (MonsterName name in MonsterName.GetValues(typeof(MonsterName)))
        {
            Population pop = EcosystemManager.GetPopulation(name);
            if (pop.GetLevel() == PopulationLevel.Extinct)
            {
                continue;
            }

            List<GameObject> spawners = GetSpawners(name);
            foreach (GameObject spawner in spawners)
            {
                DungeonSpawn dungeonSpawnScript = spawner.GetComponent<DungeonSpawn>();
                if (dungeonSpawnScript.GetSpawnCap() > 0)
                {
                    break;
                }
                // all spawn caps for this enemy is 0
                EcosystemManager.SetExtinct(name);
            }
        }
    }

    public static bool IsOverpopulated(MonsterName name)
    {
        Population pop = EcosystemManager.GetPopulation(name);
        return pop.IsOverpopulated();
    }

    public static void UpdateSpawners()
    {
        // updates every day
        UpdateLocalSpawnCaps();
        UpdateSpawnAmount();
    }

    private static void UpdateLocalSpawnCaps()
    {
        foreach (MonsterName name in MonsterName.GetValues(typeof(MonsterName)))
        {
            Population pop = EcosystemManager.GetPopulation(name);
            List<GameObject> spawners = GetSpawners(name);
            if (spawners.Count == 0)
            {
                continue;
            }
            int popNumber = pop.GetCurrentNumber();
            int localSpawnCap = Mathf.FloorToInt(popNumber / spawners.Count);
            if (localSpawnCap < 1 && pop.GetLevel() != PopulationLevel.Extinct)
            {
                localSpawnCap = 1;
            }

            for (int i = 0; i < spawners.Count; i++)
            {
                GameObject spawner = spawners[i];
                if (localSpawnCap > popNumber || i == spawners.Count - 1)
                {
                    // population number not enough for localSpawnCap or is the last spawner
                    localSpawnCap = popNumber;
                }

                DungeonSpawn dungeonSpawnScript = spawner.GetComponent<DungeonSpawn>();
                dungeonSpawnScript.SetSpawnCap(localSpawnCap);

                popNumber -= localSpawnCap;
                // Debug.Log(string.Format("spawn cap for {0} set to {1}", name, localSpawnCap));
            }
        }
    }

    private static void UpdateSpawnAmount()
    {
        foreach (MonsterName name in MonsterName.GetValues(typeof(MonsterName)))
        {
            Population pop = EcosystemManager.GetPopulation(name);
            List<GameObject> spawners = GetSpawners(name);
            foreach (GameObject spawner in spawners)
            {
                // set spawning numbers range
                DungeonSpawn dungeonSpawnScript = spawner.GetComponent<DungeonSpawn>();
                int[] spawnAmtRange = GetSpawnAmountRange(name);
                int minEnemies = spawnAmtRange[0];
                int maxEnemies = spawnAmtRange[1];
                dungeonSpawnScript.SetMinSpawn(minEnemies);
                dungeonSpawnScript.SetMaxSpawn(maxEnemies);
            }
        }
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

    public static List<GameObject> GetSpawners(MonsterName name)
    {
        List<GameObject> spawners = new List<GameObject>();
        foreach (GameObject dungeonSpawn in dungeonSpawns)
        {
            if (GetMonsterName(dungeonSpawn) == name)
            {
                spawners.Add(dungeonSpawn);
            }
        }
        return spawners;
    }

    public static int GetNumSpawners(MonsterName name)
    {
        return GetSpawners(name).Count;
    }

    private static MonsterName GetMonsterName(GameObject dungeonSpawnGO)
    {
        DungeonSpawn dungeonSpawnScript = dungeonSpawnGO.GetComponent<DungeonSpawn>();
        return dungeonSpawnScript.GetMonsterName();
    }
}
