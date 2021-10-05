using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMeleeAttack : MonoBehaviour
{
    private SpriteRenderer attackSprite;
    private SphereCollider attackCollider;
    private PlayerHealth healthScript;
    [SerializeField] private float attackDamage;

    private void Awake()
    {

        attackSprite = gameObject.GetComponent<SpriteRenderer>();
        attackCollider = gameObject.GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
        if (healthScript != null)
        {
            bool hitSuccess = healthScript.HandleHit(attackDamage);
            /*
            if (hitSuccess)
            {
                healthScript.KnockbackPlayer(transform.position);
            }
            */
        }

    }

    public void enableCollider(bool toEnable)
    {
        attackCollider.enabled = toEnable;
        //Debug.Log(attackCollider.enabled);
        if (!toEnable)
        {
            healthScript = null;
        }
    }
    public void enableSprite(bool toEnable)
    {
        attackSprite.enabled = toEnable;

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
