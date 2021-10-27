using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : SingletonGeneric<PlayerManager>
{
    [Header("Health & Stamina")]
    public float currentHealth = 200f;
    public float maxHealth = 200f;
    public float currentStamina = 200f;
    public float maxStamina = 200f;
    [Header("Combat / Stats / Buffs")]
    public float meleeDamage = 10f;
    public int criticalChance = 0;
    public int evasionChance = 0;
    public int currentWeaponHeld = 0;
    public int currentSecondaryHeld = 4;
    public bool isMeleeDamageDoubled = false;
    public int evasionBonus = 0;
    public int criticalBonus = 0;
    public float speedMultiplier = 1f;

    [Header("Upgrades")]
    public List<int> unlockedRecipesList = new List<int> { 0, 4, 6, 10, 32 };
    public int[] upgradesArray = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public int[] weaponSkillArray = new int[11] { 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 };

    [Header("Inventory")]
    public List<InventoryItem> itemList = new List<InventoryItem>();
    public int healthPill;
    public int staminaPill;
    public int potion;
    public int pfizerShot;
    public int modernaShot;
    public int currentMoney;

    [Header("Others")]
    public int currentDay;
    public int currentNewspaperIssue = 1;
    public Dictionary<MonsterName, PopulationLevel> monsterDict = new Dictionary<MonsterName, PopulationLevel>{
        {MonsterName.Corn, PopulationLevel.Normal},
        {MonsterName.Potato, PopulationLevel.Normal},
        {MonsterName.Eggplant, PopulationLevel.Normal},
        {MonsterName.Bread, PopulationLevel.Normal},
        {MonsterName.Tomato, PopulationLevel.Normal},
        {MonsterName.Meat, PopulationLevel.Normal},
        {MonsterName.Cheese, PopulationLevel.Normal},
        {MonsterName.Mushroom, PopulationLevel.Normal},
        {MonsterName.DaddyCorn, PopulationLevel.Normal},
        {MonsterName.DaddyEggplant, PopulationLevel.Normal},
        {MonsterName.DaddyPotato, PopulationLevel.Normal},
    };

    [Header("Health Regenerated per Game Minute at Campfire")]
    public float campfireRegenerationRate = 0.5f;

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
        playerData.unlockedRecipes = unlockedRecipesList.ToArray();
        playerData.upgradesArray = upgradesArray;
        playerData.meleeDamage = meleeDamage;
        playerData.currentMoney = currentMoney;
        playerData.evasionChance = evasionChance;
        playerData.criticalChance = criticalChance;
        playerData.currentDay = currentDay;
        playerData.monsterSavedDatas = SaveMonsters();
        playerData.weaponSkillArray = weaponSkillArray;
        playerData.currentWeaponHeld = currentWeaponHeld;
        playerData.currentSecondaryHeld = currentSecondaryHeld;
        playerData.campfireRegenerationRate = campfireRegenerationRate;
        playerData.healthPill = healthPill;
        playerData.staminaPill = staminaPill;
        playerData.potion = potion;
        playerData.pfizerShot = pfizerShot;
        playerData.modernaShot = modernaShot;
        playerData.currentNewspaperIssue = currentNewspaperIssue;
        SaveSystem.SaveData(playerData);
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
        unlockedRecipesList.Clear();
        unlockedRecipesList.AddRange(playerData.unlockedRecipes);
        currentHealth = playerData.currentHealth;
        maxHealth = playerData.maxHealth;
        currentStamina = playerData.currentStamina;
        maxStamina = playerData.maxStamina;
        meleeDamage = playerData.meleeDamage;
        upgradesArray = playerData.upgradesArray;
        currentMoney = playerData.currentMoney;
        criticalChance = playerData.criticalChance;
        evasionChance = playerData.evasionChance;
        currentDay = playerData.currentDay;
        weaponSkillArray = playerData.weaponSkillArray;
        currentWeaponHeld = playerData.currentWeaponHeld;
        currentSecondaryHeld = playerData.currentSecondaryHeld;
        campfireRegenerationRate = playerData.campfireRegenerationRate;
        healthPill = playerData.healthPill;
        staminaPill = playerData.staminaPill;
        potion = playerData.potion;
        pfizerShot = playerData.pfizerShot;
        modernaShot = playerData.modernaShot;
        currentNewspaperIssue = playerData.currentNewspaperIssue;
        LoadMonsters();
    }

    public void SetupManager()
    {
        itemList.Clear();
        SetupItems();
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

    public IEnumerator DoubleMeleeDamage(float duration)
    {
        isMeleeDamageDoubled = true;
        yield return new WaitForSeconds(duration);
        isMeleeDamageDoubled = false;
    }

    public IEnumerator ToggleEvasionBoost(int boost, float duration)
    {
        evasionBonus = boost;
        yield return new WaitForSeconds(duration);
        evasionBonus = 0;
    }

    public IEnumerator ToggleCritBoost(int boost, float duration)
    {
        criticalBonus = boost;
        yield return new WaitForSeconds(duration);
        criticalBonus = 0;
    }

    public void ClearBuffs()
    {
        criticalBonus = 0;
        evasionBonus = 0;
        isMeleeDamageDoubled = false;
    }

}
