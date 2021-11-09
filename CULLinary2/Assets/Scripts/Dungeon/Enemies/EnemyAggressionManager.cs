using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggressionManager : MonoBehaviour
{
    private static EnemyAggressionManager _instance;
    public static EnemyAggressionManager Instance { get { return _instance; } }

    int aggressiveEnemies;
    bool aggro;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        aggressiveEnemies = 0;
        aggro = false;
    }

    public void Add(int i)
    {
        aggressiveEnemies += i;
        if (aggressiveEnemies < 0)
        {
            aggressiveEnemies = 0;
        }

        if (aggro && aggressiveEnemies == 0)
        {
            aggro = false;
            BGM.Instance.SetTrack(0);
        }
        else if (!aggro && aggressiveEnemies > 0)
        {
            aggro = true;
            BGM.Instance.SetTrack(1);
        }
    }

    public void Reset()
    {
        aggressiveEnemies = 0;
        aggro = false;
        BGM.Instance.SetTrack(0);
    }
}
