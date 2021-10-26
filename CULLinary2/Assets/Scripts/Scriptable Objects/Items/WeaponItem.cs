using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Weapon Skill/Weapon Item")]
public class WeaponItem : WeaponSkillItem
{
    public int[] baseDamage;
    public float[] attackSpeed;
    public GameObject weaponPrefab;

    public string GetLevelDescription(int level)
    {
        string damageDesc = "Damage: " + baseDamage[level] + "DMG";
        string attackSpeedDesc = "Attack Speed: " + string.Format("{0:#.00}", attackSpeed[level]);
        return damageDesc + "\n" + attackSpeedDesc;
    }
}
