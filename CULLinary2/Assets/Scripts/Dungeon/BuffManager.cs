using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : SingletonGeneric<BuffManager>
{
    [SerializeField] private GameObject buffSlotPrefab;
    [SerializeField] private GameObject buffUIReference;
    [SerializeField] private List<GameObject> buffObjects = new List<GameObject>();
    [SerializeField] private List<BuffSlot> buffSlots = new List<BuffSlot>();

    public void AddBuff(Sprite icon, int duration, string name)
    {
        GameObject buffUI = Instantiate(buffSlotPrefab);
        BuffSlot buffSlot = buffUI.GetComponent<BuffSlot>();
        buffSlot.SetupBuffSlot(icon, duration, name);
        buffUI.transform.SetParent(buffUIReference.transform);
        buffObjects.Add(buffUI);
        buffSlots.Add(buffSlot);
    }

    public void ClearBuffManager()
    {
        PlayerStamina.instance.ClearBuffs();
        PlayerHealth.instance.ClearBuffs();
        PlayerManager.instance.ClearBuffs();
        OrdersManager.instance.ClearBuffs();
        buffSlots.Clear();
        buffObjects.Clear();
        foreach (GameObject buff in buffUIReference.transform)
        {
            Destroy(buff);
        }

    }

    public void ApplySingleBuff(Sprite buffIcon, int buffDuration, BuffType buffType, int buffAmount = 0)
    {
        switch (buffType)
        {
            case BuffType.BUFF_BASE_DAMAGE:
                BuffManager.instance.AddBuff(buffIcon, buffDuration, "Melee Boost");
                StartCoroutine(PlayerManager.instance.DoubleMeleeDamage(buffDuration));
                break;
            case BuffType.BUFF_MONEY_BONUS:
                BuffManager.instance.AddBuff(buffIcon, buffDuration, "Double Earnings");
                StartCoroutine(OrdersManager.instance.ToggleDoubleEarnings(buffDuration));
                break;
            case BuffType.BUFF_UNLIMITED_STAMINA:
                BuffManager.instance.AddBuff(buffIcon, buffDuration, "Unlimited Stamina");
                StartCoroutine(PlayerStamina.instance.ToggleUnlimitedStamina(buffDuration));
                break;
            case BuffType.BUFF_INVINCIBILITY:
                BuffManager.instance.AddBuff(buffIcon, buffDuration, "Unlimited Health");
                StartCoroutine(PlayerHealth.instance.MakePlayerInvincibleByBuff(buffDuration));
                break;
            case BuffType.BUFF_EVASION_BOOST:
                BuffManager.instance.AddBuff(buffIcon, buffDuration, "Evasion Boost");
                StartCoroutine(PlayerManager.instance.ToggleEvasionBoost(buffAmount, buffDuration));
                break;
            case BuffType.BUFF_CRIT_BOOST:
                BuffManager.instance.AddBuff(buffIcon, buffDuration, "Crit Boost");
                StartCoroutine(PlayerManager.instance.ToggleCritBoost(buffAmount, buffDuration));
                break;
        }

    }

    public void ApplyBuff(InventoryItem item)
    {
        foreach (BuffType buffType in item.buffTypes)
        {
            switch (buffType)
            {
                case BuffType.HEAL_HEALTH:
                    PlayerHealth.instance.IncreaseHealth(item.healHpAmount);
                    UIController.instance.UpdateFixedHUD();
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

}
