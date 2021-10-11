using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : SingletonGeneric<PlayerManager>
{
    public float currentHealth = 200f;
    public float maxHealth = 200f;
    public float currentStamina = 100f;
    public float maxStamina = 100f;
    public float meleeDamage = 10f;
    public int criticalChance = 0;
    public int evasionChance = 0;
    public int[] consumables = new int[3] { 0, 0, 0 };
    public bool[] recipesUnlocked = new bool[3] { true, true, true }; //use index
    public int[] upgradesArray = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public List<InventoryItem> itemList = new List<InventoryItem>();
    public int currentMoney;
    public int currentDay;
    public int[] weaponSkillArray = new int[11] { 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
    public int currentWeaponHeld = 0;
    public int currentSecondaryHeld = 3;
    public Dictionary<MonsterName, PopulationLevel> monsterDict = new Dictionary<MonsterName, PopulationLevel>{
        {MonsterName.Corn, PopulationLevel.Normal},
        {MonsterName.Potato, PopulationLevel.Normal},
        {MonsterName.Eggplant, PopulationLevel.Normal},
    };

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
        playerData.inventory = SerializeInventory(itemList);
        playerData.recipesUnlocked = recipesUnlocked;
        playerData.upgradesArray = upgradesArray;
        playerData.meleeDamage = meleeDamage;
        playerData.currentMoney = currentMoney;
        playerData.evasionChance = evasionChance;
        playerData.criticalChance = criticalChance;
        playerData.consumables = consumables;
        playerData.currentDay = currentDay;
        playerData.monsterSavedDatas = SaveMonsters();
        playerData.weaponSkillArray = weaponSkillArray;
        playerData.currentWeaponHeld = currentWeaponHeld;
        playerData.currentSecondaryHeld = currentSecondaryHeld;
        SaveSystem.SaveData(playerData);
    }

    public void LoadMonsters()
    {
        monsterDict = new Dictionary<MonsterName, PopulationLevel>();
        foreach (MonsterSavedData md in playerData.monsterSavedDatas)
        {
            monsterDict.Add(md.monsterName, md.populationLevel);
        }
    }

    public void SetPopulationLevelByMonsterName(MonsterName monsterName, PopulationLevel populationLevel)
    {
        monsterDict[monsterName] = populationLevel;
    }

    public PopulationLevel GetPopulationLevelByMonsterName(MonsterName monsterName)
    {
        return monsterDict[monsterName];
    }

    public MonsterSavedData[] SaveMonsters()
    {
        MonsterSavedData[] result = new MonsterSavedData[monsterDict.Count];
        int i = 0;
        foreach (KeyValuePair<MonsterName, PopulationLevel> entry in monsterDict)
        {
            result[i] = new MonsterSavedData(entry.Key, entry.Value);
            i++;
        }
        return result;
    }

    public void LoadInventory()
    {
        InventoryItemData[] inventory = JsonArrayParser.FromJson<InventoryItemData>(playerData.inventory);
        itemList.Clear();
        foreach (InventoryItemData item in inventory)
        {
            for (int i = 0; i < item.count; i++)
            {
                itemList.Add(DatabaseLoader.GetItemById(item.id));
            }
        }
    }

    public void LoadData()
    {
        playerData = SaveSystem.LoadData();
        SetupItems();
    }

    public PlayerData CreateBlankData()
    {
        playerData = new PlayerData();
        SetupManager();
        return playerData;
    }

    public void SetupItems()
    {
        currentHealth = playerData.currentHealth;
        maxHealth = playerData.maxHealth;
        currentStamina = playerData.currentStamina;
        maxStamina = playerData.maxStamina;
        meleeDamage = playerData.meleeDamage;
        recipesUnlocked = playerData.recipesUnlocked;
        upgradesArray = playerData.upgradesArray;
        currentMoney = playerData.currentMoney;
        criticalChance = playerData.criticalChance;
        evasionChance = playerData.evasionChance;
        consumables = playerData.consumables;
        currentDay = playerData.currentDay;
        weaponSkillArray = playerData.weaponSkillArray;
        currentWeaponHeld = playerData.currentWeaponHeld;
        currentSecondaryHeld = playerData.currentSecondaryHeld;
        LoadMonsters();
    }

    public void SetupManager()
    {
        itemList.Clear();
        SetupItems();
    }

    private static string SerializeInventory(List<InventoryItem> itemList)
    {
        Dictionary<int, int> inventory = new Dictionary<int, int>();

        foreach (InventoryItem item in itemList)
        {
            if (inventory.ContainsKey(item.inventoryItemId))
            {
                inventory[item.inventoryItemId] += 1;
            }
            else
            {
                inventory.Add(item.inventoryItemId, 1);
            }
        }
        InventoryItemData[] items = new InventoryItemData[inventory.Count];
        int i = 0;
        foreach (var item in inventory)
        {
            InventoryItemData gameItem = new InventoryItemData(item.Key, item.Value);
            items[i] = gameItem;
            i++;
        }
        return JsonArrayParser.ToJson(items, true);
    }

}
