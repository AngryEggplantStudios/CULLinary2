using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
  [SerializeField] private Image healthBar;
  [SerializeField] private Text healthText;
  private bool isInvincible = false;

  private void Awake() {
    int currentHealth = PlayerManager.instance ? PlayerManager.instance.currentHealth : 200;
    int maxHealth = PlayerManager.instance ? PlayerManager.instance.maxHealth : 200;
    healthBar.fillAmount = currentHealth / maxHealth;
    healthText.text = currentHealth + "/" + maxHealth;
  }

  public bool HandleHit(int damage) {
    if (isInvincible) {
      return false;
    }
    PlayerManager.instance.currentHealth -= damage;
    healthBar.fillAmount = PlayerManager.instance.currentHealth / PlayerManager.instance.maxHealth;
    healthText.text = PlayerManager.instance.currentHealth + "/" + PlayerManager.instance.maxHealth;
    //Spawn damage counter
    //Play Audio
    //If health is 0, play game over scene and invoke game over event
    return true;
  }



}
