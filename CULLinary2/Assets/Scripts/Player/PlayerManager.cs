using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

  public static PlayerManager instance;

  // Player Stats and default values
  public int currentHealth = 200;
  public int maxHealth = 200;
  public int meleeDamage = 20;

  private static PlayerData playerData;

  // Force singleton reference to be the first PlayerManager being instantiated
  private void Awake() {
    DontDestroyOnLoad(this.gameObject);
    if (instance == null) {
      instance = this;
      Debug.Log("Creating instance of Player Manager");
    } else {
      Debug.Log("Duplicate Player Manager Detected. Deleting new Player Manager");
      Destroy(this.gameObject);
    }
  }

  public void SaveData() {
    // Save Player Stats
    playerData.currentHealth = currentHealth;
    playerData.maxHealth = maxHealth;
    // Save Data into Save System
    SaveSystem.SaveData(playerData);
  }

  public void LoadData() {
    // Load Data from Save System
    playerData = SaveSystem.LoadData();
    // Load Player Stats
    currentHealth = playerData.currentHealth;
    maxHealth = playerData.maxHealth;
  }

}
