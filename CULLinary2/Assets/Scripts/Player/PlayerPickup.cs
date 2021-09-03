using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject itemPickupNotificationPrefab;
    [SerializeField] private GameObject canvasDisplay;
    [SerializeField] private GameObject inventoryUI;
    private KeyCode openInventoryKeyCode;

    private void Awake()
    {
        openInventoryKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenInventory);
    }
    public void ToggleInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }
    
    public void OpenInventory()
    {
        inventoryUI.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(openInventoryKeyCode))
        {
            ToggleInventory();
        }
    }

    public void PickUp(Item itemLoot)
    {
        audioSource.Play();
        GameObject itemPickupNotificationObject = Instantiate(itemPickupNotificationPrefab, transform.position, Quaternion.identity);
        itemPickupNotificationObject.transform.SetParent(canvasDisplay.transform);
        itemPickupNotificationObject.transform.GetComponentInChildren<Image>().sprite = itemLoot.icon;
    }


}
