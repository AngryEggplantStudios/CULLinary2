using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSkillManager : SingletonGeneric<WeaponSkillManager>
{
    [SerializeField] private PlayerSecondaryAttack playerSecondaryAttack;
    [SerializeField] private PlayerSlash playerSlash;
    public void InstantiateWeaponSkill()
    {
        playerSecondaryAttack.ChangeSecondaryAttack(PlayerManager.instance != null ? PlayerManager.instance.currentSecondaryHeld : 3);
        playerSlash.ChangeWeapon(PlayerManager.instance != null ? PlayerManager.instance.currentWeaponHeld : 0);
    }

}
