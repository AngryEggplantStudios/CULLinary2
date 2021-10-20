using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionLoot : Loot
{
    [SerializeField] private int consumablesIndex = 0;

    protected override void OnPickup(PlayerPickup playerPickup)
    {
        PlayerManager.instance.consumables[consumablesIndex]++;
        playerPickup.PickUpPotion(consumablesIndex);

        LootManager.instance.removeLoot(this.gameObject);
        Destroy(gameObject);
    }
}
