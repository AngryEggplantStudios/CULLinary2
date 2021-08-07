using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public static PlayerManager instance;

    // Player Stats and default values
    public float currentHealth = 200f;
    public float maxHealth = 200f;
    public float currentStamina = 100f;
    public float maxStamina = 100f;
    public float meleeDamage = 20f;

    private static PlayerData playerData;
    private GameTimer timer;

    // Force singleton reference to be the first PlayerManager being instantiated
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
            timer = GameTimer.instance;
            Debug.Log(timer);
            Debug.Log("Creating instance of Player Manager");
        }
        else
        {
            Debug.Log("Duplicate Player Manager Detected. Deleting new Player Manager");
            Destroy(this.gameObject);
        }
    }

    public void SaveData()
    {
        // Save Player Stats
        playerData.currentHealth = currentHealth;
        playerData.maxHealth = maxHealth;
        playerData.currentStamina = currentStamina;
        playerData.maxStamina = maxStamina;
        //TODO: Add GameTime after completing
        // Save Data into Save System
        SaveSystem.SaveData(playerData);
    }

    public void LoadData()
    {
        // Load Data from Save System
        playerData = SaveSystem.LoadData();
        // Load Player Stats
        currentHealth = playerData.currentHealth;
        maxHealth = playerData.maxHealth;
        currentStamina = playerData.currentStamina;
        maxStamina = playerData.maxStamina;
    }

}
