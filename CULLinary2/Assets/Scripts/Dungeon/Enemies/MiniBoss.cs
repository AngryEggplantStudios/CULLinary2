using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
    private MonsterName monsterName;

    void Start()
    {
        MonsterScript monsterScript = gameObject.GetComponent<MonsterScript>();
        if (monsterScript)
        {
            monsterName = monsterScript.GetMonsterName();
        }
    }

    public void Die()
    {
        Debug.Log("resetting population to normal for " + monsterName);
        EcosystemManager.ResetPopulationToNormal(monsterName);
    }
}
