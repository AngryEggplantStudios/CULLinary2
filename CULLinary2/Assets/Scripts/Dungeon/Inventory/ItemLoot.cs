using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLoot : Loot
{
    [SerializeField] private InventoryItem itemLoot;

    protected override bool OnPickup(PlayerPickup playerPickup)
    {
        if (InventoryManager.instance != null)
        {
            if (InventoryManager.instance.AddItem(itemLoot))
            {
                playerPickup.PickUp(itemLoot);

                LootManager.instance.removeLoot(this.gameObject);
                Destroy(gameObject);
                return true;
            }
            else
            {
                InventoryManager.instance.DisplayWarning("Inventory full!");
                return false;
            }
        }
        else if (TutorialInventoryManager.instance != null)
        {
            if (TutorialInventoryManager.instance.AddItem(itemLoot))
            {
                playerPickup.PickUp(itemLoot);

                LootManager.instance.removeLoot(this.gameObject);
                Destroy(gameObject);
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}