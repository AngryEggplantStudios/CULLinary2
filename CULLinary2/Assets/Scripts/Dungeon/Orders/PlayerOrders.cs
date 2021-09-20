using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOrders : MonoBehaviour
{
    public GameObject ordersUi;
    private KeyCode openOrdersKeyCode;

    private void Awake()
    {
        openOrdersKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenOrders);
    }

    public void ToggleOrders()
    {
        Debug.Log("orders toggled");
        OrdersManager.instance.FirstGenerationOfOrders();
        ordersUi.SetActive(!ordersUi.activeSelf);
    }

    private void Update()
    {
        if (Input.GetKeyDown(openOrdersKeyCode))
        {
            ToggleOrders();
        }
    }
}
