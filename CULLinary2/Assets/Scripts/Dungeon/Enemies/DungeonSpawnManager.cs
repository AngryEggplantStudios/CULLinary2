using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSpawnManager : MonoBehaviour
{
    private List<GameObject> dungeonSpawns;

    void Awake()
    {
        GameObject[] spawnObjectsArray = GameObject.FindGameObjectsWithTag("EnemySpawn");
        dungeonSpawns = new List<GameObject>(spawnObjectsArray);
    }

    public int GetNumSpawners(EnemyName name)
    {
        int numSpawners = 0;
        foreach (GameObject dungeonSpawn in dungeonSpawns)
        {
            if (GetEnemyName(dungeonSpawn) == name)
            {
                numSpawners++;
            }
        }

        return numSpawners;
    }

    private EnemyName GetEnemyName(GameObject dungeonSpawnGO)
    {
        DungeonSpawn dungeonSpawnScript = dungeonSpawnGO.GetComponent<DungeonSpawn>();
        return dungeonSpawnScript.GetEnemyName();
    }
}
