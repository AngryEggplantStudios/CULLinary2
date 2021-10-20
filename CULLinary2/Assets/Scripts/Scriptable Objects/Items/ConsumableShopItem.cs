using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Shop Item", menuName = "Inventory/Consumable Shop Item")]
public class ConsumableShopItem : ShopItem
{
    public float healAmount;
    public float staminaAmount;
    public int duration;
    public int evasionBoost;
    public int critBoost;
    public bool attackBoost;


}