using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageScript : MonoBehaviour
{
    //Weapon damage
    [SerializeField] private float weaponDamage = 0;

    private void OnTriggerEnter(Collider collider)
    {
        Monster target = collider.GetComponent<Monster>();
        if (target == null)
        {
            return;
        }
        target.HandleHit(CalculateDamage());
    }

    private int CalculateDamage()
    {
        float baseDamage = PlayerManager.instance ? PlayerManager.instance.meleeDamage : 10;
        float additionalDamage = weaponDamage * Random.Range(0.8f, 1.2f);
        return Mathf.RoundToInt(baseDamage + additionalDamage);
    }
}
