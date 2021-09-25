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
    private KeyCode interactKeyCode;
    private KeyCode rightUiTabKeyCode;
    private KeyCode leftUiTabKeyCode;
    private KeyCode closeUiKeyCode;
    private int currentUiPage;
    private bool isMenuActive = false;

    // For interacting with objects
    private PlayerInteractable currentInteractable = null;

    public override void Awake()
    {
        base.Awake();
        openInventoryKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenInventory);
        openOrdersKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenOrders);
        openRecipesKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenRecipeBook);
        openCreaturesKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenCreatures);
        openMapKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenMap);
        interactKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Interact);
        rightUiTabKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.UiMoveRight);
        leftUiTabKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.UiMoveLeft);
        closeUiKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.CloseMenu);
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

    // Remembers the current player interactable for interaction
    public void SetPlayerInteractable(PlayerInteractable interactable)
    {
        currentInteractable = interactable;
    }

    // Triggers the OnPlayerLeave callback and clears the current interactable
    public void TriggerLeaveAndClearPlayerInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable.OnPlayerLeave();
            currentInteractable = null;
        }
    }

    // Call this to update all the UI
    // 
    // This should be able to be called multiple times without 
    // much performance penalty, as many small changes can
    // cause an update to the UI
    public static void UpdateAllUIs()
    {
        // Stop the coroutines that are currently running
        //InventoryManager.instance.StopAllCoroutines();
        RecipeManager.instance.StopAllCoroutines();
        OrdersManager.instance.StopAllCoroutines();

        // Start the coroutines again
        //InventoryManager.instance.StartCoroutine(InventoryManager.instance.UpdateUI());
        //RecipeManager.instance.StartCoroutine(RecipeManager.instance.UpdateUI());
        OrdersManager.instance.StartCoroutine(OrdersManager.instance.UpdateUI());
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
        else if (Input.GetKeyDown(interactKeyCode) && currentInteractable != null)
        {
            currentInteractable.OnPlayerInteract();
        }
        else if (isMenuActive)
        {
            if (Input.GetKeyDown(rightUiTabKeyCode))
            {
                currentUiPage = currentUiPage >= 4 ? 0 : currentUiPage + 1;
                HandlePageChange();
            }
            else if (Input.GetKeyDown(leftUiTabKeyCode))
            {
                currentUiPage = currentUiPage <= 0 ? 4 : currentUiPage - 1;
                HandlePageChange();
            }
            else if (Input.GetKeyDown(closeUiKeyCode))
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