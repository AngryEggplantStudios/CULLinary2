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
    [Header("Truck Dimensions")]
    // World space radius of truck, 10.0 is manually determined
    // Used for summoning truck
    [SerializeField] private float truckDiameter = 10.0f;
    // 3.1 is manually determined, the half width of truck in MainScene
    [SerializeField] private float truckEdgeFromCentre = 3.1f;
    [Header("Exit Truck")]
    // Distance to check when player exits
    [SerializeField] private float maxRaycastDistance = 10.0f;
    // Eject player 5 units to the right
    [SerializeField] private Vector3 playerSpawnOffset = new Vector3(5, 0, 0);
    // To spawn the player on the ground
    [SerializeField] private Collider groundCollider;
    [Header("Spawn Truck")]
    // Number of units to spawn the truck away from the player
    [SerializeField] private float unitsFromPlayer = 5.0f;
    // For truck spawning warning messages
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject playerCanvas;
    // Prevent trails moving when summoning truck
    [SerializeField] private TrailRenderer[] hideTrailsWhenSummoning;

    private Vector3 rightEdgeOfTruck;
    private bool isPlayerInVehicle = false;
    private bool wasStaminaIconActivePreviously = false;

    // Show error message when trying to eat something
    private KeyCode consumableOneKeyCode;
    private KeyCode consumableTwoKeyCode;
    private KeyCode consumableThreeKeyCode;
    private KeyCode consumableFourKeyCode;
    private KeyCode consumableFiveKeyCode;
    // Resummon the truck
    private KeyCode truckSummonKeycode;
    // Delay for resummoning
    private float delay = 0.0f;

    void Start()
    {
        rightEdgeOfTruck = Vector3.right * truckEdgeFromCentre;
        driveableTruck.GetComponent<CarController>().AddOnCollisionAction(decel =>
        {
            collisionAudioSource.Play();
            HandlePlayerLeaveVehicle(true);
            PlayerHealth.instance.HandleHit(decel * accelToDamageRatio);
        });

        consumableOneKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable1);   //Health (Red Pill) 
        consumableTwoKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable2);   //Stamina (Blue pill)
        consumableThreeKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable3); //Health + Stamina(Potion)
        consumableFourKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable4);  //Crit & Evasion(Pfizer)
        consumableFiveKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Consumable5);  //Attack(Moderna)

        // Temporary
        truckSummonKeycode = KeyCode.T;
    }

    void Update()
    {
        delay = Mathf.Max(delay - Time.deltaTime, 0.0f);

        if (isPlayerInVehicle)
        {
            player.transform.position = driveableTruck.transform.position;
            if (driveableTruck.transform.position.y < minimumHeightToStartDrowning)
            {
                HandlePlayerLeaveVehicle(true);
            }

            if (!UIController.instance.isMenuActive && !UIController.instance.isPaused &&
                ((Input.GetKeyDown(consumableOneKeyCode) && PlayerManager.instance.healthPill > 0) ||
                (Input.GetKeyDown(consumableTwoKeyCode) && PlayerManager.instance.staminaPill > 0) ||
                (Input.GetKeyDown(consumableThreeKeyCode) && PlayerManager.instance.potion > 0) ||
                (Input.GetKeyDown(consumableFourKeyCode) && PlayerManager.instance.pfizerShot > 0) ||
                (Input.GetKeyDown(consumableFiveKeyCode) && PlayerManager.instance.modernaShot > 0)))
            {
                SpawnWarningMessage("Don't drink and drive!");
            }
        }
        else if (!UIController.instance.isMenuActive && !UIController.instance.isPaused &&
                 Input.GetKeyDown(truckSummonKeycode) && PlayerManager.instance.isTruckUnlocked)
        {
            if (delay > 0.0f)
            {
                SpawnWarningMessagePlayer("Try again later");
            }
            else
            {
                if (!TryTruckSummon())
                {
                    SpawnWarningMessagePlayer("Not enough space!");
                }
                // Set delay to half a second
                delay = 0.5f;
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

            Vector3 playerOffset = driveableTruck.transform.rotation * playerSpawnOffset;
            Vector3 newPlayerPos = driveableTruck.transform.position;
            if (Physics.Raycast(driveableTruck.transform.position, playerOffset, out _, maxRaycastDistance, LayerMask.GetMask("Environment")))
            {
                SpawnWarningMessage("Can't get out safely!");
                return;
            }
        }
        Vector3 exitPlayerPos = driveableTruck.transform.position + driveableTruck.transform.rotation * playerSpawnOffset;
        // 20.0f is above the ground for sure
        Vector3 aboveExitPos = new Vector3(exitPlayerPos.x, 20.0f, exitPlayerPos.z);
        // Find ground Y by raycast
        RaycastHit hit;
        if (!groundCollider.Raycast(new Ray(aboveExitPos, Vector3.down), out hit, Mathf.Infinity))
        {
            SpawnWarningMessage("No ground to land on!");
            return;
        }

        truckAudio.SetActive(false);
        truckController.enabled = false;
        truckCamera.gameObject.SetActive(false);
        player.transform.position = new Vector3(exitPlayerPos.x, hit.point.y, exitPlayerPos.z);
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

    public bool TryTruckSummon()
    {
        Vector3 positionLeft = player.transform.position + Vector3.left * unitsFromPlayer;
        Vector3 positionForward = player.transform.position + Vector3.forward * unitsFromPlayer;
        Vector3 positionRight = player.transform.position + Vector3.right * unitsFromPlayer;
        Vector3 positionBack = player.transform.position + Vector3.back * unitsFromPlayer;

        Vector3[] possiblePositions = new Vector3[]{
            positionLeft,
            positionForward,
            positionRight,
            positionBack
        };

        foreach (Vector3 possiblePos in possiblePositions)
        {
            if (!Physics.CheckSphere(possiblePos, truckDiameter, LayerMask.GetMask("Environment")))
            {
                if (!driveableTruck.activeSelf)
                {
                    driveableTruck.SetActive(true);
                    driveableTruck.GetComponent<VehicleInteractable>().ManuallySetPlayerInteractable();
                }

                driveableTruck.GetComponent<CarController>().ResetCarMotion();
                driveableTruck.transform.position = possiblePos;
                foreach (TrailRenderer tr in hideTrailsWhenSummoning)
                {
                    tr.Clear();
                }
                return true;
            }
        }
        return false;
    }

    private void SpawnWarningMessage(string message)
    {
        GameObject warning = Instantiate(warningPrefab);
        warning.transform.GetComponentInChildren<Text>().text = message.ToString();
        warning.transform.SetParent(truckCanvas.transform);
        warning.transform.position = truckCamera.WorldToScreenPoint(driveableTruck.transform.position);
    }

    private void SpawnWarningMessagePlayer(string message)
    {
        GameObject warning = Instantiate(warningPrefab);
        warning.transform.GetComponentInChildren<Text>().text = message.ToString();
        warning.transform.SetParent(playerCanvas.transform);
        warning.transform.position = playerCamera.WorldToScreenPoint(player.transform.position);
    }
}
