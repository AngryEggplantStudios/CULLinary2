using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionLoot : Loot
{
    protected override void OnPickup(PlayerPickup playerPickup)
    {
        int randomPotion = Random.Range(1, 6);
        bool isFull = false;
        //Hmm... if someone can help me find a better way of doing this
        if (randomPotion == 1)
        {
            if (PlayerManager.instance.healthPill < 99)
            {
                PlayerManager.instance.healthPill++;
            }
            else
            {
                randomPotion = 2;
            }
        }

        if (randomPotion == 2)
        {
            if (PlayerManager.instance.staminaPill < 99)
            {
                PlayerManager.instance.staminaPill++;
            }
            else
            {
                randomPotion = 3;
            }
        }

        if (randomPotion == 3)
        {
            if (PlayerManager.instance.potion < 99)
            {
                PlayerManager.instance.potion++;
            }
            else
            {
                randomPotion = 4;
            }
        }

        if (randomPotion == 4)
        {
            if (PlayerManager.instance.pfizerShot < 99)
            {
                PlayerManager.instance.pfizerShot++;
            }
            else
            {
                randomPotion = 5;
            }
        }

        if (randomPotion == 5)
        {
            if (PlayerManager.instance.modernaShot < 99)
            {
                PlayerManager.instance.modernaShot++;
            }
            else
            {
                isFull = true;
            }
        }
        if (isFull)
        {
            return;
        }
        playerPickup.PickUpPotion(randomPotion);
        LootManager.instance.removeLoot(this.gameObject);
        Destroy(gameObject);
    }
}
