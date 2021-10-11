using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Skill Database", menuName = "Database/Weapon Skill Database")]
public class WeaponSkillDatabase : ScriptableObject
{
    public List<WeaponSkillItem> allItems;
}
