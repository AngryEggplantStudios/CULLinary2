using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string inventory;
    public float maxHealth;
    public float currentHealth;
    public float maxStamina;
    public float currentStamina;
    public float meleeDamage;
    public int currentMoney;
    public int criticalChance;
    public int evasionChance;
    public int[] consumables;
    public int healthPill;
    public int staminaPill;
    public int potion;
    public int pfizerShot;
    public int modernaShot;
    public int[] upgradesArray;
    public int currentDay;
    public int currentNewspaperIssue = 1;
    public int[] unlockedRecipes;
    public MonsterName[] unlockedMonsters;
    public MonsterSavedData[] monsterSavedDatas;

    public int[] weaponSkillArray;
    public int currentWeaponHeld = 0;
    public int currentSecondaryHeld = 3;
    public float campfireRegenerationRate;
    public bool isTruckUnlocked = false;

    //Stats
    public int moneyEarned = 0; //from customers
    public int ordersFulfilled = 0;
    public int noOfDeaths = 0;
    public float gameTime = 0f;
    public float bossTime = 0f;
    public int enemiesCulled = 0;



    public PlayerData()
    {
        inventory = "";
        maxHealth = 200f;
        currentHealth = 200f;
        maxStamina = 200f;
        currentStamina = 200f;
        meleeDamage = 10f;
        currentMoney = 0;
        criticalChance = 0;
        evasionChance = 0;
        consumables = new int[3] { 0, 0, 0 }; //7,8,9 indices
        unlockedRecipes = new int[3] { 0, 1, 3 };
        upgradesArray = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        currentDay = 1;
        weaponSkillArray = new int[11] { 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 };
        currentWeaponHeld = 0;
        currentSecondaryHeld = 4;
        campfireRegenerationRate = 0.5f;
        healthPill = 0;
        staminaPill = 0;
        potion = 0;
        pfizerShot = 0;
        modernaShot = 0;
        currentNewspaperIssue = 1;
        isTruckUnlocked = false;
        moneyEarned = 0;
        ordersFulfilled = 0;
        noOfDeaths = 0;
        gameTime = 0f;
        bossTime = 0f;
        enemiesCulled = 0;
        unlockedMonsters = new MonsterName[3] { MonsterName.Bread, MonsterName.DaddyPotato, MonsterName.Potato };
        monsterSavedDatas = new MonsterSavedData[12] {
            new MonsterSavedData(MonsterName.Bread, PopulationLevel.Normal),
            new MonsterSavedData(MonsterName.Potato, PopulationLevel.Normal),
            new MonsterSavedData(MonsterName.DaddyPotato, PopulationLevel.Rare),
            new MonsterSavedData(MonsterName.Corn, PopulationLevel.Normal),
            new MonsterSavedData(MonsterName.DaddyCorn, PopulationLevel.Rare),
            new MonsterSavedData(MonsterName.Eggplant, PopulationLevel.Normal),
            new MonsterSavedData(MonsterName.DaddyEggplant, PopulationLevel.Rare),
            new MonsterSavedData(MonsterName.Tomato, PopulationLevel.Normal),
            new MonsterSavedData(MonsterName.Ham, PopulationLevel.Normal),
            new MonsterSavedData(MonsterName.Mushroom, PopulationLevel.Normal),
            new MonsterSavedData(MonsterName.Cheese, PopulationLevel.Normal),
            new MonsterSavedData(MonsterName.Clown, PopulationLevel.Rare)
        };
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string jsonData)
    {
        try
        {
            JsonUtility.FromJsonOverwrite(jsonData, this);
        }
        catch
        {
            Debug.Log("No save file...");
        }
    }
}
