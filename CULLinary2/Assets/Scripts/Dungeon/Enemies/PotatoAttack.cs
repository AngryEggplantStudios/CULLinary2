using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotatoAttack : MonsterAttack
{
    private SphereCollider attackCollider;
    private bool canDealDamage;


    private void Awake()
    {
        attackCollider = gameObject.GetComponent<SphereCollider>();
        canDealDamage = false;
    }

    public override void attackPlayerStart()
    {
        attackCollider.enabled = true;
    }

    public override void attackPlayerDealDamage()
    {
        canDealDamage = true;
    }

    public override void attackPlayerEnd()
    {
        attackCollider.enabled = false;
        canDealDamage = false;
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (canDealDamage && playerHealth != null)
        {
            playerHealth.HandleHit(attackDamage);
        }
    }

}
