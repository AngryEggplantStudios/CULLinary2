using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggplantAttack : EnemyAttack
{
    private SpriteRenderer attackSprite;
    private SphereCollider attackCollider;
    private PlayerHealth healthScript;
    private bool canDealDamage;

    private void Awake()
    {

        attackSprite = gameObject.GetComponent<SpriteRenderer>();
        attackCollider = gameObject.GetComponent<SphereCollider>();
        canDealDamage = false;
    }

    public override void attackPlayerStart()
    {
        attackSprite.enabled = true;
        attackCollider.enabled = true;
    }

    public override void attackPlayerDealDamage()
    {
        canDealDamage = true;
    }

    public override void attackPlayerEnd()
    {
        attackSprite.enabled = false;
        //Destroy(selectionCircleActual.gameObject);
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
