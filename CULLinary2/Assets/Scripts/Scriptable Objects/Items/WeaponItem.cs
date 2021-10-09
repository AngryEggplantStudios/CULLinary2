using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Weapon Skill/Weapon Item")]
public class WeaponItem : WeaponSkillItem
{
    public int[] baseDamage;
    public int attackSpeed;
    public GameObject weaponPrefab;
}
