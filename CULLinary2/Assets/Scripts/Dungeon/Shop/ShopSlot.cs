using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text shopItemName;
    [SerializeField] private Image shopItemIcon;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private TMP_Text itemPrice;
    [SerializeField] private Outline outline;
    public ShopItem shopItem;
    private Button button;
    private int slotIndex;

    private void Awake()
    {
        button = GetComponentInChildren<Button>();
    }

    public void SetupUI(ShopItem currentShopItem, int index)
    {
        shopItem = currentShopItem;
        slotIndex = index;
        button.onClick.AddListener(() => ShopManager.instance.HandleClick(slotIndex));
    }

    public void UpdateUI(ShopItem currentShopItem)
    {
        int level = PlayerManager.instance.upgradesArray[shopItem.shopItemId];
        int currentMoney = PlayerManager.instance.currentMoney;

        shopItemName.text = shopItem.itemName;
        shopItemIcon.sprite = shopItem.icon;
        itemDescription.text = shopItem.description[level];

        if (level >= shopItem.maxLevel)
        {
            itemPrice.text = "";
            outline.enabled = false;
            button.interactable = false;
            return;
        }
        if (currentMoney < shopItem.price[level])
        {
            outline.enabled = false;
            button.interactable = false;
        }
        itemPrice.text = "$" + shopItem.price[level];
    }

}
