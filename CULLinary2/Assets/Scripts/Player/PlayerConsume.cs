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
    [SerializeField] private TMP_Text healthPill;
    [SerializeField] private TMP_Text staminaPill;
    [SerializeField] private TMP_Text potion;
    [SerializeField] private TMP_Text pfizerShot;
    [SerializeField] private TMP_Text modernaShot;
    [SerializeField] private Sprite pfizerShotIcon;
    [SerializeField] private Sprite modernaShotIcon;
    [SerializeField] private Sprite healthPillIcon;

    private KeyCode consumableOneKeyCode;
    private KeyCode consumableTwoKeyCode;
    private KeyCode consumableThreeKeyCode;
    private KeyCode consumableFourKeyCode;
    private KeyCode consumableFiveKeyCode;
    private PlayerHealth playerHealth;
    private PlayerStamina playerStamina;

    private void DisplayOnUI()
    {
        healthPill.text = "x " + PlayerManager.instance.healthPill;
        staminaPill.text = "x " + PlayerManager.instance.staminaPill;
        potion.text = "x " + PlayerManager.instance.potion;
        pfizerShot.text = "x " + PlayerManager.instance.pfizerShot;
        modernaShot.text = "x " + PlayerManager.instance.modernaShot;
    }

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerStamina = GetComponent<PlayerStamina>();
        consumableOneKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable1); //Health (Red Pill) 
        consumableTwoKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable2); //Stamina (Blue pill)
        consumableThreeKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable3); //Health + Stamina(Potion)
        consumableFourKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable4); //Crit & Evasion(Pfizer)
        consumableFiveKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable5); //Attack(Moderna)
    }

    private void Start()
    {
        DisplayOnUI();
    }
    private void Update()
    {
        if (!UIController.instance.isMenuActive && !UIController.instance.isPaused)
        {
            if (Input.GetKeyDown(consumableOneKeyCode) && PlayerManager.instance.healthPill > 0)
            {
                PlayerManager.instance.healthPill -= 1;
                ConsumableShopItem consumableShopItem = (ConsumableShopItem)DatabaseLoader.GetShopItemById(7);
                BuffManager.instance.ApplySingleBuff(healthPillIcon, consumableShopItem.duration, BuffType.BUFF_CRIT_BOOST, consumableShopItem.critBoost);
            }
            else if (Input.GetKeyDown(consumableTwoKeyCode) && PlayerManager.instance.staminaPill > 0)
            {
                PlayerManager.instance.staminaPill -= 1;
                ConsumableShopItem consumableShopItem = (ConsumableShopItem)DatabaseLoader.GetShopItemById(8);
                playerStamina.IncreaseStamina(consumableShopItem.staminaAmount);
            }
            else if (Input.GetKeyDown(consumableThreeKeyCode) && PlayerManager.instance.potion > 0)
            {
                PlayerManager.instance.potion -= 1;
                ConsumableShopItem consumableShopItem = (ConsumableShopItem)DatabaseLoader.GetShopItemById(9);
                playerHealth.IncreaseHealth(consumableShopItem.healAmount);
                SpawnNotif("+" + consumableShopItem.healAmount);
            }
            else if (Input.GetKeyDown(consumableFourKeyCode) && PlayerManager.instance.pfizerShot > 0)
            {
                PlayerManager.instance.pfizerShot -= 1;
                ConsumableShopItem consumableShopItem = (ConsumableShopItem)DatabaseLoader.GetShopItemById(10);
                BuffManager.instance.ApplySingleBuff(pfizerShotIcon, consumableShopItem.duration, BuffType.BUFF_EVASION_BOOST, consumableShopItem.evasionBoost);
            }
            else if (Input.GetKeyDown(consumableFiveKeyCode) && PlayerManager.instance.modernaShot > 0)
            {
                PlayerManager.instance.modernaShot -= 1;
                ConsumableShopItem consumableShopItem = (ConsumableShopItem)DatabaseLoader.GetShopItemById(11);
                BuffManager.instance.ApplySingleBuff(modernaShotIcon, consumableShopItem.duration, BuffType.BUFF_BASE_DAMAGE);
            }
            else
            {
                return;
            }

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
