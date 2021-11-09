using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshShopOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        if (ShopManager.instance != null)
        {
            ShopManager.instance.UpdateShopDescription();
        }
    }
}
