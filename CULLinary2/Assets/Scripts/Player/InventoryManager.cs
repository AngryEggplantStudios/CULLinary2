using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : SingletonGeneric<InventoryManager>
{
    public List<InventoryItem> itemListReference;

    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    // All money texts go here
    [SerializeField] private List<TextMeshProUGUI> moneyTexts;
    [SerializeField] private TMP_Text healthAmount;
    [Header("Consumables")]
    [SerializeField] private TMP_Text healthPill;
    [SerializeField] private TMP_Text staminaPill;
    [SerializeField] private TMP_Text potion;
    [SerializeField] private TMP_Text pfizerShot;
    [SerializeField] private TMP_Text modernaShot;
    [Header("Weapon & Skill references")]
    [SerializeField] private Image primaryWeaponImage;
    [SerializeField] private Image secondarySkillImage;
    [Header("Stats")]
    [SerializeField] private TMP_Text staminaText;
    [SerializeField] private TMP_Text baseDamageText;
    [SerializeField] private TMP_Text weaponDamageText;
    [SerializeField] private TMP_Text totalDamageText;
    [SerializeField] private TMP_Text critChanceText;
    [SerializeField] private TMP_Text evasionChanceText;
    [SerializeField] private TMP_Text secondaryDamageText;
    private InventorySlot[] slots;
    private int inventoryLimit = 20;

    // Cache the inventory dictionary generated by InventoryToDictionary for performance reasons
    private Dictionary<int, Tuple<int, List<InventoryItem>>> inventoryDictionaryCache;
    private bool isCacheValid = false;

    private void Start()
    {
        slots = inventoryPanel.GetComponentsInChildren<InventorySlot>();
        PopulateUI(PlayerManager.instance.itemList);
    }

    private void UpdateWeaponSkillStats()
    {
        WeaponSkillItem primaryWeapon = DatabaseLoader.GetWeaponSkillById(PlayerManager.instance.currentWeaponHeld);
        WeaponSkillItem secondarySkill = DatabaseLoader.GetWeaponSkillById(PlayerManager.instance.currentSecondaryHeld);
        int primaryWeaponDamage = 0;
        int secondarySkillDamage = 0;
        if (primaryWeapon.GetType() == typeof(WeaponItem))
        {
            WeaponItem weaponItem = (WeaponItem)primaryWeapon;
            primaryWeaponImage.sprite = weaponItem.icon;
            primaryWeaponDamage = weaponItem.baseDamage[PlayerManager.instance.weaponSkillArray[weaponItem.weaponSkillId]];
            weaponDamageText.text = primaryWeaponDamage + " DMG";
        }
        if (secondarySkill.GetType() == typeof(SkillItem))
        {
            SkillItem skillItem = (SkillItem)secondarySkill;
            secondarySkillImage.sprite = skillItem.icon;
            secondarySkillDamage = skillItem.attackDamage[PlayerManager.instance.weaponSkillArray[skillItem.weaponSkillId]];
        }
        staminaText.text = PlayerManager.instance.currentStamina + " / " + PlayerManager.instance.maxStamina;
        baseDamageText.text = PlayerManager.instance.isMeleeDamageDoubled ? (PlayerManager.instance.meleeDamage * 2) + " DMG" : PlayerManager.instance.meleeDamage + " DMG";
        int minTotalMeleeDamage = Mathf.RoundToInt((PlayerManager.instance.isMeleeDamageDoubled ? PlayerManager.instance.meleeDamage * 2 : PlayerManager.instance.meleeDamage) + 0.85f * primaryWeaponDamage);
        int maxTotalMeleeDamage = Mathf.RoundToInt((PlayerManager.instance.isMeleeDamageDoubled ? PlayerManager.instance.meleeDamage * 2 : PlayerManager.instance.meleeDamage) + 1.15f * primaryWeaponDamage);
        totalDamageText.text = minTotalMeleeDamage + " ~ " + maxTotalMeleeDamage + " DMG";
        int minSecondaryDamage = Mathf.RoundToInt(secondarySkillDamage * 0.85f);
        int maxSecondaryDamage = Mathf.RoundToInt(secondarySkillDamage * 1.15f);
        secondaryDamageText.text = minSecondaryDamage + " ~ " + maxSecondaryDamage + " DMG";
        critChanceText.text = (PlayerManager.instance.criticalChance + PlayerManager.instance.criticalBonus) + "%";
        evasionChanceText.text = (PlayerManager.instance.evasionBonus + PlayerManager.instance.evasionChance) + "%";
    }


    // Adds an item and updates inventory, recipe and order UIs
    public bool AddItem(InventoryItem item)
    {
        if (itemListReference.Count < inventoryLimit)
        {
            itemListReference.Add(item);
            isCacheValid = false;
            // May affect Recipe, Orders UI as well
            UIController.UpdateAllUIs();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveItem(InventoryItem item)
    {
        itemListReference.Remove(item);
        isCacheValid = false;
        // May affect Recipe, Orders UI as well
        UIController.UpdateAllUIs();
    }

    // Tries to remove an item, given the ID.
    // If the item was not found, return false.
    // Otherwise, remove the item and return true.
    //
    // Does not call UpdateAllUis, as completing the order will call it
    public bool RemoveIdIfPossible(int idToRemove)
    {
        for (int i = 0; i < itemListReference.Count; i++)
        {
            InventoryItem currentItem = itemListReference[i];
            if (currentItem.inventoryItemId == idToRemove)
            {
                itemListReference.RemoveAt(i);
                isCacheValid = false;
                return true;
            }
        }
        return false;
    }

    public void PopulateUI(List<InventoryItem> items)
    {
        itemListReference = items;
        UIController.UpdateAllUIs();
    }

    public void ForceUIUpdate()
    {
        StopAllCoroutines();
        StartCoroutine(UpdateUI());
    }

    private IEnumerator UpdateUI()
    {
        UpdateWeaponSkillStats();
        healthPill.text = "x " + PlayerManager.instance.healthPill;
        staminaPill.text = "x " + PlayerManager.instance.staminaPill;
        potion.text = "x " + PlayerManager.instance.potion;
        pfizerShot.text = "x " + PlayerManager.instance.pfizerShot;
        modernaShot.text = "x " + PlayerManager.instance.modernaShot;
        string currentMoney = PlayerManager.instance.currentMoney.ToString();
        foreach (TextMeshProUGUI text in moneyTexts)
        {
            text.text = currentMoney;
        }
        healthAmount.text = Mathf.CeilToInt(PlayerManager.instance.currentHealth) + "/" + Mathf.CeilToInt(PlayerManager.instance.maxHealth);

        if (slots != null)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                yield return null;
                if (i < itemListReference.Count)
                {
                    slots[i].AddItem(itemListReference[i]);
                }
                else
                {
                    slots[i].ClearSlot();
                }
            }
        }
    }

    // Builds a hash table based on the current contents of the inventory
    // 
    // Returns a dictionary that maps an item ID to the number of those items,
    // as well as a list of references to the actual items
    private Dictionary<int, Tuple<int, List<InventoryItem>>> InventoryToDictionary()
    {
        if (!isCacheValid)
        {
            Dictionary<int, Tuple<int, List<InventoryItem>>> itemsInInventory =
                new Dictionary<int, Tuple<int, List<InventoryItem>>>();

            foreach (InventoryItem i in itemListReference)
            {
                if (itemsInInventory.ContainsKey(i.inventoryItemId))
                {
                    // need to do this because tuples are read-only
                    Tuple<int, List<InventoryItem>> originalPair = itemsInInventory[i.inventoryItemId];
                    originalPair.Item2.Add(i);
                    itemsInInventory[i.inventoryItemId] = new Tuple<int, List<InventoryItem>>(
                        originalPair.Item1 + 1,
                        originalPair.Item2
                    );
                }
                else
                {
                    List<InventoryItem> itemsForThatId = new List<InventoryItem>();
                    itemsForThatId.Add(i);
                    itemsInInventory.Add(i.inventoryItemId, new Tuple<int, List<InventoryItem>>(1, itemsForThatId));
                }
            }
            inventoryDictionaryCache = itemsInInventory;
            isCacheValid = true;
        }
        return inventoryDictionaryCache;
    }

    // Checks if the inventory has this item
    public bool CheckIfExists(int itemId)
    {
        foreach (InventoryItem i in itemListReference)
        {
            if (i.inventoryItemId == itemId)
            {
                return true;
            }
        }
        return false;
    }

    // Counts how many items of a certain type there are in the inventory
    public int GetAmountOfItem(int itemId)
    {
        int amount = 0;
        foreach (InventoryItem i in itemListReference)
        {
            if (i.inventoryItemId == itemId)
            {
                amount++;
            }
        }
        return amount;
    }

    // Given a list of (item IDs, the count required), check if
    // there is enough of the item in the inventory. The list
    // is not allowed to contain duplicate item IDs.
    // 
    // Returns true if all the items exist in the inventory.
    // 
    // The outInvReqCounter returns a list of
    // (item ID, amount in inventory, count required).
    public bool CheckIfItemsExist(List<(int, int)> itemsList, out List<(int, int, int)> outInvReqCounter)
    {
        Dictionary<int, Tuple<int, List<InventoryItem>>> itemsInInventory = InventoryToDictionary();
        List<(int, int, int)> inventoryCountToRequiredCount = new List<(int, int, int)>();
        bool doAllItemsExist = true;
        foreach ((int itemId, int reqCount) in itemsList)
        {
            int inventoryCount = 0;
            // Check the inventory for the items
            if (itemsInInventory.ContainsKey(itemId))
            {
                inventoryCount = itemsInInventory[itemId].Item1;
            }
            if (inventoryCount < reqCount)
            {
                doAllItemsExist = false;
            }
            inventoryCountToRequiredCount.Add((itemId, inventoryCount, reqCount));
        }
        outInvReqCounter = inventoryCountToRequiredCount;
        return doAllItemsExist;
    }

    // Checks if the item IDs specified exist in the inventory.
    // If they do, remove them and return true.
    // Otherwise, returns false.
    // 
    // NOTE: This does not call UpdateUI(), because adding the final dish to the inventory would also do so.
    public bool RemoveIdsFromInventory(List<(int, int)> itemsToRemove)
    {
        if (!CheckIfItemsExist(itemsToRemove, out _))
        {
            return false;
        }

        // Remove the items, here we are guaranteed to have enough
        Dictionary<int, Tuple<int, List<InventoryItem>>> itemsInInventory = InventoryToDictionary();
        foreach ((int itemId, int count) in itemsToRemove)
        {
            List<InventoryItem> inventoryItems = itemsInInventory[itemId].Item2;
            for (int i = 0; i < count; i++)
            {
                // just remove first one every time
                itemListReference.Remove(inventoryItems[0]);
                inventoryItems.Remove(inventoryItems[0]);
            }
        }
        return true;
    }

    // Based on the current inventory items and the current unlocked recipes,
    // generate a set of IDs of the recipes that the player is able to cook
    public HashSet<int> GetIdsOfCookableRecipes()
    {
        Dictionary<int, Tuple<int, List<InventoryItem>>> itemsInInventory = InventoryToDictionary();
        List<Recipe> unlockedRecipes = RecipeManager.instance.GetUnlockedRecipes();
        HashSet<int> cookableRecipes = new HashSet<int>();
        foreach (Recipe r in unlockedRecipes)
        {
            List<(int, int)> ingredientIds = r.GetIngredientIds();
            if (CheckIfItemsExist(ingredientIds, out _))
            {
                cookableRecipes.Add(r.recipeId);
            }
        }
        return cookableRecipes;
    }
}
