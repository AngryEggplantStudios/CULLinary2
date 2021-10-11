using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerConsume : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TMP_Text healthPotions;

    private KeyCode consumableOneKeyCode;
    private PlayerHealth playerHealth;

    private void DisplayOnUI()
    {
        healthPotions.text = "x " + PlayerManager.instance.consumables[0];
    }

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        consumableOneKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable1); //Health
    }

    private void Start()
    {
        DisplayOnUI();
    }
    private void Update()
    {
        if (!UIController.instance.isMenuActive && !UIController.instance.isFireplaceActive && !UIController.instance.isPaused && Input.GetKeyDown(consumableOneKeyCode) && PlayerManager.instance.consumables[0] > 0)
        {
            Debug.Log("Consuming health potion");
            audioSource.Play();
            PlayerManager.instance.consumables[0] -= 1;
            ConsumableShopItem consumableShopItem = (ConsumableShopItem)DatabaseLoader.GetShopItemById(7);
            playerHealth.IncreaseHealth(consumableShopItem.healAmount);
            DisplayOnUI();
        }
    }
}
