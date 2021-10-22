using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Inventory/Inventory Item")]
public class InventoryItem : Item
{
    public int inventoryItemId;
    public bool isConsumable;
    public Sprite buffIcon;
    [Header("Instant buffs")]
    public int healHpAmount; // Heals for ___
    public int increaseMaxHpAmount; // Increase max hp for ___
    public int increaseBaseDamageAmount; // Increase base damage
    [Header("Timed Buffs")]
    public int buffDuration;
    public bool isUnlimitedStamina; // Unlimited stamina for ____ s
    public bool isPlayerInvincible; // Unlimited health for ____ s
    public bool buffBaseDamage; // Double base damage for ____ s
    public bool buffMoneyBonus; // Double money earnings for ____ s
    public int evasionBoostAmount; // Boost evasion chance by ____ for ____ s
    public int critBoostAmount; // Boost crit chance by ____ for ____ s
    public BuffType[] buffTypes;

    public string description = "This is a new item";

    public string GetConsumeEffect()
    {
        string result = "";
        bool isStringModified = false;
        foreach (BuffType buffType in buffTypes)
        {
            if (isStringModified)
            {
                result += "\n";
            }
            switch (buffType)
            {
                case BuffType.HEAL_HEALTH:
                    result += "Heals for " + healHpAmount;
                    break;
                case BuffType.INCREASE_MAX_HEALTH:
                    result += "+ " + increaseMaxHpAmount + " Max Health";
                    break;
                case BuffType.INCREASE_BASE_DAMAGE:
                    result += "+ " + increaseBaseDamageAmount + " Base Damage";
                    break;
                case BuffType.BUFF_BASE_DAMAGE:
                    result += "Melee Boost for " + buffDuration + "s";
                    break;
                case BuffType.BUFF_MONEY_BONUS:
                    result += "Double Earnings for " + buffDuration + "s";
                    break;
                case BuffType.BUFF_UNLIMITED_STAMINA:
                    result += "Unlimited Stamina for " + buffDuration + "s";
                    break;
                case BuffType.BUFF_INVINCIBILITY:
                    result += "Invincible for " + buffDuration + "s";
                    break;
                case BuffType.BUFF_EVASION_BOOST:
                    result += "+ " + evasionBoostAmount + " Evasion Chance for " + buffDuration + "s";
                    break;
                case BuffType.BUFF_CRIT_BOOST:
                    result += "+ " + critBoostAmount + " Crit Chance for " + buffDuration + "s";
                    break;
            }
            isStringModified = true;
        }
        return result;
    }
    /*

    // Prints the consume effect nicely as a string
    public string GetConsumeEffect()
    {
        // Mitigate floating-point error
        float epsilon = 0.0001f;

        // All must have same length
        float[] possibleBoosts = { healHpAmount, increaseMaxHpAmount, increaseBaseDamageAmount };
        string[] prefixes = { "Heals for ", "+", "+" };
        string[] suffixes = { "", " max health", " base damage" };

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
    */
}