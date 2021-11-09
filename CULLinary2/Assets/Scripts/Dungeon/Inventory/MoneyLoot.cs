using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyLoot : Loot
{
    [SerializeField] private int moneyAmount;

    protected override bool OnPickup(PlayerPickup playerPickup)
    {
        PlayerManager.instance.currentMoney += moneyAmount;
        OrdersManager.AddToMoneyEarnedToday(moneyAmount);
        playerPickup.PickUpMoney(moneyAmount);

        LootManager.instance.removeLoot(this.gameObject);
        Destroy(gameObject);
        return true;
    }
}
