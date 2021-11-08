using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawn Event", menuName = "Event/Spawn Event")]
public class SpawnEvent : SpecialEvent
{
    public GameObject monsterToSpawn;
    public MonsterName monsterToUnlockInDex;

    public void SpawnMonster()
    {
        if (!PlayerManager.instance.unlockedMonsters.Contains(monsterToUnlockInDex))
        {
            PlayerManager.instance.unlockedMonsters.Add(monsterToUnlockInDex);
            CreatureDexManager.instance.UpdateCreatureSlots();
        }
        Instantiate(monsterToSpawn);
    }
}
