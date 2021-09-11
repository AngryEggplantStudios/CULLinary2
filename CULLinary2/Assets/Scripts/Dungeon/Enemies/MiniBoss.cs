using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
    public MonsterName monsterName;

    public void Die()
    {
        // Debug.Log("resetting population to normal for " + monsterName);
        EcosystemManager.ResetPopulationToNormal(monsterName);
    }
}
