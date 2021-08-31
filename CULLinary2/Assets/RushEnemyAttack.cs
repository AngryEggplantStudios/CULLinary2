using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushEnemyAttack : EnemyAttack
{
    private SphereCollider attackCollider;
    private PlayerHealth healthScript;
    private bool canDealDamage;
    private bool canAttack;

    private void Awake()
    {

        attackCollider = gameObject.GetComponent<SphereCollider>();
        canDealDamage = false;
        canAttack = false;
    }

    public bool attackStarted()
	{
        return canAttack;
	}

    public bool getCanDealDamage()
    {
        return this.canDealDamage;
    }

    public override void attackPlayerStart()
    {
        attackCollider.enabled = true;
        canAttack = true;
    }

    public override void attackPlayerDealDamage()
    {
        canDealDamage = true;
    }


    public override void attackPlayerEnd()
    {
        attackCollider.enabled = false;
        canDealDamage = false;
        canAttack = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (canDealDamage)
        {
            if (healthScript != null)
            {
                bool hitSuccess = healthScript.HandleHit(attackDamage);
                if (hitSuccess)
                {
                    healthScript.KnockbackPlayer(transform.position);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth target = other.GetComponent<PlayerHealth>();
        if (target != null)
        {
            healthScript = target;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerHealth target = other.GetComponent<PlayerHealth>();
        if (target != null)
        {
            healthScript = null;
        }
    }

}
