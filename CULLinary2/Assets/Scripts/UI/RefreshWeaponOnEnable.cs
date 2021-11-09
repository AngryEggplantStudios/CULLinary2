using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshWeaponOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        if (WeaponManager.instance != null)
        {
            WeaponManager.instance.UpdateShopDescription();
        }
    }
}
