using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPickup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject itemPickupNotificationPrefab;
    [SerializeField] private GameObject canvasDisplay;

    public void PickUp(InventoryItem itemLoot)
    {
        audioSource.Play();
        GameObject itemPickupNotificationObject = Instantiate(itemPickupNotificationPrefab);
        itemPickupNotificationObject.transform.SetParent(canvasDisplay.transform);
        itemPickupNotificationObject.transform.GetComponentInChildren<Image>().sprite = itemLoot.icon;
    }
}
