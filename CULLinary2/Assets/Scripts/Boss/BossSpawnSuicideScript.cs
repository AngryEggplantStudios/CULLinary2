using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnSuicideScript : MonoBehaviour
{
    // Need this else boss is destroyed before all mobs get destroyed
    public void destroyAllMobs()
	{
        int children = transform.childCount;
        for (int i = 0; i < children; i++)
        {
            BossSpawnMinionScript minion = transform.GetChild(i).GetComponent<BossSpawnMinionScript>();
            if (minion != null)
			{
                minion.DieAnimation();
            }
            else
			{
                BossEnemyScript spawn = transform.GetChild(i).GetComponent<BossEnemyScript>();
                spawn.DieAnimation();
            }
        }
    }
}
