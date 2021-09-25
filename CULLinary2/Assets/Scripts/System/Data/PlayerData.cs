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
    public bool[] recipesUnlocked;

    public PlayerData()
    {
        inventory = "";
        maxHealth = 200f;
        currentHealth = 200f;
        maxStamina = 200f;
        currentStamina = 200f;
        meleeDamage = 20f;
        currentMoney = 100;
        recipesUnlocked = new bool[3] { true, true, true };
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
