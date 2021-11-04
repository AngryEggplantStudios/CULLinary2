using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : SingletonGeneric<EnemyDetection>
{
    [SerializeField] float globalRadius;
    private List<MonsterScript> listOfEnemies;
    [SerializeField] private Transform playerTransform;
    private bool isMushActive = false;
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
        isMushActive = GameTimer.Instance.GetIsMushroomActive();
        Debug.Log("ISMUSH ACTIVE" + isMushActive);
        foreach (MonsterScript monster in listOfEnemies)
        {
            if (monster.GetMonsterName() != MonsterName.Mushroom || isMushActive)
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
