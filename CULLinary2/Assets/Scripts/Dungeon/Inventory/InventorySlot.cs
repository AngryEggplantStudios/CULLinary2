using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image itemMainIcon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private TMP_Text itemType;
    [SerializeField] private TMP_Text itemExpiry;
    [SerializeField] private TMP_Text itemEffect;
    private InventoryItem item;

    public void AddItem(InventoryItem newItem)
    {

        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
    }
    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void HandleClick()
    {
        if (item == null)
        {
            return;
        }
        itemMainIcon.enabled = true;
        itemMainIcon.sprite = item.icon;
        itemName.text = item.itemName;
        itemDescription.text = item.description;
    }

    public void RemoveItemFromInventory()
    {
        InventoryManager.instance.RemoveItem(item);
    }
}
