using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyManager
{
    private static GameObject uiForHpBar = GameObject.Find("HpUI");

    public static void attachToUIForHp(GameObject hpBarToAttach)
    {
        if (hpBarToAttach != null && uiForHpBar != null)
        {
            hpBarToAttach.transform.SetParent(uiForHpBar.transform);
        }

        //return uiForHpBar;
    }
}
