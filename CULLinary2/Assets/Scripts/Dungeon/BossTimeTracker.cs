using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTimeTracker : SingletonGeneric<BossTimeTracker>
{
    public float gameTime = 0f;
    public bool hasSummonedBoss = false;
    private void Update()
    {
        if (hasSummonedBoss)
        {
            gameTime += Time.deltaTime;
        }
    }

    private void OnDestroy()
    {
        gameTime = 0f;
    }
}
