using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Database/Inventory Item Database")]
public class InventoryItemDatabase : ScriptableObject
{
    public List<InventoryItem> allItems;
}
