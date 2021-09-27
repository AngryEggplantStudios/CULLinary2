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
    [SerializeField] private GameObject player;
    // [Header("Scene Transition")]
    // [SerializeField] private GameObject sceneTransition;

    private KeyCode ToggleInventoryKeyCode;
    private KeyCode ToggleOrdersKeyCode;
    private KeyCode ToggleRecipesKeyCode;
    private KeyCode ToggleCreaturesKeyCode;
    private KeyCode ToggleMapKeyCode;
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
        ToggleInventoryKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenInventory);
        ToggleOrdersKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenOrders);
        ToggleRecipesKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenRecipeBook);
        ToggleCreaturesKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenCreatures);
        ToggleMapKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenMap);
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
        if (!isDeathMenuActive)
        {
            Debug.Log("show death screen");
            Time.timeScale = 0;
            playerDeathMenu.SetActive(true);
            isDeathMenuActive = true;
        }
    }

    public void RespawnPlayer()
    {
        Debug.Log("go to next day");
        GameTimer.instance.GoToNextDay();

        Debug.Log("heal player");
        PlayerManager.instance.currentHealth = PlayerManager.instance.maxHealth;
        PlayerManager.instance.currentStamina = PlayerManager.instance.maxStamina;

        // Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        // Transform respawnPoint = GameObject.FindWithTag("RespawnPoint").transform;
        player.transform.position = new Vector3(0f, 1f, 0f);

        // Debug.Log("move player to " + respawnPoint.position);
        Debug.Log("player is now at " + player.transform.position);
        playerDeathMenu.SetActive(false);
        isDeathMenuActive = false;
        Time.timeScale = 1;
        Debug.Log("done respawning player, timescale: " + Time.timeScale);

        // sceneTransition.SetActive(true);
        // SceneManager.LoadScene((int)SceneIndexes.MAIN_SCENE);
        // StartCoroutine(ReloadMainScene());
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

    public void ToggleInventory()
    {
        mainHud.SetActive(inventoryTab.activeSelf);
        Time.timeScale = inventoryTab.activeSelf ? 1f : 0f;
        isMenuActive = !inventoryTab.activeSelf;
        inventoryTab.SetActive(!inventoryTab.activeSelf);
        ordersTab.SetActive(false);
        recipesTab.SetActive(false);
        creaturesTab.SetActive(false);
        mapTab.SetActive(false);
        currentUiPage = (int)UIPage.INVENTORY;
    }

    public void ToggleOrders()
    {
        mainHud.SetActive(ordersTab.activeSelf);
        Time.timeScale = ordersTab.activeSelf ? 1f : 0f;
        isMenuActive = !ordersTab.activeSelf;
        inventoryTab.SetActive(false);
        ordersTab.SetActive(!ordersTab.activeSelf);
        recipesTab.SetActive(false);
        creaturesTab.SetActive(false);
        mapTab.SetActive(false);
        currentUiPage = (int)UIPage.ORDERS;
    }

    public void ToggleRecipes()
    {
        mainHud.SetActive(recipesTab.activeSelf);
        Time.timeScale = recipesTab.activeSelf ? 1f : 0f;
        isMenuActive = !recipesTab.activeSelf;
        inventoryTab.SetActive(false);
        ordersTab.SetActive(false);
        recipesTab.SetActive(!recipesTab.activeSelf);
        creaturesTab.SetActive(false);
        mapTab.SetActive(false);
        currentUiPage = (int)UIPage.RECIPES;
    }

    public void ToggleCreatures()
    {
        mainHud.SetActive(creaturesTab.activeSelf);
        Time.timeScale = creaturesTab.activeSelf ? 1f : 0f;
        isMenuActive = !creaturesTab.activeSelf;
        inventoryTab.SetActive(false);
        ordersTab.SetActive(false);
        recipesTab.SetActive(false);
        creaturesTab.SetActive(!creaturesTab.activeSelf);
        mapTab.SetActive(false);
        currentUiPage = (int)UIPage.CREATURES;
    }

    public void ToggleMap()
    {
        mainHud.SetActive(mapTab.activeSelf);
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

        if (Input.GetKeyDown(ToggleInventoryKeyCode))
        {
            UIController.instance.ToggleInventory();
        }
        else if (Input.GetKeyDown(ToggleOrdersKeyCode))
        {
            UIController.instance.ToggleOrders();
        }
        else if (Input.GetKeyDown(ToggleRecipesKeyCode))
        {
            UIController.instance.ToggleRecipes();
        }
        else if (Input.GetKeyDown(ToggleCreaturesKeyCode))
        {
            UIController.instance.ToggleCreatures();
        }
        else if (Input.GetKeyDown(ToggleMapKeyCode))
        {
            UIController.instance.ToggleMap();
        }
        else if (Input.GetKeyDown(interactKeyCode) && currentInteractable != null)
        {
            currentInteractable.OnPlayerInteract();
        }
        else if (isMenuActive)
        {
            if (Input.GetKeyDown(rightUiTabKeyCode))
            {
                currentUiPage = (currentUiPage + 1) % 5;
                HandlePageChange();
            }
            else if (Input.GetKeyDown(leftUiTabKeyCode))
            {
                currentUiPage = (currentUiPage + 4) % 5;
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
                currentFireplaceUiPage = (currentFireplaceUiPage + 1) % 3;
                HandleFireplacePageChange();
            }
            else if (Input.GetKeyDown(leftUiTabKeyCode))
            {
                currentFireplaceUiPage = (currentFireplaceUiPage + 2) % 3;
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
                ToggleInventory();
                break;
            case (int)UIPage.ORDERS:
                ToggleOrders();
                break;
            case (int)UIPage.RECIPES:
                ToggleRecipes();
                break;
            case (int)UIPage.CREATURES:
                ToggleCreatures();
                break;
            case (int)UIPage.MAP:
                ToggleMap();
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