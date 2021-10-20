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
            chestScript.loot[2] = lootToAdd;
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
