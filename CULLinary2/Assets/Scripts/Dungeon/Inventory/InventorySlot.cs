using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image itemMainIcon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescription;
    public InventoryItem item;
    public int slotId;

    public void SetupSlot(int index)
    {
        slotId = index;
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => InventorySlotManager.instance.HandleClick(slotId));
    }

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

}
