using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : SingletonGeneric<PlayerManager>
{
    public float currentHealth = 200f;
    public float maxHealth = 200f;
    public float currentStamina = 100f;
    public float maxStamina = 100f;
    public float meleeDamage = 20f;
    public int currentInventoryLimit = 25;
    public int inventoryLimit = 25;
    public bool[] recipesUnlocked = new bool[3] { true, true, true }; //use index
    public Dictionary<int, InventoryItem> itemDict = new Dictionary<int, InventoryItem>();

    // Private variables
    private static PlayerData playerData = new PlayerData();

    public void SaveData(List<InventoryItem> itemList)
    {
        if (playerData == null)
        {
            playerData = new PlayerData();
        }
        // Save Player Stats
        playerData.currentHealth = currentHealth;
        playerData.maxHealth = maxHealth;
        playerData.currentStamina = currentStamina;
        playerData.maxStamina = maxStamina;
        playerData.recipesUnlocked = recipesUnlocked;
        playerData.meleeDamage = meleeDamage;
        playerData.currentInventoryLimit = currentInventoryLimit;
        playerData.inventoryLimit = inventoryLimit;

        playerData.inventory = SerializeInventory();
        SaveSystem.SaveData(playerData);
    }

    public void LoadInventory()
    {
        SetupItemDict();
        InventoryItemData[] parsedInventoryArr = JsonArrayParser.FromJson<InventoryItemData>(playerData.inventory);
        foreach (InventoryItemData item in parsedInventoryArr)
        {
            itemDict[item.slotId] = DatabaseLoader.GetItemById(item.itemId);
        }
    }

    public void LoadData()
    {
        playerData = SaveSystem.LoadData();
        currentHealth = playerData.currentHealth;
        maxHealth = playerData.maxHealth;
        currentStamina = playerData.currentStamina;
        maxStamina = playerData.maxStamina;
        recipesUnlocked = playerData.recipesUnlocked;
        meleeDamage = playerData.meleeDamage;
        inventoryLimit = playerData.inventoryLimit;
        currentInventoryLimit = playerData.currentInventoryLimit;
    }

    private void SetupItemDict()
    {
        itemDict.Clear();
        for (int i = 0; i < inventoryLimit; i++)
        {
            itemDict.Add(i, null);
        }
    }

    public PlayerData CreateBlankData()
    {
        playerData = new PlayerData();
        SetupManager();
        return playerData;
    }

    public void SetupManager()
    {
        SetupItemDict();
        currentHealth = playerData.currentHealth;
        maxHealth = playerData.maxHealth;
        currentStamina = playerData.currentStamina;
        maxStamina = playerData.maxStamina;
        meleeDamage = playerData.meleeDamage;
        recipesUnlocked = playerData.recipesUnlocked;
        currentInventoryLimit = playerData.currentInventoryLimit;
        inventoryLimit = playerData.inventoryLimit;
    }

    public string SerializeInventory()
    {
        InventoryItemData[] items = new InventoryItemData[inventoryLimit];
        for (int i = 0; i < inventoryLimit; i++)
        {
            items[i] = new InventoryItemData(i, itemDict[i].inventoryItemId);
        }
        return JsonArrayParser.ToJson(items, true);
    }

}
