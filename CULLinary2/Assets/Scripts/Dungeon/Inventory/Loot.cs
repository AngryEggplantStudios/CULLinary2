using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loot : MonoBehaviour
{
    [SerializeField] private Item itemLoot;
    [SerializeField] private GameObject itemPickupNotif_prefab;

    private void OnTriggerEnter(Collider other)
    {
        PlayerPickup playerPickup = other.GetComponent<PlayerPickup>();
        if (playerPickup != null)
        {
            if (InventoryManager.instance.AddItem(itemLoot))
            {
                playerPickup.PickUp(itemLoot);
                Destroy(gameObject);
            }
        }
    }
}
