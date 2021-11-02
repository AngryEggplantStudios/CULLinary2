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
    [SerializeField] private GameObject upgradeIcon;
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
        shopItemIcon.sprite = shopItem.iconArr[level];
        button.interactable = true;
        //outline.enabled = false;

        if (currentShopItem.GetType() == typeof(KeyShopItem) || currentShopItem.GetType() == typeof(UpgradeShopItem))
        {
            levelText.text = "Lvl " + level;
        }
        else
        {
            levelText.text = "";
        }

        if (level >= shopItem.maxLevel)
        {
            upgradeIcon.SetActive(false);
            return;
        }

        if (currentMoney < shopItem.price[level])
        {
            upgradeIcon.SetActive(false);
        }
        else
        {
            upgradeIcon.SetActive(true);
        }

        if (SpecialEventManager.instance.CheckIfEventIsRunning(shopItem.eventId))
        {
            upgradeIcon.SetActive(false);
        }


    }

}
