using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text inventoryCapacityText;
    [SerializeField] private InventorySlot[] slots;
    private List<Item> itemList = new List<Item>(); // Inventory
    [SerializeField] private int inventoryLimit = 16;

    public static InventoryUI instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }

    public bool AddItem(Item item)
    {
        if (itemList.Count < inventoryLimit)
        {
            itemList.Add(item);
            for (int i = 0; i < itemList.Count; i++)
            {
                Debug.Log(itemList[i]);
            }
            //UpdateUI();
            return true;
        }

        // Not enough room
        return false;
    }

    public void PopulateUI(List<Item> items)
    {
        itemList = items;
        //UpdateUI();
    }

    public void RemoveItem(Item item)
    {
        itemList.Remove(item);
        //UpdateUI();
    }

    // Checks if the item IDs specified exist in the inventory.
    // If they do, remove them and return true.
    // Otherwise, returns false.
    public bool RemoveIdsFromInventory(int[] itemsToRemove)
    {
        // Maps item ID to number of item and list of those items
        Dictionary<int, Tuple<int, List<Item>>> itemsInInventory = new Dictionary<int, Tuple<int, List<Item>>>();
        foreach (Item i in itemList)
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
                itemList.Remove(inventoryItems[0]);
                inventoryItems.Remove(inventoryItems[0]);
            }
        }

        //UpdateUI();
        return true;
    }



    private void UpdateUI()
    {
        if (slots != null)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (i < itemList.Count)
                {
                    slots[i].AddItem(itemList[i]);
                }
                else
                {
                    slots[i].ClearSlot();
                }
            }
        }
        inventoryCapacityText.text = itemList.Count + "/" + inventoryLimit;
        inventoryCapacityText.color = itemList.Count == inventoryLimit ? Color.red : Color.black;
    }
}
