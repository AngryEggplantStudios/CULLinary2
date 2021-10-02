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
    public bool[] recipesUnlocked;
    public int[] upgradesArray;

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
        recipesUnlocked = new bool[3] { true, true, true };
        upgradesArray = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
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
