using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : SingletonGeneric<InventoryManager>
{
    [Header("UI References")]
    [SerializeField] private Text inventoryCapacityText;
    [SerializeField] private GameObject inventoryPanel;
    private InventorySlot[] slots;

    private int inventoryLimit;
    public List<InventoryItem> itemListReference;

    // Cache the inventory dictionary generated by InventoryToDictionary for performance reasons
    private Dictionary<int, Tuple<int, List<InventoryItem>>> inventoryDictionaryCache;
    private bool isCacheValid = false;

    private void Start()
    {
        slots = inventoryPanel.GetComponentsInChildren<InventorySlot>();
        inventoryLimit = PlayerManager.instance ? PlayerManager.instance.inventoryLimit : 16;
        PopulateUI(PlayerManager.instance.itemList);
    }

    public bool AddItem(InventoryItem item)
    {
        if (itemListReference.Count < inventoryLimit)
        {
            itemListReference.Add(item);
            isCacheValid = false;
            StartCoroutine(UpdateUI());
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
        StartCoroutine(UpdateUI());
    }

    // Tries to remove an item, given the ID.
    // If the item was not found, return false.
    // Otherwise, remove the item and return true.
    public bool RemoveIdIfPossible(int idToRemove)
    {
        for (int i = 0; i < itemListReference.Count; i++)
        {
            InventoryItem currentItem = itemListReference[i];
            if (currentItem.inventoryItemId == idToRemove)
            {
                itemListReference.RemoveAt(i);
                isCacheValid = false;
                StartCoroutine(UpdateUI());
                return true;
            }
        }
        return false;
    }

    public void PopulateUI(List<InventoryItem> items)
    {
        itemListReference = items;
        StartCoroutine(UpdateUI());
    }

    public IEnumerator UpdateUI()
    {
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
        inventoryCapacityText.text = itemListReference.Count + "/" + inventoryLimit;
        inventoryCapacityText.color = itemListReference.Count == inventoryLimit ? Color.red : Color.black;
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

    // Given a list of item IDs, check if those IDs exist in the
    // inventory. The list is allowed to contain duplicates.
    // Returns true if all the items exist in the inventory.
    // 
    // The outItemsMap is a dictionary of item IDs to the quantity of that
    // item in the itemsList, that will be generated by the method.
    //
    // The outMissingItems is an array of item IDs which has the same
    // order as the given itemsList array. The method checks if each
    // item exists in the  inventory, and pairs it with a boolean
    // value. The item ID is paired with true if it is found in
    // the inventory, and false if it is not.
    public bool CheckIfItemsExist(
        int[] itemsList,
        out Dictionary<int, int> outItemsMap,
        out (int, bool)[] outMissingItems
    )
    {
        Dictionary<int, Tuple<int, List<InventoryItem>>> itemsInInventory = InventoryToDictionary();
        List<(int, bool)> missingItems = new List<(int, bool)>();
        // Use another dictionary to keep track of the number of the same items seen
        Dictionary<int, int> itemsSeen = new Dictionary<int, int>();
        bool doAllItemsExist = true;

        // Put the items into missingItems after checking the inventory
        foreach (int i in itemsList)
        {
            // Populate the items seen
            if (itemsSeen.ContainsKey(i))
            {
                itemsSeen[i]++;
            }
            else
            {
                itemsSeen.Add(i, 1);
            }

            // Check the inventory for the items
            if (!itemsInInventory.ContainsKey(i))
            {
                // None of this item at all
                missingItems.Add((i, false));
                doAllItemsExist = false;
            }
            else
            {
                int numberInInventory = itemsInInventory[i].Item1;
                // Check if the number of this item in the inventory
                // is less than what we have seen so far
                bool doesThisItemExist = itemsSeen[i] <= numberInInventory;
                missingItems.Add((i, doesThisItemExist));
                if (!doesThisItemExist)
                {
                    doAllItemsExist = false;
                }
            }
        }
        outItemsMap = itemsSeen;
        outMissingItems = missingItems.ToArray();
        return doAllItemsExist;
    }

    // Checks if the item IDs specified exist in the inventory.
    // If they do, remove them and return true.
    // Otherwise, returns false.
    // 
    // NOTE: This does not call UpdateUI(), because adding the final dish to the inventory would also do so.
    public bool RemoveIdsFromInventory(int[] itemsToRemove)
    {
        Dictionary<int, Tuple<int, List<InventoryItem>>> itemsInInventory = InventoryToDictionary();

        // Check the items and put the items to be removed in a HashMap
        Dictionary<int, int> itemsToRemoveMap = new Dictionary<int, int>();
        if (!CheckIfItemsExist(itemsToRemove, out itemsToRemoveMap, out _))
        {
            return false;
        }

        // Remove the items, here we are guaranteed to have enough
        foreach (KeyValuePair<int, int> pair in itemsToRemoveMap)
        {
            List<InventoryItem> inventoryItems = itemsInInventory[pair.Key].Item2;
            int count = pair.Value;
            for (int i = 0; i < pair.Value; i++)
            {
                // just remove first one every time
                itemListReference.Remove(inventoryItems[0]);
                inventoryItems.Remove(inventoryItems[0]);
            }
        }

        // StartCoroutine(UpdateUI());
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
            int[] ingredientIds = r.GetIngredientIds();
            if (CheckIfItemsExist(ingredientIds, out _, out _))
            {
                cookableRecipes.Add(r.recipeId);
            }
        }
        return cookableRecipes;
    }
}
