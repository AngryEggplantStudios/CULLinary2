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
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemPrice;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private TMP_Text currentLevelText;
    [SerializeField] private TMP_Text nextLevelText;
    [SerializeField] private TMP_Text currentLevelEffectText;
    [SerializeField] private TMP_Text nextLevelEffectText;
    [SerializeField] private TMP_Text nextLevelIncrementText;
    [SerializeField] private GameObject levelDetails;
    [SerializeField] private GameObject upgradeButton;
    [SerializeField] private GameObject itemPanel;
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
        itemPanel.SetActive(false);
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
        UpdateShopDescription();
    }

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
        itemPanel.SetActive(false);
    }

    public void UpdateShop()
    {
        selectedSlotId = -1;
        foreach (ShopSlot slot in slots)
        {
            slot.UpdateUI(slot.shopItem);
        }
    }

    public void UpdateShopDescription()
    {
        if (selectedSlotId == -1)
        {
            return;
        }
        itemPanel.SetActive(true);

        ShopItem itemSelected = slots[selectedSlotId].shopItem;
        int currentLevel = PlayerManager.instance.upgradesArray[itemSelected.shopItemId];

        itemDescription.text = itemSelected.description[currentLevel];
        itemIcon.sprite = itemSelected.iconArr[currentLevel];
        itemPrice.text = "$" + itemSelected.price[currentLevel];

        if (itemSelected.GetType() == typeof(UpgradeShopItem))
        {
            levelDetails.SetActive(true);
            UpgradeShopItem upgradeItemSelected = (UpgradeShopItem)itemSelected;
            itemName.text = itemSelected.itemName + " (Lvl " + currentLevel + ")";
            currentLevelText.text = "Current: Lvl " + currentLevel;

            //Check if max level
            bool isMaxLevel = false;
            if (currentLevel + 1 > itemSelected.maxLevel)
            {
                nextLevelText.text = "Max Level";
                nextLevelEffectText.text = "";
                nextLevelIncrementText.text = "";
                isMaxLevel = true;
            }
            else
            {
                nextLevelText.text = "Current: Lvl " + (currentLevel + 1);
            }
            switch ((int)upgradeItemSelected.upgradeType)
            {
                case (int)UpgradeType.ATTACK_INCREMENT:
                    currentLevelEffectText.text = "Melee: " + PlayerManager.instance.meleeDamage + " DMG";
                    if (!isMaxLevel)
                    {
                        nextLevelEffectText.text = "Melee: " + (upgradeItemSelected.attackIncrement[currentLevel] + PlayerManager.instance.meleeDamage) + " DMG";
                        nextLevelIncrementText.text = "(" + upgradeItemSelected.attackIncrement[currentLevel] + ")";
                    }
                    break;
                case (int)UpgradeType.CRIT_CHANCE_INCREMENT:
                    currentLevelEffectText.text = "Crit: " + PlayerManager.instance.meleeDamage + " %";
                    if (!isMaxLevel)
                    {
                        nextLevelEffectText.text = "Crit: " + (upgradeItemSelected.criticalChance[currentLevel] + PlayerManager.instance.criticalChance) + " %";
                        nextLevelIncrementText.text = "(" + upgradeItemSelected.criticalChance[currentLevel] + ")";
                    }
                    break;
                case (int)UpgradeType.MAX_STAMINA_INCREMENT:
                    currentLevelEffectText.text = "Stamina: " + PlayerManager.instance.meleeDamage + " MP";
                    if (!isMaxLevel)
                    {
                        nextLevelEffectText.text = "Stamina: " + (upgradeItemSelected.maximumStaminaIncrement[currentLevel] + PlayerManager.instance.maxStamina) + " MP";
                        nextLevelIncrementText.text = "(" + upgradeItemSelected.maximumStaminaIncrement[currentLevel] + ")";
                    }
                    break;
                case (int)UpgradeType.EVASION_CHANCE_INCREMENT:
                    currentLevelEffectText.text = "Evasion Chance: " + PlayerManager.instance.meleeDamage + " %";
                    if (!isMaxLevel)
                    {
                        nextLevelEffectText.text = "Evasion Chance: " + (upgradeItemSelected.evasionChance[currentLevel] + PlayerManager.instance.evasionChance) + " %";
                        nextLevelIncrementText.text = "(" + upgradeItemSelected.evasionChance[currentLevel] + ")";
                    }
                    break;
                case (int)UpgradeType.MAX_HEALTH_INCREMENT:
                    currentLevelEffectText.text = "Max Health: " + PlayerManager.instance.meleeDamage + " HP";
                    if (!isMaxLevel)
                    {
                        nextLevelEffectText.text = "Max Health: " + (upgradeItemSelected.maximumHealthIncrement[currentLevel] + PlayerManager.instance.maxHealth) + " HP";
                        nextLevelIncrementText.text = "(" + upgradeItemSelected.maximumHealthIncrement[currentLevel] + ")";
                    }
                    break;
                case (int)UpgradeType.NO_EFFECT:
                default:
                    break;
            }
        }

        else if (itemSelected.GetType() == typeof(KeyShopItem))
        {
            levelDetails.SetActive(true);
            KeyShopItem keyItemSelected = (KeyShopItem)itemSelected;
            itemName.text = itemSelected.itemName + " (Lvl " + currentLevel + ")";
            currentLevelText.text = currentLevel.ToString();
            if (currentLevel + 1 > itemSelected.maxLevel)
            {
                nextLevelText.text = "Max Level";
                return;
            }
            else
            {
                nextLevelText.text = (currentLevel + 1).ToString();
            }
        }

        else if (itemSelected.GetType() == typeof(EventShopItem))
        {
            levelDetails.SetActive(false);
            EventShopItem eventItemSelected = (EventShopItem)itemSelected;
        }
        else if (itemSelected.GetType() == typeof(ConsumableShopItem))
        {
            levelDetails.SetActive(false);
            ConsumableShopItem consumableItemSelected = (ConsumableShopItem)itemSelected;
        }

    }
}
