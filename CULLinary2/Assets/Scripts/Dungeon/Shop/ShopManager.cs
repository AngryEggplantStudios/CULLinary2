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
    private int selectedSlotId = -1;
    private List<ShopSlot> slots;

    public void HandlePurchase()
    {
        ShopItem itemPurchased = slots[selectedSlotId].shopItem;
        int itemPrice = itemPurchased.price[PlayerManager.instance.upgradesArray[itemPurchased.shopItemId]];
        if (itemPrice > PlayerManager.instance.currentMoney)
        {
            return;
        }
        //Effects
        PlayerManager.instance.meleeDamage += itemPurchased.attackIncrement[PlayerManager.instance.upgradesArray[itemPurchased.shopItemId]];
        PlayerManager.instance.currentMoney -= itemPrice;
        PlayerManager.instance.upgradesArray[itemPurchased.shopItemId]++;

        // Update all money UIs
        InventoryManager.instance.StopAllCoroutines();
        InventoryManager.instance.StartCoroutine(InventoryManager.instance.UpdateUI());
        
        LoadShop();
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
        if (selectedSlotId != -1)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                HandlePurchase();
            }
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

    public void LoadShop()
    {
        foreach (ShopSlot slot in slots)
        {
            int level = PlayerManager.instance.upgradesArray[slot.shopItem.shopItemId];
            slot.Setup(slot.shopItem, level);
            if (PlayerManager.instance.currentMoney < slot.shopItem.price[level])
            {
                slot.DisableSlot();
            }
        }
        moneyText.text = PlayerManager.instance.currentMoney.ToString();
    }
}
