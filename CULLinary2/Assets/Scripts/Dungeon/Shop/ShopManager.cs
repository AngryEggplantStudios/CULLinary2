using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : SingletonGeneric<ShopManager>
{
    [Header("Prefabs & References")]
    [SerializeField] private GameObject slotsParentObject;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private GameObject shopPanel;
    private int selectedSlotId = -1;
    private List<ShopSlot> slots;

    public void HandlePurchase()
    {
        //Guard clause to check if there's a valid slot selected
        if (selectedSlotId == -1)
        {
            return;
        }

        ShopItem itemPurchased = slots[selectedSlotId].shopItem;
        int currentLevel = PlayerManager.instance.upgradesArray[itemPurchased.shopItemId];
        int itemPrice = itemPurchased.price[currentLevel];

        //Guard clause to check if player has enough money
        if (itemPrice > PlayerManager.instance.currentMoney)
        {
            return;
        }

        //Handle different items
        if (itemPurchased.GetType() == typeof(UpgradeShopItem))
        {
            Debug.Log("Bought upgrade!");
            UpgradeShopItem upgradeItemPurchased = (UpgradeShopItem)itemPurchased;
            switch ((int)upgradeItemPurchased.upgradeType)
            {
                case (int)UpgradeType.ATTACK_INCREMENT:
                    PlayerManager.instance.meleeDamage += upgradeItemPurchased.attackIncrement[currentLevel];
                    break;
                case (int)UpgradeType.CRIT_CHANCE_INCREMENT:
                    PlayerManager.instance.criticalChance += upgradeItemPurchased.criticalChance[currentLevel];
                    break;
                case (int)UpgradeType.MAX_STAMINA_INCREMENT:
                    PlayerManager.instance.maxStamina += upgradeItemPurchased.maximumStaminaIncrement[currentLevel];
                    break;
                case (int)UpgradeType.EVASION_CHANCE_INCREMENT:
                    PlayerManager.instance.evasionChance += upgradeItemPurchased.evasionChance[currentLevel];
                    break;
                case (int)UpgradeType.MAX_HEALTH_INCREMENT:
                    PlayerManager.instance.maxHealth += upgradeItemPurchased.maximumHealthIncrement[currentLevel];
                    break;
                case (int)UpgradeType.NO_EFFECT:
                default:
                    Debug.Log("Upgrade has no effect");
                    break;
            }
            PlayerManager.instance.upgradesArray[upgradeItemPurchased.shopItemId]++;
        }
        else if (itemPurchased.GetType() == typeof(EventShopItem))
        {
            Debug.Log("Bought event item!");
            EventShopItem eventItemPurchased = (EventShopItem)itemPurchased;
            SpecialEventManager.instance.PlayEvent(eventItemPurchased.eventId);
        }
        else if (itemPurchased.GetType() == typeof(KeyShopItem))
        {
            Debug.Log("Bought key item!");
            KeyShopItem keyItemPurchased = (KeyShopItem)itemPurchased;
            PlayerManager.instance.upgradesArray[keyItemPurchased.shopItemId]++;
        }
        else if (itemPurchased.GetType() == typeof(ConsumableShopItem))
        {
            Debug.Log("Bought consumable!");
            ConsumableShopItem consumableItemPurchased = (ConsumableShopItem)itemPurchased;
            switch (consumableItemPurchased.shopItemId)
            {
                case 7:
                    PlayerManager.instance.consumables[0]++;
                    break;
                case 8:
                    PlayerManager.instance.consumables[1]++;
                    break;
                case 9:
                    PlayerManager.instance.consumables[2]++;
                    break;
            }
        }

        // Update all UIs
        PlayerManager.instance.currentMoney -= itemPrice;
        InventoryManager.instance.StopAllCoroutines();
        InventoryManager.instance.StartCoroutine(InventoryManager.instance.UpdateUI());
        UpdateShop();
        UIController.instance.UpdateFixedHUD();
    }

    public void HandleClick(int slotId)
    {
        if (slotId == selectedSlotId)
        {
            return;
        }
        slots[slotId].gameObject.GetComponent<Outline>().enabled = true;
        if (selectedSlotId != -1)
        {
            slots[selectedSlotId].gameObject.GetComponent<Outline>().enabled = false;
        }
        selectedSlotId = slotId;
    }

    // // Handled in UIController instead
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.F) && shopPanel.activeSelf)
    //     {
    //         HandlePurchase();
    //     }
    // }

    public void SetupShop()
    {
        List<ShopItem> shopItemList = DatabaseLoader.GetAllShopItems();
        slots = new List<ShopSlot>();
        int currentSlotId = 0;
        foreach (ShopItem item in shopItemList)
        {
            GameObject slotObject = Instantiate(slotPrefab);
            ShopSlot slot = slotObject.GetComponent<ShopSlot>();
            slot.SetupUI(item, currentSlotId);
            currentSlotId++;
            slots.Add(slot);
            slotObject.transform.SetParent(slotsParentObject.transform);
        }
        UpdateShop();
    }

    public void UpdateShop()
    {
        selectedSlotId = -1;
        foreach (ShopSlot slot in slots)
        {
            slot.UpdateUI(slot.shopItem);
        }
        moneyText.text = PlayerManager.instance.currentMoney.ToString();
    }

}
