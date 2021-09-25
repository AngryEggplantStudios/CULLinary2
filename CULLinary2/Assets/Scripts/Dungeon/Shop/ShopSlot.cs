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

    private void Awake()
    {
        button = GetComponentInChildren<Button>();
    }

    public void Setup(ShopItem shopItem, int level)
    {
        this.shopItem = shopItem;
        shopItemName.text = shopItem.itemName;
        shopItemIcon.sprite = shopItem.icon;
        levelText.text = "Level " + level;
        itemDescription.text = shopItem.description[level];
        itemPrice.text = "$" + shopItem.price[level];
        button.interactable = true;
    }

    public void DisableSlot()
    {
        Button button = GetComponentInChildren<Button>();
        button.interactable = false;
    }

}
