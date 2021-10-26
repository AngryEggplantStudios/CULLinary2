using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles the player entering and leaving vehicle
public class DrivingManager : SingletonGeneric<DrivingManager>
{
    public GameObject driveableTruck;
    public GameObject truckAudio;
    public GameObject truckCamera;
    public GameObject interactionPrompt;
    public GameObject staminaIcon;
    public GameObject player;
    public GameObject responsiveUICanvas;
    [Header("For Collision")]
    public AudioSource collisionAudioSource;

    // Threshhold is 1500 m/s-2
    // At the threshhold, damage taken is 100 damage
    public float accelToDamageRatio = 0.06666667f;

    private Vector3 spawnOffset = Vector3.left * 5;
    private bool isPlayerInVehicle = false;
    private bool wasStaminaIconActivePreviously = false;

    void Start()
    {
        driveableTruck.GetComponent<CarController>().AddOnCollisionAction(decel => {
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
        truckCamera.SetActive(true);
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
        truckAudio.SetActive(false);
        driveableTruck.GetComponent<CarController>().enabled = false;
        truckCamera.SetActive(false);
        player.transform.position = driveableTruck.transform.position + spawnOffset;
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
}
