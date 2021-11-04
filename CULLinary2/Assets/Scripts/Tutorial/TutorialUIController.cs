using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TutorialUIController : SingletonGeneric<TutorialUIController>
{
    [Header("Main UI")]
    [SerializeField] private GameObject inventoryTab;
    [SerializeField] private GameObject ordersTab;
    [SerializeField] private GameObject recipesTab;

    [Header("Main HUD")]
    [SerializeField] private GameObject mainHud;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject endOfDayMenu;
    [SerializeField] private GameObject confirmationToMainMenu;
    [SerializeField] private GameObject confirmationToDesktop;
    [SerializeField] private GameObject controlsPage;
    [SerializeField] private GameObject settingsPage;

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
    [SerializeField] private Image primaryAttack;
    [SerializeField] private Image secondaryAttack;

    private KeyCode ToggleInventoryKeyCode;
    private KeyCode ToggleOrdersKeyCode;
    private KeyCode ToggleRecipesKeyCode;
    private KeyCode interactKeyCode;
    private KeyCode rightUiTabKeyCode;
    private KeyCode leftUiTabKeyCode;
    private KeyCode closeUiKeyCode;
    private KeyCode campfireActionKeyCode;
    private int currentUiPage;
    private int currentFireplaceUiPage;
    public bool isMenuActive = false;
    public bool isPaused = false;
    public bool isDialogueOpen = false;
    private bool anyUIActive = false;
    private bool anyUIWasActive = false;
    private GameObject player;
    // For interacting with objects
    private List<PlayerInteractable> currentInteractables = new List<PlayerInteractable>();

    // For triggering tutorial events
    private bool wasOrdersOpened = false;
    private bool wasRecipesOpened = false;

    public override void Awake()
    {
        base.Awake();
        ToggleInventoryKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenInventory);
        ToggleOrdersKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenOrders);
        ToggleRecipesKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenRecipeBook);
        // ToggleWeaponsKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenWeapons);
        // ToggleShopKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenShop);
        interactKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Interact);
        rightUiTabKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.UiMoveRight);
        leftUiTabKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.UiMoveLeft);
        closeUiKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.CloseMenu);
        campfireActionKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.CampfireAction);
    }

    public void OpenControls()
    {
        controlsPage.SetActive(!controlsPage.activeSelf);
    }

    public void OpenSettings()
    {
        settingsPage.SetActive(!settingsPage.activeSelf);
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
        if (controlsPage.activeSelf)
        {
            controlsPage.SetActive(false);
        }
        if (settingsPage.activeSelf)
        {
            settingsPage.SetActive(false);
        }
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    // private IEnumerator PauseToShowDeathAnimation()
    // {
    //     yield return new WaitForSeconds(1.9f);
    //     Time.timeScale = 0;
    //     player.GetComponent<Animator>().ResetTrigger("isDead");
    //     player.GetComponent<Animator>().SetTrigger("revive");
    // }

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

    // public void ContinueToNextDay()
    // {
    //     //isPaused = false;
    //     endOfDayMenu.SetActive(false);
    //     SceneTransitionManager.instance.FadeOutImage();
    //     Invoke("ResumeGameTimer", 1);
    // }

    private void ResumeGameTimer()
    {
        GameTimer.instance.Run();
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
        // WeaponManager.instance.UpdateWeaponSkillStats();
        InventoryManager.instance.ForceUIUpdate();
        mainHud.SetActive(inventoryTab.activeSelf);
        Time.timeScale = inventoryTab.activeSelf ? 1f : 0f;
        isMenuActive = !inventoryTab.activeSelf;
        inventoryTab.SetActive(!inventoryTab.activeSelf);
        ordersTab.SetActive(false);
        recipesTab.SetActive(false);
        // weaponsTab.SetActive(false);
        // shopTab.SetActive(false);
        // creatureTab.SetActive(false);
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
        // weaponsTab.SetActive(false);
        // shopTab.SetActive(false);
        // creatureTab.SetActive(false);
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
        // weaponsTab.SetActive(false);
        // shopTab.SetActive(false);
        // creatureTab.SetActive(false);
        currentUiPage = (int)UIPage.RECIPES;
    }

    public bool WereOrdersAndRecipesOpened()
    {
        Debug.Log("orders opened?: " + wasOrdersOpened + ", recipes opened?: " + wasRecipesOpened);
        return wasOrdersOpened && wasRecipesOpened;
    }

    // public void ToggleWeapons()
    // {
    //     mainHud.SetActive(weaponsTab.activeSelf);
    //     Time.timeScale = weaponsTab.activeSelf ? 1f : 0f;
    //     isMenuActive = !weaponsTab.activeSelf;
    //     inventoryTab.SetActive(false);
    //     ordersTab.SetActive(false);
    //     recipesTab.SetActive(false);
    //     weaponsTab.SetActive(!weaponsTab.activeSelf);
    //     shopTab.SetActive(false);
    //     creatureTab.SetActive(false);
    //     currentUiPage = (int)UIPage.WEAPONS;
    // }

    // public void ToggleShop()
    // {
    //     mainHud.SetActive(shopTab.activeSelf);
    //     Time.timeScale = shopTab.activeSelf ? 1f : 0f;
    //     isMenuActive = !shopTab.activeSelf;
    //     inventoryTab.SetActive(false);
    //     ordersTab.SetActive(false);
    //     recipesTab.SetActive(false);
    //     weaponsTab.SetActive(false);
    //     creatureTab.SetActive(false);
    //     shopTab.SetActive(!shopTab.activeSelf);
    //     currentUiPage = (int)UIPage.SHOP;
    // }

    // public void ToggleCreatures()
    // {
    //     mainHud.SetActive(creatureTab.activeSelf);
    //     Time.timeScale = creatureTab.activeSelf ? 1f : 0f;
    //     isMenuActive = !creatureTab.activeSelf;
    //     inventoryTab.SetActive(false);
    //     ordersTab.SetActive(false);
    //     recipesTab.SetActive(false);
    //     weaponsTab.SetActive(false);
    //     creatureTab.SetActive(!creatureTab.activeSelf);
    //     shopTab.SetActive(false);
    //     currentUiPage = (int)UIPage.CREATURES;
    // }

    public void CloseMenu()
    {
        mainHud.SetActive(true);
        Time.timeScale = 1f;
        isMenuActive = false;
        inventoryTab.SetActive(false);
        ordersTab.SetActive(false);
        recipesTab.SetActive(false);
        // weaponsTab.SetActive(false);
        // shopTab.SetActive(false);
        // creatureTab.SetActive(false);
    }

    public void OpenCampfireInterface()
    {
        mainHud.SetActive(true);
        Time.timeScale = 0f;
        isMenuActive = true;
        inventoryTab.SetActive(false);
        ordersTab.SetActive(false);
        recipesTab.SetActive(true);
        // weaponsTab.SetActive(false);
        // shopTab.SetActive(false);
        // creatureTab.SetActive(false);
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
        // weaponsTab.SetActive(false);
        // shopTab.SetActive(false);
        // creatureTab.SetActive(false);
    }

    // Remembers the current player interactable for interaction
    public void SetPlayerInteractable(PlayerInteractable interactable)
    {
        Debug.Log("added player interactable: " + interactable.name);
        currentInteractables.Add(interactable);
    }

    // Clears the current interactable if possible
    public void ClearPlayerInteractable(PlayerInteractable interactable)
    {
        Debug.Log("removed player interactable: " + interactable.name);
        currentInteractables.Remove(interactable);
    }

    // Clears all interactables except one
    // Also triggers leaving for each player interactable
    public void ClearPlayerInteractablesButOne(PlayerInteractable interactable)
    {
        // Need to copy to another list before triggering leave
        // Otherwise triggering leave would modify the list
        List<PlayerInteractable> oldInteractables = new List<PlayerInteractable>();
        foreach (PlayerInteractable pi in currentInteractables)
        {
            if (pi != interactable)
            {
                oldInteractables.Add(pi);
            }
        }
        currentInteractables = new List<PlayerInteractable> { interactable };
        foreach (PlayerInteractable pi in oldInteractables)
        {
            pi.ForceExit();
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
        // WeaponSkillItem primaryWeapon = DatabaseLoader.GetWeaponSkillById(PlayerManager.instance.currentWeaponHeld);
        // WeaponSkillItem secondarySkill = DatabaseLoader.GetWeaponSkillById(PlayerManager.instance.currentSecondaryHeld);
        // primaryAttack.sprite = primaryWeapon.icon;
        // secondaryAttack.sprite = secondarySkill.icon;
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
            InventoryManager.instance.ForceUIUpdate();
        }
        if (RecipeManager.instance != null)
        {
            RecipeManager.instance.ForceUIUpdate();
        }
        if (OrdersManager.instance != null)
        {
            OrdersManager.instance.ForceUIUpdate();
        }
        // if (WeaponManager.instance != null)
        // {
        //     WeaponManager.instance.UpdateWeaponSkillStats();
        // }
        TutorialUIController.instance.UpdateFixedHUD();
    }

    private void Update()
    {
        anyUIActive = isMenuActive
                || isPaused
                || endOfDayMenu.activeSelf;

        if (anyUIActive != anyUIWasActive)
        {
            anyUIWasActive = anyUIActive;
            HandleUIActiveChange(anyUIActive);
        }

        if (isDialogueOpen)
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
            TutorialUIController.instance.ToggleInventory();
        }
        else if (Input.GetKeyDown(ToggleOrdersKeyCode))
        {
            TutorialUIController.instance.ToggleOrders();
        }
        else if (Input.GetKeyDown(ToggleRecipesKeyCode))
        {
            TutorialUIController.instance.ToggleRecipes();
        }

        if (Input.GetKeyDown(campfireActionKeyCode))
        {
            HandleCampfireAction();
        }

        if (!isMenuActive)
        {
            // Exit the truck if possible
            // if (Input.GetKeyDown(interactKeyCode) && isPlayerInVehicle)
            // {
            //     DrivingManager.instance.HandlePlayerLeaveVehicle(false);
            // }

            // Toggle interactable if menu is not active
            if (Input.GetKeyDown(interactKeyCode) && currentInteractables.Count > 0)
            {
                // Interact with the top of the interactables stack
                currentInteractables[currentInteractables.Count - 1].OnPlayerInteract();
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

        if (ordersTab.activeSelf)
        {
            wasOrdersOpened = true;
        }

        if (recipesTab.activeSelf)
        {
            wasRecipesOpened = true;
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
                // case (int)UIPage.WEAPONS:
                // TODO
                //     break;
                // // case (int)UIPage.SHOP:
                //     ShopManager.instance.HandlePurchase();
                //     break;
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
                // case (int)UIPage.WEAPONS:
                //     ToggleWeapons();
                //     break;
                // case (int)UIPage.SHOP:
                //     ToggleShop();
                //     break;
                // case (int)UIPage.CREATURES:
                //     ToggleCreatures();
                //     break;
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

    public void QuitToMainMenu()
    {
        isPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene((int)SceneIndexes.MAIN_MENU);
    }

    public void ExitGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        Application.Quit();
    }
}
