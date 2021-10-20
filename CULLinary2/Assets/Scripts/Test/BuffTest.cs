using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffTest : MonoBehaviour
{
    [SerializeField] private Sprite testSprite;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            TestCritBoost();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            TestUnlimitedHealth();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            TestUnlimitedStamina();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            TestMeleeAttackBoost();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TestEvasionBoost();
        }
    }

    public void TestHeal()
    {
        PlayerHealth.instance.IncreaseHealth(50f);
    }

    public void TestUnlimitedStamina()
    {
        BuffManager.instance.AddBuff(testSprite, 30, "Unlimited Stamina");
        StartCoroutine(PlayerStamina.instance.ToggleUnlimitedStamina(30f));
    }

    public void TestUnlimitedHealth()
    {
        BuffManager.instance.AddBuff(testSprite, 30, "Unlimited Health");
        StartCoroutine(PlayerHealth.instance.MakePlayerInvincibleByBuff(30f));
    }

    public void TestMultiplyEarningsByTwo()
    {
        BuffManager.instance.AddBuff(testSprite, 50, "Double Earnings");
        StartCoroutine(OrdersManager.instance.ToggleDoubleEarnings(50f));
    }

    public void TestMeleeAttackBoost()
    {
        BuffManager.instance.AddBuff(testSprite, 20, "Melee Boost");
        StartCoroutine(PlayerManager.instance.DoubleMeleeDamage(20f));
    }

    public void TestEvasionBoost()
    {
        BuffManager.instance.AddBuff(testSprite, 80, "Evasion Boost");
        StartCoroutine(PlayerManager.instance.ToggleEvasionBoost(50, 80f));
    }

    public void TestCritBoost()
    {
        BuffManager.instance.AddBuff(testSprite, 120, "Crit Boost");
        StartCoroutine(PlayerManager.instance.ToggleCritBoost(50, 120f));
    }

}
