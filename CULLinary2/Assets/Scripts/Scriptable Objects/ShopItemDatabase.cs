using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Database/Shop Item Database")]
public class ShopItemDatabase : ScriptableObject
{
    public List<ShopItem> allItems;
}
