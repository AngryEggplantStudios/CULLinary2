using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : SingletonGeneric<UIController>
{
    [SerializeField] private GameObject inventoryTab;
    [SerializeField] private GameObject campfireMenu;
    [SerializeField] private GameObject cookingInterface;
    [SerializeField] private GameObject ordersTab;
    [SerializeField] private GameObject recipesTab;
    [SerializeField] private GameObject creaturesTab;
    [SerializeField] private GameObject mapTab;
    private KeyCode openInventoryKeyCode;
    private KeyCode openOrdersKeyCode;
    private KeyCode openRecipesKeyCode;
    private KeyCode openCreaturesKeyCode;
    private KeyCode openMapKeyCode;
    private int currentUiPage;
    private bool isMenuActive = false;

    public override void Awake()
    {
        base.Awake();
        openInventoryKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenInventory);
        openOrdersKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenOrders);
        openRecipesKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenRecipeBook);
        openCreaturesKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenCreatures);
        openMapKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenMap);
    }

    public void OpenInventory()
    {
        Time.timeScale = inventoryTab.activeSelf ? 1f : 0f;
        isMenuActive = !inventoryTab.activeSelf;
        inventoryTab.SetActive(!inventoryTab.activeSelf);
        ordersTab.SetActive(false);
        recipesTab.SetActive(false);
        creaturesTab.SetActive(false);
        mapTab.SetActive(false);
        currentUiPage = (int)UIPage.INVENTORY;
    }

    public void OpenOrders()
    {
        Time.timeScale = ordersTab.activeSelf ? 1f : 0f;
        isMenuActive = !ordersTab.activeSelf;
        inventoryTab.SetActive(false);
        ordersTab.SetActive(!ordersTab.activeSelf);
        recipesTab.SetActive(false);
        creaturesTab.SetActive(false);
        mapTab.SetActive(false);
        currentUiPage = (int)UIPage.ORDERS;
    }

    public void OpenRecipes()
    {
        Time.timeScale = recipesTab.activeSelf ? 1f : 0f;
        isMenuActive = !recipesTab.activeSelf;
        inventoryTab.SetActive(false);
        ordersTab.SetActive(false);
        recipesTab.SetActive(!recipesTab.activeSelf);
        creaturesTab.SetActive(false);
        mapTab.SetActive(false);
        currentUiPage = (int)UIPage.RECIPES;
    }

    public void OpenCreatures()
    {
        Time.timeScale = creaturesTab.activeSelf ? 1f : 0f;
        isMenuActive = !creaturesTab.activeSelf;
        inventoryTab.SetActive(false);
        ordersTab.SetActive(false);
        recipesTab.SetActive(false);
        creaturesTab.SetActive(!creaturesTab.activeSelf);
        mapTab.SetActive(false);
        currentUiPage = (int)UIPage.CREATURES;
    }

    public void OpenMap()
    {
        Time.timeScale = mapTab.activeSelf ? 1f : 0f;
        isMenuActive = !mapTab.activeSelf;
        inventoryTab.SetActive(false);
        ordersTab.SetActive(false);
        recipesTab.SetActive(false);
        creaturesTab.SetActive(false);
        mapTab.SetActive(!mapTab.activeSelf);
        currentUiPage = (int)UIPage.MAP;
    }

    public void CloseMenu()
    {
        Time.timeScale = 1f;
        isMenuActive = false;
        inventoryTab.SetActive(false);
        ordersTab.SetActive(false);
        recipesTab.SetActive(false);
        creaturesTab.SetActive(false);
        mapTab.SetActive(false);
    }

    public void OpenCampfireInterface()
    {
        // TODO: campfireMenu.SetActive(true);
        cookingInterface.SetActive(true);
    }

    public void CloseCampfireInterface()
    {
        // TODO: campfireMenu.SetActive(false);
        cookingInterface.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(openInventoryKeyCode))
        {
            UIController.instance.OpenInventory();
        }
        else if (Input.GetKeyDown(openOrdersKeyCode))
        {
            UIController.instance.OpenOrders();
        }
        else if (Input.GetKeyDown(openRecipesKeyCode))
        {
            UIController.instance.OpenRecipes();
        }
        else if (Input.GetKeyDown(openCreaturesKeyCode))
        {
            UIController.instance.OpenCreatures();
        }
        else if (Input.GetKeyDown(openMapKeyCode))
        {
            UIController.instance.OpenMap();
        }
        else if (isMenuActive)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentUiPage = currentUiPage >= 4 ? 0 : currentUiPage + 1;
                HandlePageChange();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                currentUiPage = currentUiPage <= 0 ? 4 : currentUiPage - 1;
                HandlePageChange();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseMenu();
            }
        }
    }

    private void HandlePageChange()
    {
        switch (currentUiPage)
        {
            case (int)UIPage.INVENTORY:
                OpenInventory();
                break;
            case (int)UIPage.ORDERS:
                OpenOrders();
                break;
            case (int)UIPage.RECIPES:
                OpenRecipes();
                break;
            case (int)UIPage.CREATURES:
                OpenCreatures();
                break;
            case (int)UIPage.MAP:
                OpenMap();
                break;
        }
    }
}

public enum UIPage
{
    INVENTORY = 0,
    ORDERS = 1,
    RECIPES = 2,
    CREATURES = 3,
    MAP = 4
}