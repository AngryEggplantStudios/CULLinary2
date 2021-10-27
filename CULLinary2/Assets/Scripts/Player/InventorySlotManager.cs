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
    [SerializeField] private TMP_Text itemEffects;
    [SerializeField] private TMP_Text itemType;
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

    public void HandleClick(ConsumableShopItem consumableShopItem)
    {
        ResetSlot();
        itemName.text = consumableShopItem.itemName;
        itemDescription.text = consumableShopItem.description[0];
        itemMainIcon.enabled = true;
        itemMainIcon.sprite = consumableShopItem.iconArr[0];
        itemType.text = "Potion";
    }

    public void HandleClickPrimary()
    {
        ResetSlot();
        WeaponSkillItem weaponSkillItem = DatabaseLoader.GetWeaponSkillById(PlayerManager.instance.currentWeaponHeld);
        if (weaponSkillItem.GetType() == typeof(WeaponItem))
        {
            WeaponItem weaponItem = (WeaponItem)weaponSkillItem;
            itemName.text = weaponItem.itemName;
            itemDescription.text = weaponItem.description[0];
            itemMainIcon.enabled = true;
            itemMainIcon.sprite = weaponItem.icon;
            itemType.text = "Melee Weapon";
            itemEffects.text = weaponItem.GetDescription(PlayerManager.instance.weaponSkillArray[weaponItem.weaponSkillId]);
        }
    }

    public void HandleClickSecondary()
    {
        ResetSlot();
        WeaponSkillItem weaponSkillItem = DatabaseLoader.GetWeaponSkillById(PlayerManager.instance.currentSecondaryHeld);
        if (weaponSkillItem.GetType() == typeof(SkillItem))
        {
            SkillItem skillItem = (SkillItem)weaponSkillItem;
            itemName.text = skillItem.itemName;
            itemDescription.text = skillItem.description[0];
            itemMainIcon.enabled = true;
            itemMainIcon.sprite = skillItem.icon;
            itemType.text = "Skill";
            itemEffects.text = skillItem.GetDescription(PlayerManager.instance.weaponSkillArray[skillItem.weaponSkillId]);
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
        discardButtonObject.SetActive(true);
        itemMainIcon.enabled = true;
        itemMainIcon.sprite = item.icon;
        itemName.text = item.itemName;
        itemDescription.text = item.description;
        itemEffects.text = item.GetConsumeEffect();
        if (item.isConsumable)
        {
            itemType.text = "Dish";
        }
        else
        {
            itemType.text = "Ingredient";
        }
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
        InventoryManager.instance.ForceUIUpdate();
    }

    private void OnDisable()
    {
        if (selectedSlotId != -1)
        {
            slots[selectedSlotId].gameObject.GetComponent<Outline>().enabled = false;
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
            item.buffIcon = item.buffIcon == null ? item.icon : item.buffIcon;
            BuffManager.instance.ApplyBuff(item);
            ResetSlot();
            InventoryManager.instance.RemoveItem(item);
        }
    }

    private void ResetSlot()
    {
        itemType.text = "";
        selectedSlotId = -1;
        itemName.text = "";
        itemDescription.text = "";
        itemEffects.text = "";
        itemMainIcon.enabled = false;
        itemMainIcon.sprite = null;
        discardButtonObject.SetActive(false);
        consumableButtonObject.SetActive(false);
    }


}
