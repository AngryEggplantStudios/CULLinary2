using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private float distanceTriggered;
    [SerializeField] private float stopChase;
    [SerializeField] private float damageMultiplier = 1.5f;
    [Header("MiniBoss Loot")]
    [SerializeField] private int[] randomLootRange = new int[2] { 3, 5 };
    [SerializeField] private GameObject lootChest;
    private MonsterName monsterName;

    void Start()
    {
        MonsterScript monsterScript = gameObject.GetComponent<MonsterScript>();
        if (monsterScript)
        {
            // Set monster variables for miniboss
            monsterName = monsterScript.GetMonsterName();
            monsterScript.SetMiniBossValues(health, distanceTriggered, stopChase, damageMultiplier);

            // Set up loot
            ChestLoot lootToAdd;
            lootToAdd.prefab = monsterScript.GetLoot();
            lootToAdd.quantity = Random.Range(randomLootRange[0], randomLootRange[1] + 1);
            Chest chestScript = lootChest.GetComponent<Chest>();
            // int lootIndex = chestScript.FindLootIndex("PotatoLoot");
            // if (lootIndex >= 0)
            // {
            chestScript.loot[2] = lootToAdd;
            foreach (ChestLoot l in chestScript.loot)
            {
                Debug.Log("loot: " + l.prefab.name + " x" + l.quantity);
            }
            // }
            // else
            // {
            //     Debug.Log("cannot find PotatoLoot in MiniBossLootChest");
            // }
            // foreach (ChestLoot l in lootArr)
            // {
            //     if (l.prefab.name == lootToAdd.prefab.name)
            //     {
            //         Debug.Log("loot chest already has this loot: " + lootToAdd.prefab.name);
            //         return;
            //     }
            // }
            // lootChest.GetComponent<Chest>().AddLoot(lootToAdd);
            // Debug.Log("added loot: " + lootToAdd.prefab.name);
        }
    }

    public void Die()
    {
        EcosystemManager.OnMiniBossDeath(monsterName);
    }

    public void DropLoot()
    {
        GameObject chestGO = Instantiate(lootChest, transform.position, Quaternion.identity);
        chestGO.transform.localScale *= 5;
        Chest chestScript = chestGO.GetComponent<Chest>();
        chestScript.spCollider.SetPlayerInteractable(chestScript);
    }
}
