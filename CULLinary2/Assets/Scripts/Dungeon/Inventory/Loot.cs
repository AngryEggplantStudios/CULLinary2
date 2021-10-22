using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Loot : MonoBehaviour
{
    private Rigidbody rb;

    bool lootable = false;

    void Start()
    {
        LootManager.instance.addLoot(this.gameObject);
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 20, ForceMode.Impulse);
        StartCoroutine(spawningCooldown());
    }

    IEnumerator spawningCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        lootable = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!lootable) return;

        PlayerPickup playerPickup = other.GetComponent<PlayerPickup>();
        if (playerPickup != null)
        {
            OnPickup(playerPickup);
        }
    }

    protected virtual void OnPickup(PlayerPickup playerPickup)
    {

    }
}
