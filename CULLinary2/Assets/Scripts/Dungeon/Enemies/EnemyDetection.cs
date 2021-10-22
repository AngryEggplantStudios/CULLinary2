using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : SingletonGeneric<EnemyDetection>
{
    [SerializeField] float globalRadius;
    private List<MonsterScript> listOfEnemies;
    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        listOfEnemies = new List<MonsterScript>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (MonsterScript monster in listOfEnemies)
        {
            if (monster.IsFarEnoughFromPlayer(playerTransform.position, globalRadius))
			{
                monster.SetActiveMonster(false);
			} else
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
