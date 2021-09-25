using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string inventory;
    public int inventoryLimit;
    public int currentInventoryLimit;
    public float maxHealth;
    public float currentHealth;
    public float maxStamina;
    public float currentStamina;
    public float meleeDamage;
    public bool[] recipesUnlocked;

    public PlayerData()
    {
        inventory = "";
        inventoryLimit = 25;
        currentInventoryLimit = 25;
        maxHealth = 200f;
        currentHealth = 200f;
        maxStamina = 200f;
        currentStamina = 200f;
        meleeDamage = 20f;
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
