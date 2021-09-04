using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    public static GameCanvas instance;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
            Debug.Log("Creating instance of GameData");
        }
        else
        {
            Debug.Log("Duplicate GameData Detected. Deleting new GameData");
            Destroy(this.gameObject);
        }
    }
}
