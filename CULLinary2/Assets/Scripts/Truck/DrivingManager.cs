using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Handles the player entering and leaving vehicle
public class DrivingManager : SingletonGeneric<DrivingManager>
{
    public Camera truckCamera;
    public GameObject driveableTruck;
    public GameObject truckAudio;
    public GameObject interactionPrompt;
    public GameObject staminaIcon;
    public GameObject player;
    public GameObject responsiveUICanvas;
    [Header("For Collision")]
    [SerializeField] private AudioSource collisionAudioSource;
    // Threshhold is 1500 m/s-2
    // At the threshhold, damage taken is 100 damage
    public float accelToDamageRatio = 0.06666667f;
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

    void Start()
    {
        rightEdgeOfTruck = Vector3.right * truckEdgeFromCentre;
        driveableTruck.GetComponent<CarController>().AddOnCollisionAction(decel =>
        {
            collisionAudioSource.Play();
            HandlePlayerLeaveVehicle();
            PlayerHealth.instance.HandleHit(decel * accelToDamageRatio);
        });
    }

    void Update()
    {
        if (isPlayerInVehicle)
        {
            player.transform.position = driveableTruck.transform.position;
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
        isPlayerInVehicle = true;
        UIController.instance.isPlayerInVehicle = true;

        // Hide stamina when driving
        wasStaminaIconActivePreviously = staminaIcon.activeSelf;
        staminaIcon.SetActive(false);
    }

    public void HandlePlayerLeaveVehicle()
    {
        Vector3 playerOffset = driveableTruck.transform.rotation * spawnOffset;
        Vector3 newPlayerPos = driveableTruck.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(driveableTruck.transform.position, playerOffset, out hit, maxRaycastDistance, LayerMask.GetMask("Environment")))
        {
            SpawnWarningMessage("Can't get out safely!");
            return;
        }

        truckAudio.SetActive(false);
        driveableTruck.GetComponent<CarController>().enabled = false;
        truckCamera.gameObject.SetActive(false);
        player.transform.position = driveableTruck.transform.position + driveableTruck.transform.rotation * spawnOffset;
        player.SetActive(true);
        interactionPrompt.SetActive(true);
        staminaIcon.SetActive(wasStaminaIconActivePreviously);
        responsiveUICanvas.SetActive(true);
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
