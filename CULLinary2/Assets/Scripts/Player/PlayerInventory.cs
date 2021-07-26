using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryUI;
    private KeyCode openInventoryKeyCode;

    private void Awake()
    {
        openInventoryKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenInventory);
    }

    public void ToggleInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }

    private void Update()
    {
        if (Input.GetKeyDown(openInventoryKeyCode))
        {
            ToggleInventory();
        }
    }
}
