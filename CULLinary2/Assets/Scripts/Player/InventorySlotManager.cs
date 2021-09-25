using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotManager : MonoBehaviour
{
    [Header("Inventory Menu References")]
    [SerializeField] private Image itemMainIcon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescription;
    private InventorySlot[] slots;
    private int selectedSlotId;
    public void HandleClick(int slotId)
    {
        InventorySlot itemSlot = slots[slotId];
        InventoryItem item = itemSlot.item;
        if (item == null || slotId == selectedSlotId)
        {
            return;
        }
        itemMainIcon.enabled = true;
        itemMainIcon.sprite = item.icon;
        itemName.text = item.itemName;
        itemDescription.text = item.description;
        itemSlot.gameObject.GetComponent<Outline>().enabled = true;
        if (selectedSlotId != -1)
        {
            slots[selectedSlotId].gameObject.GetComponent<Outline>().enabled = false;
        }
        selectedSlotId = slotId;
    }

    private void OnEnable()
    {
        selectedSlotId = -1;
        itemName.text = "";
        itemDescription.text = "";
    }

    private void OnDisable()
    {
        if (selectedSlotId != -1)
        {
            itemMainIcon.enabled = false;
            itemMainIcon.sprite = null;
            slots[selectedSlotId].gameObject.GetComponent<Outline>().enabled = false;
        }
    }

    private void Update()
    {
        if (selectedSlotId != -1)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                HandleDiscard();
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                HandleConsume();
            }
        }
    }

    public void HandleDiscard()
    {
        InventoryItem item = slots[selectedSlotId].item;
        if (InventoryManager.instance != null && item != null)
        {
            InventoryManager.instance.RemoveItem(item);
        }
    }

    public void HandleConsume()
    {
        InventoryItem item = slots[selectedSlotId].item;
        if (InventoryManager.instance != null && item != null) //Need to check if item is consumable
        {
            Debug.Log("Consumed!");
        }
    }

    private void Start()
    {
        slots = gameObject.GetComponentsInChildren<InventorySlot>();
    }
}
