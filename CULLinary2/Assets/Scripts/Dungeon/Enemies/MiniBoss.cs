using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
    public EnemyName enemyName;

    public void Die()
    {
        // Debug.Log("resetting population to normal for " + enemyName);
        EcosystemManager.ResetPopulationToNormal(enemyName);
    }
}
