using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTest : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            TestHeal();
        }
    }

    public void TestHeal()
    {
        PlayerHealth.instance.IncreaseHealth(50f);
    }

    public void TestUnlimitedStamina()
    {
        StartCoroutine(PlayerStamina.instance.ToggleUnlimitedStamina(30f));
    }

    public void TestUnlimitedHealth()
    {
        StartCoroutine(PlayerHealth.instance.MakePlayerInvincibleByBuff(30f));
    }

    public void TestMultiplyEarningsByTwo()
    {
        StartCoroutine(OrdersManager.instance.ToggleDoubleEarnings(100f));
    }

    public void TestMeleeAttackBoost()
    {
        StartCoroutine(PlayerManager.instance.DoubleMeleeDamage(30f));
    }

    public void TestEvasionBoost()
    {
        StartCoroutine(PlayerManager.instance.ToggleEvasionBoost(50, 30f));
    }

    public void TestCritBoost()
    {
        StartCoroutine(PlayerManager.instance.ToggleCritBoost(50, 30f));
    }

}
