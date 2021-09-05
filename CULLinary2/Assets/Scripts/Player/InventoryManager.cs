using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text inventoryCapacityText;
    [SerializeField] private GameObject inventoryPanel;
    private InventorySlot[] slots;
    public static InventoryManager instance;
    private int inventoryLimit;
    public List<Item> innerItemList;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        slots = inventoryPanel.GetComponentsInChildren<InventorySlot>();
        inventoryLimit = PlayerManager.instance ? PlayerManager.instance.inventoryLimit : 16;
        PopulateUI(PlayerManager.instance.itemList);
    }

    public bool AddItem(Item item)
    {
        if (innerItemList.Count < inventoryLimit)
        {
            innerItemList.Add(item);
            StartCoroutine(UpdateUI());
            return true;
        }
        return false;
    }

    public void RemoveItem(Item item)
    {
        innerItemList.Remove(item);
        StartCoroutine(UpdateUI());
    }

    // Tries to remove an item, given the ID.
    // If the item was not found, return false.
    // Otherwise, remove the item and return true.
    public bool RemoveIdIfPossible(int idToRemove)
    {
        for (int i = 0; i < innerItemList.Count; i++)
        {
            Item currentItem = innerItemList[i];
            if (currentItem.itemId == idToRemove)
            {
                innerItemList.RemoveAt(i);
                StartCoroutine(UpdateUI());
                return true;
            }
        }
        return false;
    }

    public void PopulateUI(List<Item> items)
    {
        innerItemList = items;
        StartCoroutine(UpdateUI());
    }

    public IEnumerator UpdateUI()
    {
        if (slots != null)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                yield return null;
                if (i < innerItemList.Count)
                {
                    slots[i].AddItem(innerItemList[i]);
                }
                else
                {
                    slots[i].ClearSlot();
                }
            }
        }
        inventoryCapacityText.text = innerItemList.Count + "/" + inventoryLimit;
        inventoryCapacityText.color = innerItemList.Count == inventoryLimit ? Color.red : Color.black;
    }

    // Checks if the item IDs specified exist in the inventory.
    // If they do, remove them and return true.
    // Otherwise, returns false.
    // 
    // NOTE: This does not call UpdateUI(), because adding the final dish to the inventory would also do so.
    public bool RemoveIdsFromInventory(int[] itemsToRemove)
    {
        // Maps item ID to number of item and list of those items
        Dictionary<int, Tuple<int, List<Item>>> itemsInInventory = new Dictionary<int, Tuple<int, List<Item>>>();
        foreach (Item i in innerItemList)
        {
            if (itemsInInventory.ContainsKey(i.itemId))
            {
                // need to do this because tuples are read-only
                Tuple<int, List<Item>> originalPair = itemsInInventory[i.itemId];
                originalPair.Item2.Add(i);
                itemsInInventory[i.itemId] = new Tuple<int, List<Item>>(
                    originalPair.Item1 + 1,
                    originalPair.Item2
                );
            }
            else
            {
                List<Item> itemsForThatId = new List<Item>();
                itemsForThatId.Add(i);
                itemsInInventory.Add(i.itemId, new Tuple<int, List<Item>>(1, itemsForThatId));
            }
        }

        // Put the items to be removed in a HashMap as well
        Dictionary<int, int> itemsToRemoveMap = new Dictionary<int, int>();
        foreach (int i in itemsToRemove)
        {
            if (itemsToRemoveMap.ContainsKey(i))
            {
                itemsToRemoveMap[i]++;
            }
            else
            {
                itemsToRemoveMap.Add(i, 1);
            }
        }

        // Checks whether we can remove the items
        foreach (KeyValuePair<int, int> pair in itemsToRemoveMap)
        {
            if (!itemsInInventory.ContainsKey(pair.Key))
            {
                // do not have that ingredient at all
                return false;
            }
            int numberRequired = pair.Value;
            int numberInInventory = itemsInInventory[pair.Key].Item1;
            if (numberInInventory < numberRequired)
            {
                // have less than the number required
                return false;
            }
        }

        // Remove the items, here we are guaranteed to have enough
        foreach (KeyValuePair<int, int> pair in itemsToRemoveMap)
        {
            List<Item> inventoryItems = itemsInInventory[pair.Key].Item2;
            int count = pair.Value;
            for (int i = 0; i < pair.Value; i++)
            {
                // just remove first one every time
                innerItemList.Remove(inventoryItems[0]);
                inventoryItems.Remove(inventoryItems[0]);
            }
        }

        // StartCoroutine(UpdateUI());
        return true;
    }
}
