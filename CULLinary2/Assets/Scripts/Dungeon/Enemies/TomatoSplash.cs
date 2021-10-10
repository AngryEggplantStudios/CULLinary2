using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomatoSplash : MonoBehaviour
{
    [SerializeField] private float attackDamage;
    private bool canDealDamage;

    private void Start()
    {
        StartCoroutine(DelayBeforeDisappearing());
    }

    private IEnumerator DelayBeforeDisappearing()
	{
        yield return new WaitForSeconds(15.0f);
        Destroy(gameObject);
	}

    private void OnTriggerStay(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.HandleHit(attackDamage);
        }
    }
}
