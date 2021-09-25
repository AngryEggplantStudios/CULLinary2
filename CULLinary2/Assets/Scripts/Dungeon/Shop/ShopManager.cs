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
    private int selectedSlotId;
    private List<ShopSlot> slots;

    public void HandlePurchase()
    {
        ShopItem itemPurchased = slots[selectedSlotId].shopItem;
        int itemPrice = itemPurchased.price[PlayerManager.instance.upgradesArray[itemPurchased.shopItemId]];
        if (itemPrice > PlayerManager.instance.currentMoney)
        {
            return;
        }
        PlayerManager.instance.currentMoney -= itemPrice;
        moneyText.text = PlayerManager.instance.currentMoney.ToString();
        PlayerManager.instance.upgradesArray[itemPurchased.shopItemId]++;
        LoadShop();
    }

    private void OnEnable()
    {
        foreach (ShopSlot slot in slots)
        {
            slot.gameObject.GetComponent<Outline>().enabled = false;
        }
        selectedSlotId = -1;
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
            slot.Setup(slot.shopItem, PlayerManager.instance.upgradesArray[slot.shopItem.shopItemId]);
        }
        moneyText.text = PlayerManager.instance.currentMoney.ToString();
    }
}
