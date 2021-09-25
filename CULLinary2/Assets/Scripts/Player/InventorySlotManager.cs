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
    private int previousSlotId;
    public void HandleClick(int slotId)
    {
        InventorySlot itemSlot = slots[slotId];
        InventoryItem item = itemSlot.item;
        if (item == null || slotId == previousSlotId)
        {
            return;
        }
        itemMainIcon.enabled = true;
        itemMainIcon.sprite = item.icon;
        itemName.text = item.itemName;
        itemDescription.text = item.description;
        itemSlot.gameObject.GetComponent<Outline>().enabled = true;
        slots[previousSlotId].gameObject.GetComponent<Outline>().enabled = false;
        previousSlotId = slotId;
    }

    private void Start()
    {
        slots = gameObject.GetComponentsInChildren<InventorySlot>();
    }
}
