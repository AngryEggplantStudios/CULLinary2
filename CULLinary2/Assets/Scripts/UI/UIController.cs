using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIController : SingletonGeneric<UIController>
{
    [Header("Main UI")]
    [SerializeField] private GameObject inventoryTab;
    [SerializeField] private GameObject ordersTab;
    [SerializeField] private GameObject recipesTab;
    [SerializeField] private GameObject weaponsTab;
    [SerializeField] private GameObject shopTab;
    [Header("Main HUD")]
    [SerializeField] private GameObject mainHud;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject playerDeathMenu;
    [SerializeField] private GameObject endOfDayMenu;
    [SerializeField] private GameObject confirmationToMainMenu;
    [SerializeField] private GameObject confirmationToDesktop;
    [Header("Uniquely Non-Campfire Elements")]
    [SerializeField] private List<GameObject> hideAtCampfire;
    [Header("Campfire Elements")]
    [SerializeField] private List<GameObject> showAtCampfire;

    [Header("Fixed HUD References")]
    [SerializeField] private Image healthBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image staminaCircleImage;
    [SerializeField] private TMP_Text healthPill;
    [SerializeField] private TMP_Text staminaPill;
    [SerializeField] private TMP_Text potion;
    [SerializeField] private TMP_Text pfizerShot;
    [SerializeField] private TMP_Text modernaShot;
    [Header("Winning Screen References")]
    [SerializeField] private AudioSource winningAudio;
    [SerializeField] private GameObject winPanel;


    private KeyCode ToggleInventoryKeyCode;
    private KeyCode ToggleOrdersKeyCode;
    private KeyCode ToggleRecipesKeyCode;
    private KeyCode ToggleWeaponsKeyCode;
    private KeyCode ToggleShopKeyCode;
    private KeyCode interactKeyCode;
    private KeyCode rightUiTabKeyCode;
    private KeyCode leftUiTabKeyCode;
    private KeyCode closeUiKeyCode;
    private KeyCode campfireActionKeyCode;
    private int currentUiPage;
    private int currentFireplaceUiPage;
    public bool isMenuActive = false;
    public bool isPaused = false;
    public bool isPlayerInVehicle = false;
    public bool isNewspaperOpen = false;
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
        ToggleWeaponsKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenWeapons);
        ToggleShopKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenShop);
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
        if (confirmationToMainMenu.activeSelf || confirmationToDesktop.activeSelf)
        {
            confirmationToDesktop.SetActive(false);
            confirmationToMainMenu.SetActive(false);
            return;
        }
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
            //isPaused = true;
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
        //isPaused = false;
        player.GetComponent<CharacterController>().enabled = true;
        GameTimer.instance.ShowRandomNews();
        GameTimer.instance.GoToNextDay();
        playerDeathMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void ShowEndOfDayMenu()
    {
        if (endOfDayMenu)
        {
            //isPaused = true;
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
        //isPaused = false;
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
        //isPaused = true;
        Time.timeScale = 0;
    }

    public void CloseWinPanel()
    {
        winPanel.SetActive(false);
        //isPaused = false;
        Time.timeScale = 1;
    }

    public void OnPlayerEnterCampfire()
    {
        StartCoroutine(ActivateCampfireInterface(true));
    }

    public void OnPlayerLeaveCampfire()
    {
        StartCoroutine(ActivateCampfireInterface(false));
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
        weaponsTab.SetActive(false);
        shopTab.SetActive(false);
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
        weaponsTab.SetActive(false);
        shopTab.SetActive(false);
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
        weaponsTab.SetActive(false);
        shopTab.SetActive(false);
        currentUiPage = (int)UIPage.RECIPES;
    }

    public void ToggleWeapons()
    {
        mainHud.SetActive(weaponsTab.activeSelf);
        Time.timeScale = weaponsTab.activeSelf ? 1f : 0f;
        isMenuActive = !weaponsTab.activeSelf;
        inventoryTab.SetActive(false);
        ordersTab.SetActive(false);
        recipesTab.SetActive(false);
        weaponsTab.SetActive(!weaponsTab.activeSelf);
        shopTab.SetActive(false);
        currentUiPage = (int)UIPage.WEAPONS;
    }

    public void ToggleShop()
    {
        mainHud.SetActive(shopTab.activeSelf);
        Time.timeScale = shopTab.activeSelf ? 1f : 0f;
        isMenuActive = !shopTab.activeSelf;
        inventoryTab.SetActive(false);
        ordersTab.SetActive(false);
        recipesTab.SetActive(false);
        weaponsTab.SetActive(false);
        shopTab.SetActive(!shopTab.activeSelf);
        currentUiPage = (int)UIPage.SHOP;
    }

    public void CloseMenu()
    {
        mainHud.SetActive(true);
        Time.timeScale = 1f;
        isMenuActive = false;
        inventoryTab.SetActive(false);
        ordersTab.SetActive(false);
        recipesTab.SetActive(false);
        weaponsTab.SetActive(false);
        shopTab.SetActive(false);
    }

    public void OpenCampfireInterface()
    {
        mainHud.SetActive(true);
        Time.timeScale = 0f;
        isMenuActive = true;
        inventoryTab.SetActive(false);
        ordersTab.SetActive(false);
        recipesTab.SetActive(true);
        weaponsTab.SetActive(false);
        shopTab.SetActive(false);
        currentUiPage = (int)UIPage.RECIPES;
    }

    public void CloseCampfireInterface()
    {
        mainHud.SetActive(true);
        Time.timeScale = 1f;
        isMenuActive = false;
        inventoryTab.SetActive(false);
        ordersTab.SetActive(false);
        recipesTab.SetActive(false);
        weaponsTab.SetActive(false);
        shopTab.SetActive(false);
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
        healthPill.text = "x " + PlayerManager.instance.healthPill;
        staminaPill.text = "x " + PlayerManager.instance.staminaPill;
        potion.text = "x " + PlayerManager.instance.potion;
        pfizerShot.text = "x " + PlayerManager.instance.pfizerShot;
        modernaShot.text = "x " + PlayerManager.instance.modernaShot;
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
                || isMenuActive
                || isPaused
                || endOfDayMenu.activeSelf
                || winPanel.activeSelf;

        if (anyUIActive != anyUIWasActive)
        {
            anyUIWasActive = anyUIActive;
            HandleUIActiveChange(anyUIActive);
        }

        if (playerDeathMenu.activeSelf || isNewspaperOpen)
        {
            return;
        }

        if (!isMenuActive)
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

        // Open menu
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
        else if (Input.GetKeyDown(ToggleWeaponsKeyCode))
        {
            UIController.instance.ToggleWeapons();
        }
        else if (Input.GetKeyDown(ToggleShopKeyCode))
        {
            UIController.instance.ToggleShop();
        }

        if (Input.GetKeyDown(campfireActionKeyCode))
        {
            HandleCampfireAction();
        }

        if (!isMenuActive)
        {
            // Exit the truck if possible
            if (Input.GetKeyDown(interactKeyCode) && isPlayerInVehicle)
            {
                DrivingManager.instance.HandlePlayerLeaveVehicle();
            }
            // Toggle interactable if menu is not active
            else if (Input.GetKeyDown(interactKeyCode) && currentInteractable != null)
            {
                currentInteractable.OnPlayerInteract();
            }
        }
        // Menu/Tabs interface is active
        else if (Input.GetKeyDown(rightUiTabKeyCode))
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

    private void HandleUIActiveChange(bool active)
    {
        BGM.Instance.SetVolume(active ? 0.15f : 0.3f);
    }

    private void HandleCampfireAction()
    {
        switch (currentUiPage)
        {
            case (int)UIPage.RECIPES:
                RecipeManager.instance.CookCurrentlySelected();
                break;
            case (int)UIPage.WEAPONS:
                // TODO
                break;
            case (int)UIPage.SHOP:
                ShopManager.instance.HandlePurchase();
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
            case (int)UIPage.WEAPONS:
                ToggleWeapons();
                break;
            case (int)UIPage.SHOP:
                ToggleShop();
                break;
        }
    }

    // Used to switch between campfire and non-campfire menu
    private IEnumerator ActivateCampfireInterface(bool isCampfire)
    {
        foreach (GameObject obj in hideAtCampfire)
        {
            obj.SetActive(!isCampfire);
            yield return null;
        }
        foreach (GameObject obj in showAtCampfire)
        {
            obj.SetActive(isCampfire);
            yield return null;
        }
    }
}

public enum UIPage
{
    INVENTORY = 0,
    ORDERS = 1,
    RECIPES = 2,
    WEAPONS = 3,
    SHOP = 4
}