using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Inventory/Shop Item")]
public class ShopItem : Item
{
    public int maxLevel;
    public int shopItemId;
    public int[] price;
    public string[] description;

    //Effects
    public int[] attackIncrement;

    //Event Id
    public int[] events;

}