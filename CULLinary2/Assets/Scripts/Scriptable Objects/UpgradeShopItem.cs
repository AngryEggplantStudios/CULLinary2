using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade Shop Item", menuName = "Inventory/Upgrade Shop Item")]
public class UpgradeShopItem : ShopItem
{

    public int[] attackIncrement;
    public int[] maximumHealthIncrement;
    public int[] maximumStaminaIncrement;
    public int[] criticalChance;
    public int[] evasionChance;
    public UpgradeType upgradeType;
}
