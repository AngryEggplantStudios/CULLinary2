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

    public static int GetLocalSpawnCap(EnemyName name)
    {
        Population pop = EcosystemManager.GetPopulation(name);
        int localSpawnCap = Mathf.RoundToInt(pop.GetCurrentNumber() / GetNumSpawners(name));
        if (localSpawnCap < 1 && pop.GetLevel() != PopulationLevel.Extinct)
        {
            localSpawnCap = 1;
        }
        return localSpawnCap;
    }

    public static bool IsOverpopulated(EnemyName name)
    {
        Population pop = EcosystemManager.GetPopulation(name);
        return pop.IsOverpopulated();
    }

    public static void UpdateLocalSpawnCap()
    {
        // set new local spawn cap for each dungeon spawner when population level naturally increases
        foreach (GameObject spawner in dungeonSpawns)
        {
            DungeonSpawn dungeonSpawnScript = spawner.GetComponent<DungeonSpawn>();
            EnemyName enemyName = dungeonSpawnScript.GetEnemyName();
            int newLocalSpawnCap = GetLocalSpawnCap(enemyName);
            dungeonSpawnScript.SetSpawnCap(newLocalSpawnCap);
        }
    }

    public static List<GameObject> GetSpawners(EnemyName name)
    {
        List<GameObject> spawners = new List<GameObject>();
        foreach (GameObject dungeonSpawn in dungeonSpawns)
        {
            if (GetEnemyName(dungeonSpawn) == name)
            {
                spawners.Add(dungeonSpawn);
            }
        }
        return spawners;
    }

    public static int GetNumSpawners(EnemyName name)
    {
        return GetSpawners(name).Count;
    }

    private static EnemyName GetEnemyName(GameObject dungeonSpawnGO)
    {
        DungeonSpawn dungeonSpawnScript = dungeonSpawnGO.GetComponent<DungeonSpawn>();
        return dungeonSpawnScript.GetEnemyName();
    }
}
