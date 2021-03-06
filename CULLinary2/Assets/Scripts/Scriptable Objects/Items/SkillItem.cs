using UnityEngine;


[CreateAssetMenu(fileName = "New Skill Item", menuName = "Weapon Skill/Skill Item")]
public class SkillItem : WeaponSkillItem
{
    public int[] blockPercentage;
    public int[] staminaCost;
    public int[] skillDuration;
    public int[] attackDamage;
    public GameObject skillPrefab;

    public string GetDescription(int level)
    {
        return "Damage: " + attackDamage[level] + "\n" + "Stamina Cost: " + staminaCost[level];
    }

}
