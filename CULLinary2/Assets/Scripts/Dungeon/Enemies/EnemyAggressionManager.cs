using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggressionManager : MonoBehaviour
{
    private static EnemyAggressionManager _instance;
    public static EnemyAggressionManager Instance { get { return _instance; } }

    int aggressiveEnemies = 0;
    bool aggro = false;

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
    }

    public void Add(int i)
    {
        aggressiveEnemies += i;

        if (aggro && aggressiveEnemies <= 0)
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
}