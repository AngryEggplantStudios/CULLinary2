using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private float distanceTriggered;
    [SerializeField] private float stopChase;
    [SerializeField] private float damageMultiplier = 1.5f;
    private MonsterName monsterName;

    void Start()
    {
        MonsterScript monsterScript = gameObject.GetComponent<MonsterScript>();
        if (monsterScript)
        {
            monsterName = monsterScript.GetMonsterName();
            monsterScript.SetMiniBossValues(health, distanceTriggered, stopChase, damageMultiplier);
        }
    }

    public void Die()
    {
        EcosystemManager.OnMiniBossDeath(monsterName);
    }
}
