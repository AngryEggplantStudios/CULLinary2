using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSpawnManager : MonoBehaviour
{
    private static List<GameObject> monsterSpawns;

    void Update()
    {
    }

    public IEnumerator GetSpawners()
    {
        // Get spawners and update spawning caps and spawning numbers
        GameObject[] spawnObjectsArray = GameObject.FindGameObjectsWithTag("MonsterSpawn");
        monsterSpawns = new List<GameObject>(spawnObjectsArray);
        UpdateSpawners();
        yield return null;

        // temp
        //List<MonsterData> monsterDatas = DatabaseLoader.GetAllMonsters();
        // Need to loop through monsterdatas to get the names
        List<GameObject> cornSpawners = GetSpawners(MonsterName.Corn);
        List<GameObject> potatoSpawners = GetSpawners(MonsterName.Potato);
        List<GameObject> eggplantSpawners = GetSpawners(MonsterName.Eggplant);
        Debug.Log(string.Format("at loading: {0} corn spawners, {1} potato spawners, {2} eggplant spawners", cornSpawners.Count, potatoSpawners.Count, eggplantSpawners.Count));
    }

    public static void CheckIfExtinct(MonsterName name)
    {
        // Updates population level if found to be extinct and population level is not already Extinct

        Population pop = EcosystemManager.GetPopulation(name);
        if (pop.GetLevel() == PopulationLevel.Extinct)
        {
            return;
        }

        List<GameObject> spawners = GetSpawners(name);
        bool isExtinct = true;

        foreach (GameObject spawner in spawners)
        {
            MonsterSpawn monsterSpawn = spawner.GetComponent<MonsterSpawn>();
            if (monsterSpawn.GetSpawnCap() > 0)
            {
                isExtinct = false;
                break;
            }
        }

        if (isExtinct)
        {
            // All spawn caps for this enemy are 0
            EcosystemManager.SetExtinct(name);
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

                MonsterSpawn monsterSpawn = spawner.GetComponent<MonsterSpawn>();
                monsterSpawn.SetSpawnCap(localSpawnCap);

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
                MonsterSpawn monsterSpawn = spawner.GetComponent<MonsterSpawn>();
                int[] spawnAmtRange = GetSpawnAmountRange(name);
                int minEnemies = spawnAmtRange[0];
                int maxEnemies = spawnAmtRange[1];
                monsterSpawn.SetMinSpawn(minEnemies);
                monsterSpawn.SetMaxSpawn(maxEnemies);
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
                maxSpawnAmount = 6;
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
        if (monsterSpawns != null)
        {
            foreach (GameObject monsterSpawn in monsterSpawns)
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
        return GetSpawners(name).Count;
    }

    private static MonsterName GetMonsterName(GameObject monsterSpawnGO)
    {
        MonsterSpawn monsterSpawn = monsterSpawnGO.GetComponent<MonsterSpawn>();
        return monsterSpawn.GetMonsterName();
    }

}
