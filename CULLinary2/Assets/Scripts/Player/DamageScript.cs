using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For primary attack weapons
public class DamageScript : MonoBehaviour
{
    //Weapon damage
    [SerializeField] private GameObject onHitParticle;

    [SerializeField] private float weaponDamage;

    private void Start()
    {
        InstantiateDamageScript();
    }

    public void InstantiateDamageScript()
    {
        if (PlayerManager.instance != null)
        {
            int currentWeaponHeld = PlayerManager.instance.currentWeaponHeld;
            int level = PlayerManager.instance.weaponSkillArray[currentWeaponHeld];
            WeaponSkillItem weaponSkillItem = DatabaseLoader.GetWeaponSkillById(currentWeaponHeld);
            if (weaponSkillItem.GetType() == typeof(WeaponItem))
            {
                weaponDamage = ((WeaponItem)weaponSkillItem).baseDamage[level];
            }
            else
            {
                weaponDamage = 0;
            }
        }
        else
        {
            weaponDamage = 0;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        Monster target = collider.GetComponent<Monster>();
        if (target == null)
        {
            return;
        }
        target.HandleHit(CalculateDamage());

        Instantiate(onHitParticle, transform.position, transform.rotation);
    }

    private int CalculateDamage()
    {
        bool isCritical = PlayerManager.instance != null && PlayerManager.instance.criticalChance > 0 ? Random.Range(0, 100) < PlayerManager.instance.criticalChance : false;
        float baseDamage = PlayerManager.instance != null ? PlayerManager.instance.meleeDamage : 10;
        float additionalDamage = weaponDamage * Random.Range(0.85f, 1.15f);
        float finalDamage = baseDamage + additionalDamage;
        float finalDamageWithCrit = isCritical ? finalDamage * 2 : finalDamage;
        return Mathf.RoundToInt(finalDamageWithCrit);
    }
}
