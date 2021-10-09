using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryAttackDamageScript : MonoBehaviour
{

    [SerializeField] private float attackDamage = 50f;

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
        bool isCritical = PlayerManager.instance != null && PlayerManager.instance.criticalChance > 0 ? Random.Range(0, 100) < PlayerManager.instance.criticalChance : false;
        float finalDamage = attackDamage * Random.Range(0.85f, 1.15f);
        float finalDamageWithCrit = isCritical ? finalDamage * 2 : finalDamage;
        return Mathf.RoundToInt(finalDamageWithCrit);
    }
}
