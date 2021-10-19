using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotManager : SingletonGeneric<InventorySlotManager>
{
    [Header("Inventory Menu References")]
    [SerializeField] private Image itemMainIcon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private GameObject consumableButtonObject;
    [SerializeField] private GameObject discardButtonObject;
    private InventorySlot[] slots;
    private int selectedSlotId;

    public override void Awake()
    {
        base.Awake();
        slots = gameObject.GetComponentsInChildren<InventorySlot>();
        int i = 0;
        foreach (InventorySlot slot in slots)
        {
            slot.SetupSlot(i);
            i++;
        }
    }

    public void HandleClick(int slotId)
    {
        InventorySlot itemSlot = slots[slotId];
        InventoryItem item = itemSlot.item;

        if (item == null || slotId == selectedSlotId)
        {
            return;
        }

        consumableButtonObject.SetActive(item.isConsumable);
        consumableButtonObject.SetActive(false);
        discardButtonObject.SetActive(true);

        itemMainIcon.enabled = true;
        itemMainIcon.sprite = item.icon;
        itemName.text = item.itemName;
        itemDescription.text = item.description;
        //StartCoroutine(UpdateLayoutGroup());
        itemSlot.gameObject.GetComponent<Outline>().enabled = true;

        if (selectedSlotId != -1)
        {
            slots[selectedSlotId].gameObject.GetComponent<Outline>().enabled = false;
        }

        selectedSlotId = slotId;
    }

    /* IEnumerator UpdateLayoutGroup()
    {
        descriptionLayoutGroup.enabled = false;
        yield return new WaitForEndOfFrame();
        descriptionLayoutGroup.enabled = true;
    } */

    private void OnEnable()
    {
        ResetSlot();
    }

    private void OnDisable()
    {
        if (selectedSlotId != -1)
        {
            slots[selectedSlotId].gameObject.GetComponent<Outline>().enabled = false;
        }
    }

    private void Update()
    {
        if (selectedSlotId != -1)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                HandleDiscard();
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                HandleConsume();
            }
        }
    }

    public void HandleDiscard()
    {
        InventoryItem item = slots[selectedSlotId].item;
        if (InventoryManager.instance != null && item != null)
        {
            slots[selectedSlotId].gameObject.GetComponent<Outline>().enabled = false;
            ResetSlot();
            InventoryManager.instance.RemoveItem(item);
        }
    }

    public void HandleConsume()
    {
        InventoryItem item = slots[selectedSlotId].item;
        if (InventoryManager.instance != null && item != null && item.isConsumable)
        {
            slots[selectedSlotId].gameObject.GetComponent<Outline>().enabled = false;
            ResetSlot();
            InventoryManager.instance.RemoveItem(item);
            switch (item.buffType)
            {
                case BuffType.HEAL_HEALTH:
                    PlayerHealth.instance.IncreaseHealth(item.healHpAmount);
                    break;
                case BuffType.INCREASE_MAX_HEALTH:
                    PlayerManager.instance.maxHealth += item.increaseMaxHpAmount;
                    UIController.instance.UpdateFixedHUD();
                    break;
                case BuffType.INCREASE_BASE_DAMAGE:
                    PlayerManager.instance.meleeDamage += item.increaseBaseDamageAmount;
                    UIController.instance.UpdateFixedHUD();
                    break;
                case BuffType.BUFF_BASE_DAMAGE:
                    BuffManager.instance.AddBuff(item.buffIcon, item.buffDuration, "Melee Boost");
                    StartCoroutine(PlayerManager.instance.DoubleMeleeDamage(item.buffDuration));
                    break;
                case BuffType.BUFF_MONEY_BONUS:
                    BuffManager.instance.AddBuff(item.buffIcon, item.buffDuration, "Double Earnings");
                    StartCoroutine(OrdersManager.instance.ToggleDoubleEarnings(item.buffDuration));
                    break;
                case BuffType.BUFF_UNLIMITED_STAMINA:
                    BuffManager.instance.AddBuff(item.buffIcon, item.buffDuration, "Unlimited Stamina");
                    StartCoroutine(PlayerStamina.instance.ToggleUnlimitedStamina(item.buffDuration));
                    break;
                case BuffType.BUFF_INVINCIBILITY:
                    BuffManager.instance.AddBuff(item.buffIcon, item.buffDuration, "Unlimited Health");
                    StartCoroutine(PlayerHealth.instance.MakePlayerInvincibleByBuff(item.buffDuration));
                    break;
                case BuffType.BUFF_EVASION_BOOST:
                    BuffManager.instance.AddBuff(item.buffIcon, item.buffDuration, "Evasion Boost");
                    StartCoroutine(PlayerManager.instance.ToggleEvasionBoost(item.evasionBoostAmount, item.buffDuration));
                    break;
                case BuffType.BUFF_CRIT_BOOST:
                    BuffManager.instance.AddBuff(item.buffIcon, item.buffDuration, "Crit Boost");
                    StartCoroutine(PlayerManager.instance.ToggleCritBoost(item.critBoostAmount, item.buffDuration));
                    break;
            }
        }
    }

    private void ResetSlot()
    {
        selectedSlotId = -1;
        itemName.text = "";
        itemDescription.text = "";
        itemMainIcon.enabled = false;
        itemMainIcon.sprite = null;
        discardButtonObject.SetActive(false);
        consumableButtonObject.SetActive(false);
    }


}
