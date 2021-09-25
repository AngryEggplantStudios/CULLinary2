using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManagerTwo : SingletonGeneric<InventoryManagerTwo>
{
    [Header("UI References")]
    [SerializeField] private GameObject inventorySlotParentObject;
    private InventorySlot[] slots;
    private Dictionary<int, InventoryItem> itemDictReference;
    public override void Awake()
    {
        base.Awake();
        slots = inventorySlotParentObject.GetComponentsInChildren<InventorySlot>();
        itemDictReference = PlayerManager.instance == null ? null : PlayerManager.instance.itemDict;
    }

    public void UpdateInventory()
    {
        if (PlayerManager.instance == null)
        {
            return;
        }
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].AddItem(itemDictReference[i]);
        }
    }

    public int CheckNextSlot()
    {
        foreach (KeyValuePair<int, InventoryItem> entry in itemDictReference)
        {
            if (entry.Value == null)
            {
                return entry.Key;
            }
        }
        return -1;
    }

    public bool AddItem(InventoryItem item)
    {
        if (PlayerManager.instance == null)
        {
            return false;
        }
        int checkSlotIndex = CheckNextSlot();
        if (checkSlotIndex == -1)
        {
            return false;
        }
        itemDictReference[checkSlotIndex] = item;
        return true;
    }

    public bool RemoveItem(InventoryItem item)
    {
        foreach (KeyValuePair<int, InventoryItem> entry in itemDictReference)
        {
            if (entry.Value.inventoryItemId == item.inventoryItemId)
            {
                itemDictReference[entry.Key] = null;
                return true;
            }
        }
        return false;
    }

    public bool RemoveItemById(int id)
    {
        foreach (KeyValuePair<int, InventoryItem> entry in itemDictReference)
        {
            if (entry.Value.inventoryItemId == id)
            {
                itemDictReference[entry.Key] = null;
                return true;
            }
        }
        return false;
    }

    public bool RemoveItemByKey(int key)
    {
        itemDictReference[key] = null;
        return true;
    }

    public bool CheckIfItemExists(int itemId)
    {
        foreach (KeyValuePair<int, InventoryItem> entry in itemDictReference)
        {
            if (entry.Value.inventoryItemId == itemId)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckIfAllItemsExist(int[] itemsToCheck, out Dictionary<int, int> outItemsMap, out (int, bool)[] outMissingItems)
    {
        List<(int, bool)> missingItems = new List<(int, bool)>();
        Dictionary<int, int> checkCount = new Dictionary<int, int>();
        Dictionary<int, int> checkItems = new Dictionary<int, int>();
        bool doAllItemsExist = true;
        foreach (KeyValuePair<int, InventoryItem> entry in itemDictReference)
        {
            if (checkCount.ContainsKey(entry.Value.inventoryItemId))
            {
                checkCount[entry.Value.inventoryItemId] += 1;
            }
            else
            {
                checkCount.Add(entry.Value.inventoryItemId, 1);
            }
        }
        foreach (int i in itemsToCheck)
        {
            if (checkItems.ContainsKey(i))
            {
                checkItems[i] += 1;
            }
            else
            {
                checkCount.Add(i, 1);
            }
        }
        foreach (KeyValuePair<int, int> entry in checkItems)
        {
            if (!checkCount.ContainsKey(entry.Key) || checkCount[entry.Key] < entry.Value)
            {
                missingItems.Add((entry.Key, false));
                doAllItemsExist = false;
            }
            else
            {
                missingItems.Add((entry.Key, true));
            }
        }
        outItemsMap = checkItems;
        outMissingItems = missingItems.ToArray();
        return doAllItemsExist;

    }

    public bool RemoveItemArrayFromInventory(int[] itemsToRemove)
    {
        if (!CheckIfAllItemsExist(itemsToRemove, out _, out _))
        {
            return false;
        }
        foreach (int i in itemsToRemove)
        {
            RemoveItemById(i);
        }
        return true;
    }

}
