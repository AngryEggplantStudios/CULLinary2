using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : SingletonGeneric<LootManager>
{
    List<GameObject> listOfLoot;
    // Start is called before the first frame update
    void Start()
    {
        listOfLoot = new List<GameObject>();
        GameTimer.OnStartNewDay += destroyLootOnNewDay;
    }

    private void destroyLootOnNewDay()
    {
        foreach (GameObject loot in listOfLoot)
        {
            Destroy(loot);
        }
        listOfLoot = new List<GameObject>();
    }

    public void addLoot(GameObject loot)
    {
        this.listOfLoot.Add(loot);
    }

    public void removeLoot(GameObject loot)
    {
        this.listOfLoot.Remove(loot);
    }

    public void OnDestroy()
    {
        GameTimer.OnStartNewDay -= destroyLootOnNewDay;
    }
}
