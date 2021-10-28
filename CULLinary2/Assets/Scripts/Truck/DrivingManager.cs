using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Handles the player entering and leaving vehicle
public class DrivingManager : SingletonGeneric<DrivingManager>
{
    [Header("Items to Activate")]
    [SerializeField] private Camera truckCamera;
    [SerializeField] private GameObject driveableTruck;
    [SerializeField] private GameObject truckAudio;
    [Header("Items to Hide")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private GameObject staminaIcon;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject responsiveUICanvas;
    [SerializeField] private GameObject minimapIcon;
    [SerializeField] private GameObject ordersMapIcon;
    [Header("For Collision and Drowning")]
    [SerializeField] private AudioSource collisionAudioSource;
    // Threshhold is 1500 m/s-2
    // At the threshhold, damage taken is 100 damage
    public float accelToDamageRatio = 0.06666667f;
    // The height below the ground where player will be kicked out
    public float minimumHeightToStartDrowning = -5.0f;
    [Header("UI References")]
    [SerializeField] private GameObject warningPrefab;
    [SerializeField] private GameObject truckCanvas;

    // 3.1 is manually determined, the half width of truck in MainScene
    private float truckEdgeFromCentre = 3.1f;
    private float maxRaycastDistance = 10.0f;

    private Vector3 spawnOffset = Vector3.right * 5;
    private Vector3 rightEdgeOfTruck;
    private bool isPlayerInVehicle = false;
    private bool wasStaminaIconActivePreviously = false;

    // Show error message when trying to eat something
    private KeyCode consumableOneKeyCode;
    private KeyCode consumableTwoKeyCode;
    private KeyCode consumableThreeKeyCode;
    private KeyCode consumableFourKeyCode;
    private KeyCode consumableFiveKeyCode;

    void Start()
    {
        rightEdgeOfTruck = Vector3.right * truckEdgeFromCentre;
        driveableTruck.GetComponent<CarController>().AddOnCollisionAction(decel =>
        {
            collisionAudioSource.Play();
            HandlePlayerLeaveVehicle(true);
            PlayerHealth.instance.HandleHit(decel * accelToDamageRatio);
        });

        consumableOneKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable1); //Health (Red Pill) 
        consumableTwoKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable2); //Stamina (Blue pill)
        consumableThreeKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable3); //Health + Stamina(Potion)
        consumableFourKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable4); //Crit & Evasion(Pfizer)
        consumableFiveKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable5); //Attack(Moderna)
    }

    void Update()
    {
        if (isPlayerInVehicle)
        {
            player.transform.position = driveableTruck.transform.position;
            if (driveableTruck.transform.position.y < minimumHeightToStartDrowning)
            {
                HandlePlayerLeaveVehicle(true);
            }

            if ((Input.GetKeyDown(consumableOneKeyCode) && PlayerManager.instance.healthPill > 0) ||
                (Input.GetKeyDown(consumableTwoKeyCode) && PlayerManager.instance.staminaPill > 0) ||
                (Input.GetKeyDown(consumableThreeKeyCode) && PlayerManager.instance.potion > 0) ||
                (Input.GetKeyDown(consumableFourKeyCode) && PlayerManager.instance.pfizerShot > 0) ||
                (Input.GetKeyDown(consumableFiveKeyCode) && PlayerManager.instance.modernaShot > 0))
            {
                SpawnWarningMessage("Don't drink and drive!");
            }
        }
    }

    public void HandlePlayerEnterVehicle()
    {
        truckAudio.SetActive(true);
        driveableTruck.GetComponent<CarController>().enabled = true;
        truckCamera.gameObject.SetActive(true);
        player.SetActive(false);
        interactionPrompt.SetActive(false);
        responsiveUICanvas.SetActive(false);
        minimapIcon.SetActive(false);
        ordersMapIcon.SetActive(false);
        isPlayerInVehicle = true;
        UIController.instance.isPlayerInVehicle = true;

        // Hide stamina when driving
        wasStaminaIconActivePreviously = staminaIcon.activeSelf;
        staminaIcon.SetActive(false);
    }

    // Makes the player leave the vehicle
    // 
    // If skipChecks is set to true, player will leave
    // regardless of the vehicle's speed or location
    public void HandlePlayerLeaveVehicle(bool skipChecks)
    {
        CarController truckController = driveableTruck.GetComponent<CarController>();
        if (!skipChecks)
        {
            if (truckController.IsPastCollisionSpeed())
            {
                SpawnWarningMessage("Moving too fast!");
                return;
            }

            Vector3 playerOffset = driveableTruck.transform.rotation * spawnOffset;
            Vector3 newPlayerPos = driveableTruck.transform.position;
            RaycastHit hit;
            if (Physics.Raycast(driveableTruck.transform.position, playerOffset, out hit, maxRaycastDistance, LayerMask.GetMask("Environment")))
            {
                SpawnWarningMessage("Can't get out safely!");
                return;
            }
        }

        truckAudio.SetActive(false);
        truckController.enabled = false;
        truckCamera.gameObject.SetActive(false);
        player.transform.position = driveableTruck.transform.position + driveableTruck.transform.rotation * spawnOffset;
        player.SetActive(true);
        interactionPrompt.SetActive(true);
        staminaIcon.SetActive(wasStaminaIconActivePreviously);
        responsiveUICanvas.SetActive(true);
        minimapIcon.SetActive(true);
        ordersMapIcon.SetActive(true);
        isPlayerInVehicle = false;
        UIController.instance.isPlayerInVehicle = false;
    }

    // Checks if the player is driving the truck
    public bool IsPlayerInVehicle()
    {
        return isPlayerInVehicle;
    }

    // Returns the y-axis rotation of the truck
    public float GetTruckYRotation()
    {
        return driveableTruck.transform.eulerAngles.y;
    }

    // Returns the transform of the truck
    public Transform GetTruckTransform()
    {
        return driveableTruck.transform;
    }

    private void SpawnWarningMessage(string message)
    {
        GameObject warning = Instantiate(warningPrefab);
        warning.transform.GetComponentInChildren<Text>().text = message.ToString();
        warning.transform.SetParent(truckCanvas.transform);
        warning.transform.position = truckCamera.WorldToScreenPoint(driveableTruck.transform.position);
    }
}
