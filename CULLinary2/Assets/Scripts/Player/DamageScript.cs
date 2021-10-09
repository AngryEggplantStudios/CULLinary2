using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For primary attack weapons
public class DamageScript : MonoBehaviour
{
    //Weapon damage
    [SerializeField] private float weaponDamage = 0;
    
    [SerializeField] private GameObject onHitParticle;

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
