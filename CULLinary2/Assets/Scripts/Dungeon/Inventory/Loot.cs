using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loot : MonoBehaviour
{
    [SerializeField] private InventoryItem itemLoot;
    [SerializeField] private GameObject itemPickupNotif_prefab;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 20, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerPickup playerPickup = other.GetComponent<PlayerPickup>();
        if (playerPickup != null)
        {
            if (InventoryManager.instance.AddItem(itemLoot))
            {
                playerPickup.PickUp(itemLoot);
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
    }
}
