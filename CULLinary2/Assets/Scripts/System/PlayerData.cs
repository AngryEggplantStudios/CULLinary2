using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string inventory = "";
    public float maxHealth = 200f;
    public float currentHealth = 200f;
    public float maxStamina = 200f;
    public float currentStamina = 200f;
    public float meleeDamage = 20f;
    public bool[] recipesUnlocked = new bool[3] { true, true, true };


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
