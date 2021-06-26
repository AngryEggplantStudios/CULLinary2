using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
  [SerializeField] private Image healthBar;
  [SerializeField] private Text healthText;
  private bool isInvincible = false;

  private void Awake() {
    int health = PlayerManager.instance ? PlayerManager.instance.currentHealth : 200;
  }
}
