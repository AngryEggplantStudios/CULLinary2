using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : SingletonGeneric<ShopManager>
{
    [SerializeField] private GameObject slotsParentObject;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private GameObject shopPanel;
    private int selectedSlotId = -1;
    private List<ShopSlot> slots;

    public void HandlePurchase()
    {
        if (selectedSlotId == -1)
        {
            return;
        }

        ShopItem itemPurchased = slots[selectedSlotId].shopItem;
        int currentLevel = PlayerManager.instance.upgradesArray[itemPurchased.shopItemId];
        int itemPrice = itemPurchased.price[currentLevel];
        if (itemPrice > PlayerManager.instance.currentMoney)
        {
            return;
        }
        //Effects
        PlayerManager.instance.meleeDamage += itemPurchased.attackIncrement[currentLevel];

        //Handle Special Events
        if (itemPurchased.events[currentLevel] != 0 && SpecialEventManager.instance != null)
        {
            SpecialEventManager.instance.PlayEvent(itemPurchased.events[currentLevel]);
        }

        // Update all money UIs
        PlayerManager.instance.upgradesArray[itemPurchased.shopItemId]++;
        PlayerManager.instance.currentMoney -= itemPrice;

        InventoryManager.instance.StopAllCoroutines();
        InventoryManager.instance.StartCoroutine(InventoryManager.instance.UpdateUI());

        UpdateShop();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && shopPanel.activeSelf)
        {
            HandlePurchase();
        }
    }

    public void SetupShop()
    {
        List<ShopItem> shopItemList = DatabaseLoader.GetAllShopItems();
        slots = new List<ShopSlot>();
        foreach (ShopItem item in shopItemList)
        {
            GameObject slotObject = Instantiate(slotPrefab);
            ShopSlot slot = slotObject.GetComponent<ShopSlot>();
            Button button = slotObject.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => HandleClick(item.shopItemId));
            slot.Setup(item, PlayerManager.instance.upgradesArray[item.shopItemId]);
            slots.Add(slot);
            slotObject.transform.SetParent(slotsParentObject.transform);
        }
        moneyText.text = PlayerManager.instance.currentMoney.ToString();
    }

    public void UpdateShop()
    {
        foreach (ShopSlot slot in slots)
        {
            int level = PlayerManager.instance.upgradesArray[slot.shopItem.shopItemId];
            slot.Setup(slot.shopItem, level);

            if (level >= slot.shopItem.maxLevel)
            {
                slot.DisableSlot();
                slot.HandleMaxLevel(slot.shopItem, level);
                return;
            }

            slot.IncrementLevel(slot.shopItem, level);

            if (PlayerManager.instance.currentMoney < slot.shopItem.price[level])
            {
                slot.DisableSlot();
            }

        }
        moneyText.text = PlayerManager.instance.currentMoney.ToString();
    }

    public void LoadShop()
    {
        selectedSlotId = -1;
        UpdateShop();
    }
}
