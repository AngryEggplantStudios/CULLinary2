using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerConsume : MonoBehaviour
{
    [SerializeField] private GameObject canvasDisplay;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject comsumeNotif;
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
            // Debug.Log("Consuming health potion");
            PlayerManager.instance.consumables[0] -= 1;
            ConsumableShopItem consumableShopItem = (ConsumableShopItem)DatabaseLoader.GetShopItemById(7);
            playerHealth.IncreaseHealth(consumableShopItem.healAmount);
            SpawnNotif("+" + consumableShopItem.healAmount);
            audioSource.Play();
            DisplayOnUI();
        }
    }

    private void SpawnNotif(string text)
    {
        GameObject notif = Instantiate(comsumeNotif);
        notif.transform.GetComponentInChildren<Text>().text = text;
        notif.transform.SetParent(canvasDisplay.transform);
        notif.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }
}
