using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
    [SerializeField] private int health = 200;
    [SerializeField] private float distanceTriggered = 45f;
    private MonsterName monsterName;

    void Start()
    {
        MonsterScript monsterScript = gameObject.GetComponent<MonsterScript>();
        if (monsterScript)
        {
            monsterName = monsterScript.GetMonsterName();
            monsterScript.SetMiniBossValues(health, distanceTriggered);
        }
    }

    public void Die()
    {
        Debug.Log("resetting population to normal for " + monsterName);
        EcosystemManager.ResetPopulationToNormal(monsterName);
    }
}
