using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] private GameObject endOfDayMenu;

    [Header("Fixed HUD References")]
    [SerializeField] private Image healthBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image staminaCircleImage;
    [SerializeField] private TMP_Text healthPotions;
    [Header("Winning Screen References")]
    [SerializeField] private AudioSource winningAudio;
    [SerializeField] private GameObject winPanel;

    private KeyCode ToggleInventoryKeyCode;
    private KeyCode ToggleOrdersKeyCode;
    private KeyCode ToggleRecipesKeyCode;
    private KeyCode ToggleCreaturesKeyCode;
    private KeyCode ToggleMapKeyCode;
    private KeyCode interactKeyCode;
    private KeyCode rightUiTabKeyCode;
    private KeyCode leftUiTabKeyCode;
    private KeyCode closeUiKeyCode;
    private KeyCode campfireActionKeyCode;
    private int currentUiPage;
    private int currentFireplaceUiPage;
    public bool isMenuActive = false;
    public bool isFireplaceActive = false;
    public bool isPaused = false;
    private bool deathMenuActive = false;
    private bool anyUIActive = false;
    private bool anyUIWasActive = false;
    private GameObject player;
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
        campfireActionKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.CampfireAction);
    }

    public void Start()
    {
        //Use this for now, later when have more scene only adjust accordingly. TODO MC.
        player = GameObject.FindWithTag("Player");
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void SaveAndQuit()
    {
        isPaused = false;
        Time.timeScale = 1;
        //PlayerManager.instance.SaveData(InventoryManager.instance.itemListReference);
        SceneManager.LoadScene((int)SceneIndexes.MAIN_MENU);
    }

    public void SaveAndExit()
    {
        isPaused = false;
        Time.timeScale = 1;
        //PlayerManager.instance.SaveData(InventoryManager.instance.itemListReference);
        Application.Quit();
    }
    public void ShowDeathMenu()
    {
        if (!playerDeathMenu.activeSelf && !deathMenuActive)
        {
            player.GetComponent<CharacterController>().enabled = false;
            deathMenuActive = true;
            isPaused = true;
            StartCoroutine(PauseToShowDeathAnimation());
        }
    }

    private IEnumerator PauseToShowDeathAnimation()
    {
        yield return new WaitForSeconds(1.9f);
        Time.timeScale = 0;
        player.GetComponent<Animator>().ResetTrigger("isDead");
        player.GetComponent<Animator>().SetTrigger("revive");
        playerDeathMenu.SetActive(true);
    }

    public void RespawnPlayer()
    {
        deathMenuActive = false;
        isPaused = false;
        player.GetComponent<CharacterController>().enabled = true;
        GameTimer.instance.GoToNextDay();
        playerDeathMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void ShowEndOfDayMenu()
    {
        if (endOfDayMenu)
        {
            isPaused = true;
            endOfDayMenu.SetActive(true);
            EndOfDayPanelStatistics stats = endOfDayMenu.GetComponent<EndOfDayPanelStatistics>();
            stats.UpdateStatistics(GameTimer.GetDayNumber(),
                                   OrdersManager.GetNumberOfOrdersCompletedToday(),
                                   OrdersManager.GetNumberOfOrdersGeneratedDaily(),
                                   OrdersManager.GetMoneyEarnedToday(),
                                   EcosystemManager.GetNumberOfMonstersKilledToday());
        }
    }

    public void ContinueToNextDay()
    {
        isPaused = false;
        endOfDayMenu.SetActive(false);
        SceneTransitionManager.instance.FadeOutImage();
        Invoke("ResumeGameTimer", 1);
    }

    private void ResumeGameTimer()
    {
        GameTimer.instance.Run();
    }

    public void ShowWinPanel()
    {
        if (winningAudio != null)
        {
            winningAudio.Play();
        }

        winPanel.SetActive(true);
        isPaused = true;
        Time.timeScale = 0;
    }

    public void CloseWinPanel()
    {
        winPanel.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
    }

    public void ToggleInventory()
    {
        StartCoroutine(InventoryManager.instance.UpdateUI());
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

    public void UpdateFixedHUD()
    {
        healthBar.fillAmount = PlayerManager.instance.currentHealth / PlayerManager.instance.maxHealth;
        healthText.text = Mathf.CeilToInt(PlayerManager.instance.currentHealth) + "/" + Mathf.CeilToInt(PlayerManager.instance.maxHealth);
        staminaCircleImage.fillAmount = PlayerManager.instance.currentStamina / PlayerManager.instance.maxStamina;
        healthPotions.text = "x " + PlayerManager.instance.consumables[0];
    }

    // Call this to update all the UI
    // 
    // This should be able to be called multiple times without 
    // much performance penalty, as many small changes can
    // cause an update to the UI
    public static void UpdateAllUIs()
    {
        // Stop the coroutines that are currently running
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.StopAllCoroutines();
            InventoryManager.instance.StartCoroutine(InventoryManager.instance.UpdateUI());
        }
        if (RecipeManager.instance != null)
        {
            RecipeManager.instance.StopAllCoroutines();
            RecipeManager.instance.StartCoroutine(RecipeManager.instance.UpdateUI());
        }
        if (OrdersManager.instance != null)
        {
            OrdersManager.instance.StopAllCoroutines();
            OrdersManager.instance.StartCoroutine(OrdersManager.instance.UpdateUI());
        }
    }

    private void Update()
    {
        anyUIActive = playerDeathMenu.activeSelf
                || isFireplaceActive
                || isMenuActive
                || isPaused;

        if (anyUIActive != anyUIWasActive)
        {
            anyUIWasActive = anyUIActive;
            HandleUIActiveChange(anyUIActive);
        }

        if (playerDeathMenu.activeSelf)
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

        if (!isFireplaceActive)
        {
            // Open menu if campfire inactive
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
        }
        // Campfire interface is active
        else
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
                RecipeManager.instance.DeactivateCooking();
                CloseCampfireInterface();
            }
            else if (Input.GetKeyDown(campfireActionKeyCode))
            {
                HandleCampfireAction();
            }
        }

        if (!isMenuActive)
        {
            // Toggle fireplace if menu is not active
            if (Input.GetKeyDown(interactKeyCode) && currentInteractable != null)
            {
                currentInteractable.OnPlayerInteract();
            }
        }
        // Menu/Tabs interface is active
        else if (Input.GetKeyDown(rightUiTabKeyCode))
        {
            currentUiPage = (currentUiPage + 1) % 4;
            HandlePageChange();
        }
        else if (Input.GetKeyDown(leftUiTabKeyCode))
        {
            currentUiPage = (currentUiPage + 3) % 4;
            HandlePageChange();
        }
        else if (Input.GetKeyDown(closeUiKeyCode))
        {
            CloseMenu();
        }
    }

    private void HandleUIActiveChange(bool active)
    {
        BGM.Instance.SetVolume(active ? 0.15f : 0.3f);
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

    private void HandleCampfireAction()
    {
        switch (currentFireplaceUiPage)
        {
            case (int)FireplaceUIPage.COOKING:
                RecipeManager.instance.CookCurrentlySelected();
                break;
            case (int)FireplaceUIPage.UPGRADES:
                ShopManager.instance.HandlePurchase();
                break;
            case (int)FireplaceUIPage.WEAPONS:
                // TODO
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
    CREATURES = 3003,
    MAP = 3
}