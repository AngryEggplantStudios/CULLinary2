using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : SingletonGeneric<UIController>
{
    [Header("Campfire UI")]
    [SerializeField] private GameObject campfireMenu;
    [SerializeField] private GameObject cookingInterface;
    [SerializeField] private GameObject upgradesInterface;
    [SerializeField] private GameObject weaponsInterface;
    [Header("Main UI")]
    [SerializeField] private GameObject inventoryTab;
    [SerializeField] private GameObject ordersTab;
    [SerializeField] private GameObject recipesTab;
    [SerializeField] private GameObject creaturesTab;
    [SerializeField] private GameObject mapTab;
    [Header("Main HUD")]
    [SerializeField] private GameObject mainHud;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject playerDeathMenu;
    [Header("Scene Transition")]
    [SerializeField] private GameObject sceneTransition;


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
    private int currentFireplaceUiPage;
    private bool isMenuActive = false;
    private bool isFireplaceActive = false;
    private bool isPaused = false;
    private bool isDeathMenuActive = false;

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

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void SaveAndQuit()
    {
        Time.timeScale = 1;
        PlayerManager.instance.SaveData(InventoryManager.instance.itemListReference);
        SceneManager.LoadScene((int)SceneIndexes.MAIN_MENU);
    }

    public void ShowDeathMenu()
    {
        Time.timeScale = 0;
        playerDeathMenu.SetActive(true);
        isDeathMenuActive = true;
    }

    public void RespawnPlayer()
    {
        Time.timeScale = 1;
        // sceneTransition.SetActive(true);
        // SceneManager.LoadScene((int)SceneIndexes.MAIN_SCENE);
        StartCoroutine(ReloadMainScene());
    }

    private IEnumerator ReloadMainScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync((int)SceneIndexes.MAIN_SCENE);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void OpenInventory()
    {
        mainHud.SetActive(false);
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
        mainHud.SetActive(false);
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
        mainHud.SetActive(false);
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
        mainHud.SetActive(false);
        mainHud.SetActive(false);
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
        mainHud.SetActive(false);
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
        mainHud.SetActive(true);
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
        mainHud.SetActive(false);
        isFireplaceActive = true;
        Time.timeScale = 0f;
        campfireMenu.SetActive(true);
        cookingInterface.SetActive(true);
        upgradesInterface.SetActive(false);
        weaponsInterface.SetActive(false);
        currentFireplaceUiPage = (int)FireplaceUIPage.COOKING;
    }

    public void OpenUpgradesInterface()
    {
        mainHud.SetActive(false);
        campfireMenu.SetActive(true);
        upgradesInterface.SetActive(true);
        cookingInterface.SetActive(false);
        weaponsInterface.SetActive(false);
        currentFireplaceUiPage = (int)FireplaceUIPage.UPGRADES;
    }

    public void OpenCookingInterface()
    {
        mainHud.SetActive(false);
        campfireMenu.SetActive(true);
        cookingInterface.SetActive(true);
        upgradesInterface.SetActive(false);
        weaponsInterface.SetActive(false);
        currentFireplaceUiPage = (int)FireplaceUIPage.COOKING;
    }

    public void OpenWeaponsInterface()
    {
        mainHud.SetActive(false);
        campfireMenu.SetActive(true);
        cookingInterface.SetActive(false);
        upgradesInterface.SetActive(false);
        weaponsInterface.SetActive(true);
        currentFireplaceUiPage = (int)FireplaceUIPage.WEAPONS;
    }
    public void CloseCampfireInterface()
    {
        mainHud.SetActive(true);
        Time.timeScale = 1f;
        isFireplaceActive = false;
        campfireMenu.SetActive(false);
        cookingInterface.SetActive(false);
        upgradesInterface.SetActive(false);
        weaponsInterface.SetActive(false);
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
        InventoryManager.instance.StopAllCoroutines();
        RecipeManager.instance.StopAllCoroutines();
        OrdersManager.instance.StopAllCoroutines();

        // Start the coroutines again
        InventoryManager.instance.StartCoroutine(InventoryManager.instance.UpdateUI());
        RecipeManager.instance.StartCoroutine(RecipeManager.instance.UpdateUI());
        OrdersManager.instance.StartCoroutine(OrdersManager.instance.UpdateUI());
    }

    private void Update()
    {
        if (isDeathMenuActive)
        {
            return;
        }

        if (!isFireplaceActive && !isMenuActive)
        {
            if (Input.GetKeyDown(closeUiKeyCode))
            {
                TogglePauseMenu();
            }
        }
        if (isPaused)
        {
            return;
        }

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
                currentUiPage = currentUiPage >= 4 ? 4 : currentUiPage + 1;
                HandlePageChange();
            }
            else if (Input.GetKeyDown(leftUiTabKeyCode))
            {
                currentUiPage = currentUiPage <= 0 ? 0 : currentUiPage - 1;
                HandlePageChange();
            }
            else if (Input.GetKeyDown(closeUiKeyCode))
            {
                CloseMenu();
            }
        }
        else if (isFireplaceActive)
        {
            if (Input.GetKeyDown(rightUiTabKeyCode))
            {
                currentFireplaceUiPage = currentFireplaceUiPage >= 2 ? 2 : currentFireplaceUiPage + 1;
                HandleFireplacePageChange();
            }
            else if (Input.GetKeyDown(leftUiTabKeyCode))
            {
                currentFireplaceUiPage = currentFireplaceUiPage <= 0 ? 0 : currentFireplaceUiPage - 1;
                HandleFireplacePageChange();
            }
            else if (Input.GetKeyDown(closeUiKeyCode))
            {
                CloseCampfireInterface();
            }
        }
    }

    private void HandleFireplacePageChange()
    {
        switch (currentFireplaceUiPage)
        {
            case (int)FireplaceUIPage.COOKING:
                OpenCookingInterface();
                break;
            case (int)FireplaceUIPage.UPGRADES:
                OpenUpgradesInterface();
                break;
            case (int)FireplaceUIPage.WEAPONS:
                OpenWeaponsInterface();
                break;
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

public enum FireplaceUIPage
{
    COOKING = 0,
    UPGRADES = 1,
    WEAPONS = 2
}

public enum UIPage
{
    INVENTORY = 0,
    ORDERS = 1,
    RECIPES = 2,
    CREATURES = 3,
    MAP = 4
}