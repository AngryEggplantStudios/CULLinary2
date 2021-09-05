using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Inventory/Inventory Item")]
public class InventoryItem : Item
{
    public int inventoryItemId;
    public bool isConsumable;
    public float healHpAmount;
    public float increaseMaxHpAmount;
    public float increaseBaseDamageAmount;
}