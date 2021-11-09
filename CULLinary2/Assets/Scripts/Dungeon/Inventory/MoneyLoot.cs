using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyLoot : Loot
{
    [SerializeField] private int moneyAmount;

    protected override bool OnPickup(PlayerPickup playerPickup)
    {
        int moneyAmountReal = moneyAmount;
        if (OrdersManager.instance != null && OrdersManager.instance.AreEarningsDoubled())
        {
            moneyAmountReal *= 2;
        }
        PlayerManager.instance.currentMoney += moneyAmountReal;
        OrdersManager.AddToMoneyEarnedToday(moneyAmountReal);
        playerPickup.PickUpMoney(moneyAmountReal);

        LootManager.instance.removeLoot(this.gameObject);
        Destroy(gameObject);
        return true;
    }
}
