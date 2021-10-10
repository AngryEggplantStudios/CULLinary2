using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Inventory/Inventory Item")]
public class InventoryItem : Item
{
    public int inventoryItemId;
    public bool isConsumable;
    public float healHpAmount;
    public float increaseMaxHpAmount;
    public float increaseBaseDamageAmount;
    public string description = "This is a new item";

    // Prints the consume effect nicely as a string
    public string GetConsumeEffect()
    {
        // Mitigate floating-point error
        float epsilon = 0.0001f;

        // All must have same length
        float[] possibleBoosts = {healHpAmount, increaseMaxHpAmount, increaseBaseDamageAmount};
        string[] prefixes = {"Heals for ", "+", "+"};
        string[] suffixes = {"", " max health", " base damage"};

        string effectString = "None";
        bool stringModified = false;
        for (int i = 0; i < possibleBoosts.Length; i++)
        {
            float boost = possibleBoosts[i];
            if (boost > epsilon)
            {
                string boostString = prefixes[i] + boost + suffixes[i];
                if (stringModified)
                {
                    effectString += "\n" + boostString;
                }
                else
                {
                    effectString = boostString;
                }
                stringModified = true;
            }
        }
        return effectString;
    }
}