using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawn Event", menuName = "Event/Spawn Event")]
public class SpawnEvent : SpecialEvent
{
    public GameObject monsterToSpawn;

    public void SpawnMonster()
    {
        Instantiate(monsterToSpawn);
    }
}
