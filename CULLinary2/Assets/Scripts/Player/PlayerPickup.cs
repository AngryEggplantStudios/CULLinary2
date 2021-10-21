using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPickup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject itemPickupNotificationPrefab;
    [SerializeField] private GameObject moneyNotif_prefab;
    [SerializeField] private GameObject canvasDisplay;

    [Header("Icon for Potion Loot")]
    [SerializeField] private Sprite redPillIcon;
    [SerializeField] private Sprite bluePillIcon;
    [SerializeField] private Sprite potionIcon;
    [SerializeField] private Sprite pfizerIcon;
    [SerializeField] private Sprite modernaIcon;

    public void PickUp(InventoryItem itemLoot)
    {
        audioSource.Play();
        GameObject itemPickupNotificationObject = Instantiate(itemPickupNotificationPrefab);
        itemPickupNotificationObject.transform.SetParent(canvasDisplay.transform);
        itemPickupNotificationObject.transform.GetComponentInChildren<Image>().sprite = itemLoot.icon;
    }

    public void PickUpMoney(int amount)
    {
        audioSource.Play();
        GameObject moneyNotif = Instantiate(moneyNotif_prefab);
        moneyNotif.transform.GetComponentInChildren<Text>().text = "+$" + amount.ToString();
        moneyNotif.transform.SetParent(canvasDisplay.transform);
        moneyNotif.transform.localPosition = Vector3.zero;

        // Update money UI
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.StopAllCoroutines();
            InventoryManager.instance.StartCoroutine(InventoryManager.instance.UpdateUI());
        }
    }

    public void PickUpPotion(int consumableIndex)
    {
        audioSource.Play();
        GameObject itemPickupNotificationObject = Instantiate(itemPickupNotificationPrefab);
        itemPickupNotificationObject.transform.SetParent(canvasDisplay.transform);
        switch (consumableIndex)
        {
            case 1:
                itemPickupNotificationObject.transform.GetComponentInChildren<Image>().sprite = redPillIcon;
                break;
            case 2:
                itemPickupNotificationObject.transform.GetComponentInChildren<Image>().sprite = bluePillIcon;
                break;
            case 3:
                itemPickupNotificationObject.transform.GetComponentInChildren<Image>().sprite = potionIcon;
                break;
            case 4:
                itemPickupNotificationObject.transform.GetComponentInChildren<Image>().sprite = pfizerIcon;
                break;
            case 5:
                itemPickupNotificationObject.transform.GetComponentInChildren<Image>().sprite = modernaIcon;
                break;
        }
        // Update potions UI
        UIController.instance.UpdateFixedHUD();
    }
}
