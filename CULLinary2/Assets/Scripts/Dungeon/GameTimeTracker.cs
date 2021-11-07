using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeTracker : SingletonGeneric<GameTimeTracker>
{
    public float gameTime = 0f;

    private void Update()
    {
        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.gameTime += Time.deltaTime;
        }
    }

    private void OnDestroy()
    {
        gameTime = 0f;
    }
}
