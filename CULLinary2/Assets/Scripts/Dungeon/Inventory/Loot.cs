using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Loot : MonoBehaviour
{
    private Rigidbody rb;

    public float delayTimeBetweenPickupTries = 1.0f;
    bool lootable = false;
    float delay = 0.0f;

    void Start()
    {
        LootManager.instance.addLoot(this.gameObject);
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 20, ForceMode.Impulse);
        StartCoroutine(spawningCooldown());
    }

    void Update()
    {
        if (delay > 0.0f)
        {
            delay -= Time.deltaTime;
            delay = Mathf.Max(delay, 0.0f);
        }
    }

    IEnumerator spawningCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        lootable = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!lootable || delay > 0.0f) return;

        PlayerPickup playerPickup = other.GetComponent<PlayerPickup>();
        if (playerPickup != null)
        {
            if (!OnPickup(playerPickup))
            {
                delay = delayTimeBetweenPickupTries;
            }
        }
    }

    protected virtual bool OnPickup(PlayerPickup playerPickup)
    {
        return false;
    }
}
