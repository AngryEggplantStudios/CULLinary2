using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : PlayerInteractable
{
    public SpherePlayerCollider spCollider;
    public Transform lootSpawnPoint;
    public GameObject[] spawnOnOpen;
    public GameObject[] destroyOnOpen;
    public ChestLoot[] loot;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    public override SpherePlayerCollider GetCollider()
    {
        return spCollider;
    }

    public override void OnPlayerInteract()
    {
        StartCoroutine(OpenChest());
    }

    public override void OnPlayerLeave()
    {
    }

    IEnumerator OpenChest()
    {
        animator.enabled = true;
        foreach (GameObject obj in spawnOnOpen)
        {
            Instantiate(obj, transform.position, transform.rotation);
        }
        foreach (GameObject obj in destroyOnOpen)
        {
            Destroy(obj);
        }

        foreach (ChestLoot l in loot)
        {
            for (int i = 0; i < l.quantity; i++)
            {
                GameObject obj = Instantiate(l.prefab, lootSpawnPoint.position, lootSpawnPoint.rotation);

                // Push loot in random direction
                float randomRadius = Random.Range(2f, 8f);
                float randomAngle = Random.Range(0f, 2f * Mathf.PI);
                Vector3 direction = new Vector3(
                    randomRadius * Mathf.Cos(randomAngle),
                    0,
                    randomRadius * Mathf.Sin(randomAngle)
                );
                obj.GetComponentInChildren<Rigidbody>().AddForce(direction, ForceMode.Impulse);

                yield return new WaitForSeconds(0.2f);
            }
        }
        yield return null;
    }
}

[System.Serializable]
public struct ChestLoot
{
    public GameObject prefab;
    public int quantity;
}