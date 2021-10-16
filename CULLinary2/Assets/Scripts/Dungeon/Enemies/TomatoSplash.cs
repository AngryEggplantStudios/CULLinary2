using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomatoSplash : MonoBehaviour
{
    [SerializeField] private float attackDamage;
    private bool canDealDamage;
    private SphereCollider attackCollider;
    PlayerHealth playerHealth = null;

    private void Start()
    {
        attackCollider = gameObject.GetComponent<SphereCollider>();
        StartCoroutine(DelayBeforeAppearing());
        StartCoroutine(DelayBeforeDisappearing());
    }

    private IEnumerator DelayBeforeAppearing()
    {
        yield return new WaitForSeconds(0.3f);
        attackCollider.enabled = true;
    }

    private IEnumerator DelayBeforeDisappearing()
    {
        yield return new WaitForSeconds(15.0f);
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.HandleHit(attackDamage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerHealth tempPlayerHealth = other.GetComponent<PlayerHealth>();
        if (tempPlayerHealth != null)
        {
            playerHealth = null;
        }
    }
}
