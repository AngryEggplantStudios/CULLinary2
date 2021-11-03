using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnAttack : MonoBehaviour
{
    // Start is called before the first frame update
    //first 3 spawn points for stage 1
    [SerializeField] private BossSpawn[] spawnPoint1;
    //final 3 spawns with tomato cheese and meat.
    [SerializeField] private BossSpawn[] spawnPoint2;


    private int totalSpawnPoints = 3;
    private int currSpawnPoint = 0;
    private bool stage2On = false;
    private bool stage3On = false;

    public void spawnMobs()
    {
        if (stage2On)
        {
            for (int i = 0; i < totalSpawnPoints; i++)
            {
                spawnPoint1[i].activateSpawn();
            }
        } 
        else if (stage3On)
		{
            for (int i = 0; i < totalSpawnPoints; i++)
            {
                spawnPoint2[i].activateSpawn();
            }
        }
        else
        {
            spawnPoint2[currSpawnPoint].activateSpawn();
            currSpawnPoint = (currSpawnPoint + 1) % 3;
        }

    }

    public void activateStage2()
    {
        Debug.Log("Activate Stage 2");
        stage2On = true;
    }

    public void activateStage3()
    {
        Debug.Log("Activate Stage 2");
        stage2On = false;
        stage3On = true;
    }

    public void destroySpawnPoints()
    {
        for (int i = 0; i < totalSpawnPoints; i++)
        {
            spawnPoint1[i].destroyAllSpawns();
        }

        for (int i = 0; i < totalSpawnPoints; i++)
        {
            spawnPoint2[i].destroyAllSpawns();
        }
    }
}
