using UnityEngine;

[System.Serializable]
public class PlayerData {

  public string inventory;
  public int maxHealth;
  public int currentHealth;
  public string ToJson() {
    return JsonUtility.ToJson(this);
  }

  public void LoadFromJson(string jsonData) {
    try {
      JsonUtility.FromJsonOverwrite(jsonData, this);
    } catch {
      Debug.Log("No save file...");
    }
  }
}
