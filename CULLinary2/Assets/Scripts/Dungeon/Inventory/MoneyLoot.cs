using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyLoot : MonoBehaviour
{
    [SerializeField] private int moneyAmount;

    private void OnTriggerEnter(Collider other)
    {
        PlayerPickup playerPickup = other.GetComponent<PlayerPickup>();
        if (playerPickup != null)
        {
            PlayerManager.instance.currentMoney += moneyAmount;
            playerPickup.PickUpMoney(moneyAmount);
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
