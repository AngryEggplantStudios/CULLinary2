using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponManager : SingletonGeneric<WeaponManager>
{
    [Header("Prefabs & References")]
    [SerializeField] private GameObject primaryParentObject;
    [SerializeField] private GameObject secondaryParentObject;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private TMP_Text currentLevelText;
    [SerializeField] private TMP_Text nextLevelText;
    [SerializeField] private TMP_Text currentLevelEffectText;
    [SerializeField] private TMP_Text nextLevelEffectText;
    [SerializeField] private TMP_Text nextLevelIncrementText;
    [Header("Colors")]
    [SerializeField] private Color ableToBePurchasedColor;
    [SerializeField] private Color unableToBePurchasedColor;
    [SerializeField] private Button equipButton;
    [SerializeField] private TMP_Text equipText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private AudioSource kaching;
    [SerializeField] private PlayerSecondaryAttack playerSecondaryAttack;
    [SerializeField] private PlayerSlash playerSlash;
    private int currentTab = 0; //0 for primary, 1 for secondary
    private int primarySelectedSlotId = -1;
    private int secondarySelectedSlotId = -1;
    private List<WeaponSlot> primarySlots;
    private List<WeaponSlot> secondarySlots;

    public void HandleClick(int index)
    {
        switch (currentTab)
        {
            case 0: //Primary
                if (primarySelectedSlotId == index)
                {
                    return;
                }
                primarySlots[index].GetComponent<WeaponSlot>().ToggleSelect(true);
                if (primarySelectedSlotId != -1)
                {
                    primarySlots[primarySelectedSlotId].GetComponent<WeaponSlot>().ToggleSelect(false);
                }
                primarySelectedSlotId = index;
                break;
            case 1: //Secondary
                if (secondarySelectedSlotId == index)
                {
                    return;
                }
                secondarySlots[index].GetComponent<WeaponSlot>().ToggleSelect(true);
                if (secondarySelectedSlotId != -1)
                {
                    secondarySlots[secondarySelectedSlotId].GetComponent<WeaponSlot>().ToggleSelect(false);
                }
                secondarySelectedSlotId = index;
                break;
        }
        UpdateShopDescription();
    }

    public void HandleEquip()
    {
        if ((currentTab == 0 && primarySelectedSlotId == -1) || (currentTab == 1 && secondarySelectedSlotId == -1))
        {
            return;
        }
        int weaponId = 0;
        if (currentTab == 0)
        {
            weaponId = primarySlots[primarySelectedSlotId].weaponSkillItem.weaponSkillId;
            playerSlash.ChangeWeapon(weaponId);
        }
        else if (currentTab == 1)
        {
            weaponId = secondarySlots[secondarySelectedSlotId].weaponSkillItem.weaponSkillId;
            playerSecondaryAttack.ChangeSecondaryAttack(weaponId);
        }
    }

    public void HandlePurchase()
    {

    }

    public void HandleTabSwitch(int tab)
    {
        /*
        switch (currentTab)
        {
            case 0:
                if (selectedSlotId != -1)
                {
                    secondarySlots[selectedSlotId].GetComponent<WeaponSlot>().ToggleSelect(false);
                }
                break;
            case 1:
                if (selectedSlotId != -1)
                {
                    primarySlots[selectedSlotId].GetComponent<WeaponSlot>().ToggleSelect(false);
                }
                break;
        }
        */
        //selectedSlotId = -1;
        currentTab = tab;
    }

    public void UpdateShopDescription()
    {
        if ((currentTab == 0 && primarySelectedSlotId == -1) || (currentTab == 1 && secondarySelectedSlotId == -1))
        {
            return;
        }
    }

    public void SetupShop()
    {
        List<WeaponSkillItem> weaponSkillItems = DatabaseLoader.GetWeaponSkillList();
        primarySlots = new List<WeaponSlot>();
        secondarySlots = new List<WeaponSlot>();
        int currentPrimarySlotId = 0;
        int currentSecondarySlotId = 0;
        foreach (WeaponSkillItem item in weaponSkillItems)
        {
            if (item.GetType() == typeof(WeaponItem))
            {
                GameObject slotObject = Instantiate(slotPrefab);
                WeaponSlot slot = slotObject.GetComponent<WeaponSlot>();
                slot.SetupUI(item, currentPrimarySlotId);
                currentPrimarySlotId++;
                primarySlots.Add(slot);
                slotObject.transform.SetParent(primaryParentObject.transform);
            }
            if (item.GetType() == typeof(SkillItem))
            {
                GameObject slotObject = Instantiate(slotPrefab);
                WeaponSlot slot = slotObject.GetComponent<WeaponSlot>();
                slot.SetupUI(item, currentSecondarySlotId);
                currentSecondarySlotId++;
                secondarySlots.Add(slot);
                slotObject.transform.SetParent(secondaryParentObject.transform);
            }
        }

    }


}
