using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDamageScript : MonoBehaviour
{

    [SerializeField] private float attackDamage = 50f;

    private void OnParticleCollision(GameObject other)
    {
        Monster target = other.GetComponent<Monster>();
        if (target == null)
        {
            return;
        }
        target.HandleHit(CalculateDamage());
    }


    private void Awake()
    {
        InstantiateDamageScript();
    }

    public void InstantiateDamageScript()
    {
        if (PlayerManager.instance != null)
        {
            int currentSecondaryHeld = PlayerManager.instance.currentSecondaryHeld;
            int level = PlayerManager.instance.weaponSkillArray[currentSecondaryHeld];
            WeaponSkillItem weaponSkillItem = DatabaseLoader.GetWeaponSkillById(currentSecondaryHeld);
            if (weaponSkillItem.GetType() == typeof(SkillItem))
            {
                attackDamage = ((SkillItem)weaponSkillItem).attackDamage[level];
            }
            else
            {
                attackDamage = 50f;
            }
        }
        else
        {
            attackDamage = 50f;
        }
    }

    private int CalculateDamage()
    {
        InstantiateDamageScript();
        int totalCritChance = PlayerManager.instance != null ? PlayerManager.instance.criticalBonus + PlayerManager.instance.criticalChance : 0;
        bool isCritical = totalCritChance > 0 ? Random.Range(0, 100) < totalCritChance : false;
        float finalDamage = attackDamage * Random.Range(0.85f, 1.15f);
        float finalDamageWithCrit = isCritical ? finalDamage * 2 : finalDamage;
        return Mathf.RoundToInt(finalDamageWithCrit);
    }
}
