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

    public PlayerData()
    {
        inventory = "";
        maxHealth = 200f;
        currentHealth = 200f;
        maxStamina = 100f;
        currentStamina = 100f;
        meleeDamage = 10f;
        currentMoney = 10000;
        criticalChance = 0;
        evasionChance = 0;
        consumables = new int[3] { 0, 0, 0 }; //7,8,9 indices
        recipesUnlocked = new bool[3] { true, true, true };
        upgradesArray = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        currentDay = 1;
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
