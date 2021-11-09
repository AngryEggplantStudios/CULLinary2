using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggplantAttack : MonsterAttack
{
    private SpriteRenderer attackSprite;
    private SphereCollider attackCollider;
    private PlayerHealth healthScript;
    private bool canDealDamage;

    private void Awake()
    {
        attackSprite = gameObject.GetComponent<SpriteRenderer>();
        attackCollider = gameObject.GetComponent<SphereCollider>();
        attackSprite.enabled = false;
        attackCollider.enabled = false;
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
        ScreenShake.Instance.Shake(0.4f, 1f, 0.2f, 1f);
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
