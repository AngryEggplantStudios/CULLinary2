[System.Serializable]
public class InventoryItemData
{
    //Inventory Slot ID
    public int slotId;
    //Item Id
    public int itemId;

    public InventoryItemData(int slotId, int itemId)
    {
        this.slotId = slotId;
        this.itemId = itemId;
    }
}