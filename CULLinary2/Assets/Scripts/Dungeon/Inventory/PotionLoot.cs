using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionLoot : Loot
{

    protected override void OnPickup(PlayerPickup playerPickup)
    {
        int randomPotion = Random.Range(1, 6);
        switch (randomPotion)
        {
            case 1:
                PlayerManager.instance.healthPill++;
                break;
            case 2:
                PlayerManager.instance.staminaPill++;
                break;
            case 3:
                PlayerManager.instance.potion++;
                break;
            case 4:
                PlayerManager.instance.pfizerShot++;
                break;
            case 5:
                PlayerManager.instance.modernaShot++;
                break;
        }
        playerPickup.PickUpPotion(randomPotion);
        LootManager.instance.removeLoot(this.gameObject);
        Destroy(gameObject);
    }
}
