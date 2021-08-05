using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : SingletonGeneric<GameTimer>
{
    private float dayLengthInMinutes = 1f;
    private static float gameTime;
    private static float timeScale;
    private int dayNum;
    void Start()
    {
        gameTime = 0f;
        timeScale = 24 / (dayLengthInMinutes / 60);
        Debug.Log("Starting class");
    }

    private void Update()
    {
        gameTime += Time.deltaTime * timeScale / 86400;
        //Debug.Log(gameTime);
        if (gameTime > 1)
        {
            dayNum++;
            gameTime -= 1;
            Debug.Log(dayNum);
        }
    }

    public static float GetTime()
    {
        return gameTime;
    }

    private void OnDestroy()
    {
        gameTime = 0f;
    }
}