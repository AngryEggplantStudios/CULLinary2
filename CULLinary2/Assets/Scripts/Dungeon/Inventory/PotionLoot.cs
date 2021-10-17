using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionLoot : MonoBehaviour
{
    [SerializeField] private int consumablesIndex = 0;
    private void OnTriggerEnter(Collider other)
    {
        PlayerPickup playerPickup = other.GetComponent<PlayerPickup>();
        if (playerPickup != null)
        {
            PlayerManager.instance.consumables[consumablesIndex]++;
            playerPickup.PickUpPotion(consumablesIndex);
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
