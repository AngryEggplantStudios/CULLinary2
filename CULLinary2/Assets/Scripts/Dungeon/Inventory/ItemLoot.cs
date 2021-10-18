using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLoot : Loot
{
    [SerializeField] private InventoryItem itemLoot;

    protected override void OnPickup(PlayerPickup playerPickup)
    {
        if (InventoryManager.instance.AddItem(itemLoot))
        {
            playerPickup.PickUp(itemLoot);

            LootManager.instance.removeLoot(this.gameObject);
            Destroy(gameObject);
        }
    }
}