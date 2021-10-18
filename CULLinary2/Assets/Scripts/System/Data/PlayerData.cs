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
    public bool[] recipesUnlocked;
    public int[] upgradesArray;
    public int currentDay;
    public MonsterSavedData[] monsterSavedDatas;

    public int[] weaponSkillArray;
    public int currentWeaponHeld = 0;
    public int currentSecondaryHeld = 3;
    public float campfireRegenerationRate;


    public PlayerData()
    {
        inventory = "";
        maxHealth = 200f;
        currentHealth = 200f;
        maxStamina = 100f;
        currentStamina = 100f;
        meleeDamage = 10f;
        currentMoney = 0;
        criticalChance = 0;
        evasionChance = 0;
        consumables = new int[3] { 0, 0, 0 }; //7,8,9 indices
        recipesUnlocked = new bool[36] {
            true, true, true, true, true,
            false, true, true, true, true,
            true, false , false, false, false,
            false, false, false, false , false,
            false, false, false, false , false,
            false, false, false, false , false,
            false, false, false, false , false,
            false, };
        upgradesArray = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        currentDay = 1;
        weaponSkillArray = new int[11] { 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
        currentWeaponHeld = 0;
        currentSecondaryHeld = 3;
        campfireRegenerationRate = 0.5f;
        monsterSavedDatas = new MonsterSavedData[3] {
            new MonsterSavedData(MonsterName.Eggplant, PopulationLevel.Normal),
            new MonsterSavedData(MonsterName.Corn, PopulationLevel.Normal),
            new MonsterSavedData(MonsterName.Potato, PopulationLevel.Normal),
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
