using UnityEngine;

[CreateAssetMenu(fileName = "New Event Shop Item", menuName = "Inventory/Event Shop Item")]
public class EventShopItem : ShopItem
{
    public bool CheckIfPurchased()
    {
        if (PlayerManager.instance == null)
        {
            return false;
        }
        switch (eventId)
        {
            case 2:
                return PlayerManager.instance.isTruckUnlocked;
        }
        return false;
    }

}