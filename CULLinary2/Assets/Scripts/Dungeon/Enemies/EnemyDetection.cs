using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : SingletonGeneric<EnemyDetection>
{
    [SerializeField] float globalRadius;
    private List<MonsterScript> listOfEnemies;
    [SerializeField] private Transform playerTransform;

    void Start()
    {
        listOfEnemies = new List<MonsterScript>();
    }

    void Update()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        foreach (MonsterScript monster in listOfEnemies)
        {
            if (monster.IsFarEnoughFromPlayer(playerTransform.position, globalRadius))
            {
                monster.SetActiveMonster(false);
            }
            else
            {
                monster.SetActiveMonster(true);
            }
        }
    }

    public void AddToListOfEnemies(MonsterScript monster)
    {
        listOfEnemies.Add(monster);
    }

    public void RemoveFromListOfEnemies(MonsterScript monster)
    {
        listOfEnemies.Remove(monster);
    }
}
