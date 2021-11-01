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
    [SerializeField] private GameObject itemPanel;
    [Header("Effect Description")]
    [SerializeField] private TMP_Text currentLevelText;
    [SerializeField] private TMP_Text nextLevelText;
    [SerializeField] private GameObject currentLevelEffectPrefab;
    [SerializeField] private GameObject nextLevelEffectPrefab;
    [SerializeField] private GameObject currentLevelEffectList;
    [SerializeField] private GameObject nextLevelEffectList;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Button equipButton;
    [SerializeField] private TMP_Text equipText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text upgradeText;
    [SerializeField] private TMP_Text itemPrice;

    [Header("Audio References")]
    [SerializeField] private AudioSource kaching;
    [SerializeField] private AudioSource equipSound;
    [Header("Player References")]

    [SerializeField] private PlayerSecondaryAttack playerSecondaryAttack;
    [SerializeField] private PlayerSlash playerSlash;
    [Header("Colors")]
    [SerializeField] private Color ableToBePurchasedColor;
    [SerializeField] private Color unableToBePurchasedColor;

    [Header("Stats")]
    [SerializeField] private TMP_Text staminaText;
    [SerializeField] private TMP_Text baseDamageText;
    [SerializeField] private TMP_Text weaponDamageText;
    [SerializeField] private TMP_Text totalDamageText;
    [SerializeField] private TMP_Text secondaryDamageText;

    private int currentTab = 0; //0 for primary, 1 for secondary
    private int primarySelectedSlotId = -1;
    private int secondarySelectedSlotId = -1;
    private List<WeaponSlot> primarySlots;
    private List<WeaponSlot> secondarySlots;

    private void UpdateWeaponSkillStats()
    {
        WeaponSkillItem primaryWeapon = DatabaseLoader.GetWeaponSkillById(PlayerManager.instance.currentWeaponHeld);
        WeaponSkillItem secondarySkill = DatabaseLoader.GetWeaponSkillById(PlayerManager.instance.currentSecondaryHeld);
        int primaryWeaponDamage = 0;
        int secondarySkillDamage = 0;
        if (primaryWeapon.GetType() == typeof(WeaponItem))
        {
            WeaponItem weaponItem = (WeaponItem)primaryWeapon;
            primaryWeaponDamage = weaponItem.baseDamage[PlayerManager.instance.weaponSkillArray[weaponItem.weaponSkillId]];
            weaponDamageText.text = primaryWeaponDamage + " DMG";
        }
        if (secondarySkill.GetType() == typeof(SkillItem))
        {
            SkillItem skillItem = (SkillItem)secondarySkill;
            secondarySkillDamage = skillItem.attackDamage[PlayerManager.instance.weaponSkillArray[skillItem.weaponSkillId]];
        }
        staminaText.text = PlayerManager.instance.currentStamina + " / " + PlayerManager.instance.maxStamina;
        baseDamageText.text = PlayerManager.instance.isMeleeDamageDoubled ? (PlayerManager.instance.meleeDamage * 2) + " DMG" : PlayerManager.instance.meleeDamage + " DMG";
        int minTotalMeleeDamage = Mathf.RoundToInt((PlayerManager.instance.isMeleeDamageDoubled ? PlayerManager.instance.meleeDamage * 2 : PlayerManager.instance.meleeDamage) + 0.85f * primaryWeaponDamage);
        int maxTotalMeleeDamage = Mathf.RoundToInt((PlayerManager.instance.isMeleeDamageDoubled ? PlayerManager.instance.meleeDamage * 2 : PlayerManager.instance.meleeDamage) + 1.15f * primaryWeaponDamage);
        totalDamageText.text = minTotalMeleeDamage + " ~ " + maxTotalMeleeDamage + " DMG";
        int minSecondaryDamage = Mathf.RoundToInt(secondarySkillDamage * 0.85f);
        int maxSecondaryDamage = Mathf.RoundToInt(secondarySkillDamage * 1.15f);
        secondaryDamageText.text = minSecondaryDamage + " ~ " + maxSecondaryDamage + " DMG";
    }


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
        UpdateShopDescription();
        kaching.Play();
    }

    public void HandlePurchase()
    {
        WeaponSkillItem itemToBePurchased;
        if (currentTab == 0 && primarySelectedSlotId != -1)
        {
            itemToBePurchased = primarySlots[primarySelectedSlotId].weaponSkillItem;
        }
        else if (currentTab == 1 && secondarySelectedSlotId != -1)
        {
            itemToBePurchased = secondarySlots[secondarySelectedSlotId].weaponSkillItem;
        }
        else
        {
            return;
        }

        int currentLevel = PlayerManager.instance.upgradesArray[itemToBePurchased.weaponSkillId];
        int itemPrice = itemToBePurchased.price[currentLevel];

        if (itemPrice > PlayerManager.instance.currentMoney)
        {
            return;
        }

        if (itemToBePurchased.GetType() == typeof(WeaponItem))
        {
            WeaponItem weaponItem = (WeaponItem)itemToBePurchased;
            PlayerManager.instance.weaponSkillArray[weaponItem.weaponSkillId]++;
            if (PlayerManager.instance.currentWeaponHeld == weaponItem.weaponSkillId)
            {
                playerSlash.ChangeWeapon(weaponItem.weaponSkillId);
            }
        }
        else if (itemToBePurchased.GetType() == typeof(SkillItem))
        {
            SkillItem skillItem = (SkillItem)itemToBePurchased;
            PlayerManager.instance.weaponSkillArray[skillItem.weaponSkillId]++;
            if (PlayerManager.instance.currentSecondaryHeld == skillItem.weaponSkillId)
            {
                playerSecondaryAttack.ChangeSecondaryAttack(skillItem.weaponSkillId);
            }
        }

        kaching.Play();
        PlayerManager.instance.currentMoney -= itemPrice;
        UpdateShopDescription();
        InventoryManager.instance.ForceUIUpdate();
        UIController.instance.UpdateFixedHUD();
    }

    public void HandleTabSwitch(int tab)
    {
        currentTab = tab;
        UpdateShopDescription();
    }

    public void UpdateShopDescription()
    {
        UpdateWeaponSkillStats();
        if ((currentTab == 0 && primarySelectedSlotId == -1) || (currentTab == 1 && secondarySelectedSlotId == -1))
        {
            return;
        }
        itemPanel.SetActive(true);
        int weaponId = 0;
        if (currentTab == 0)
        {
            weaponId = primarySlots[primarySelectedSlotId].weaponSkillItem.weaponSkillId;
        }
        else if (currentTab == 1)
        {
            weaponId = secondarySlots[secondarySelectedSlotId].weaponSkillItem.weaponSkillId;
        }
        WeaponSkillItem weaponSkillItem = DatabaseLoader.GetWeaponSkillById(weaponId);
        int currentLevel = PlayerManager.instance.weaponSkillArray[weaponSkillItem.weaponSkillId];
        itemName.text = weaponSkillItem.itemName;
        itemDescription.text = weaponSkillItem.description[currentLevel];
        itemIcon.sprite = weaponSkillItem.icon;
        itemPrice.text = "$" + weaponSkillItem.price[currentLevel];
        if (weaponId == PlayerManager.instance.currentWeaponHeld || weaponId == PlayerManager.instance.currentSecondaryHeld)
        {
            equipText.text = "Equipped";
            equipButton.interactable = false;
        }
        else if (PlayerManager.instance.weaponSkillArray[weaponId] == 0)
        {
            equipText.text = "Not Owned";
            equipButton.interactable = false;
        }
        else
        {
            equipText.text = "Equip";
            equipButton.interactable = true;
        }

        if (PlayerManager.instance.currentMoney < weaponSkillItem.price[currentLevel])
        {
            upgradeButton.interactable = false;
            itemPrice.color = unableToBePurchasedColor;
        }
        else
        {
            upgradeButton.interactable = true;
            itemPrice.color = ableToBePurchasedColor;
        }

        foreach (Transform effect in currentLevelEffectList.transform)
        {
            Destroy(effect.gameObject);
        }
        foreach (Transform effect in nextLevelEffectList.transform)
        {
            Destroy(effect.gameObject);
        }

        currentLevelText.text = "Current: Lvl " + currentLevel;
        nextLevelText.text = "Next: Lvl " + (currentLevel + 1);

        if (weaponSkillItem.GetType() == typeof(WeaponItem))
        {
            WeaponItem weaponItem = (WeaponItem)weaponSkillItem;
            if (currentLevel > 0)
            {
                GameObject damageEffectCurrent = Instantiate(currentLevelEffectPrefab);
                damageEffectCurrent.GetComponent<EffectDescriptionSlot>().SetupSlot("Damage: " + weaponItem.baseDamage[currentLevel] + " DMG");
                GameObject attackSpeedCurrent = Instantiate(currentLevelEffectPrefab);
                attackSpeedCurrent.GetComponent<EffectDescriptionSlot>().SetupSlot("Attack Speed: " + weaponItem.attackSpeed[currentLevel]);
                damageEffectCurrent.transform.SetParent(currentLevelEffectList.transform);
                attackSpeedCurrent.transform.SetParent(currentLevelEffectList.transform);
                upgradeText.text = "Upgrade";
            }
            else
            {
                upgradeText.text = "Purchase";
            }

            if (currentLevel + 1 > weaponItem.maxLevel)
            {
                itemPrice.color = unableToBePurchasedColor;
                itemPrice.text = "$ N/A";
                nextLevelText.text = "Max level reached";
                upgradeButton.interactable = false;
            }
            else
            {
                upgradeButton.interactable = true;
                GameObject damageEffectNext = Instantiate(nextLevelEffectPrefab);
                damageEffectNext.GetComponent<EffectDescriptionWithIncrementSlot>().SetupSlot(
                    "Damage: " + weaponItem.baseDamage[currentLevel + 1] + " DMG",
                    weaponItem.baseDamage[currentLevel + 1] - weaponItem.baseDamage[currentLevel]
                );
                GameObject attackSpeedNext = Instantiate(nextLevelEffectPrefab);
                attackSpeedNext.GetComponent<EffectDescriptionWithIncrementSlot>().SetupSlot(
                    "Attack Speed: " + weaponItem.attackSpeed[currentLevel],
                    weaponItem.attackSpeed[currentLevel + 1] - weaponItem.attackSpeed[currentLevel]
                );
                damageEffectNext.transform.SetParent(nextLevelEffectList.transform);
                attackSpeedNext.transform.SetParent(nextLevelEffectList.transform);
            }

        }
        else if (weaponSkillItem.GetType() == typeof(SkillItem))
        {
            SkillItem skillItem = (SkillItem)weaponSkillItem;
            if (currentLevel > 0)
            {
                GameObject damageEffectCurrent = Instantiate(currentLevelEffectPrefab);
                damageEffectCurrent.GetComponent<EffectDescriptionSlot>().SetupSlot("Damage: " + skillItem.attackDamage[currentLevel] + " DMG");
                GameObject staminaCostCurrent = Instantiate(currentLevelEffectPrefab);
                staminaCostCurrent.GetComponent<EffectDescriptionSlot>().SetupSlot("Stamina Cost: " + skillItem.staminaCost[currentLevel]);
                damageEffectCurrent.transform.SetParent(currentLevelEffectList.transform);
                staminaCostCurrent.transform.SetParent(currentLevelEffectList.transform);
                upgradeText.text = "Upgrade";
            }
            else
            {
                upgradeText.text = "Purchase";
            }

            if (currentLevel + 1 > skillItem.maxLevel)
            {
                itemPrice.color = unableToBePurchasedColor;
                itemPrice.text = "$ N/A";
                nextLevelText.text = "Max level reached";
                upgradeButton.interactable = false;
            }
            else
            {
                upgradeButton.interactable = true;
                GameObject damageEffectNext = Instantiate(nextLevelEffectPrefab);
                damageEffectNext.GetComponent<EffectDescriptionWithIncrementSlot>().SetupSlot(
                    "Damage: " + skillItem.attackDamage[currentLevel + 1] + " DMG",
                    skillItem.attackDamage[currentLevel + 1] - skillItem.attackDamage[currentLevel]
                );
                GameObject staminaCostNext = Instantiate(nextLevelEffectPrefab);
                staminaCostNext.GetComponent<EffectDescriptionWithIncrementSlot>().SetupSlot(
                    "Stamina Cost: " + skillItem.staminaCost[currentLevel],
                    skillItem.staminaCost[currentLevel + 1] - skillItem.staminaCost[currentLevel]
                );
                damageEffectNext.transform.SetParent(nextLevelEffectList.transform);
                staminaCostNext.transform.SetParent(nextLevelEffectList.transform);
            }

        }

    }

    public void SetupShop()
    {
        itemPanel.SetActive(false);
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
