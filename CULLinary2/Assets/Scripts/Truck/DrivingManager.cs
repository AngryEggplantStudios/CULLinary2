using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles the player entering and leaving vehicle
public class DrivingManager : SingletonGeneric<DrivingManager>
{
    public GameObject driveableTruck;
    public GameObject truckCamera;
    public GameObject interactionPrompt;
    public GameObject player;
    public GameObject responsiveUICanvas;

    private Vector3 spawnOffset = Vector3.left * 5;
    private bool isPlayerInVehicle = false;

    void Update()
    {
        if (isPlayerInVehicle)
        {
            player.transform.position = driveableTruck.transform.position;
        }
    }

    public void HandlePlayerEnterVehicle()
    {
        driveableTruck.GetComponent<CarController>().enabled = true;
        truckCamera.SetActive(true);
        player.SetActive(false);
        interactionPrompt.SetActive(false);
        responsiveUICanvas.SetActive(false);
        isPlayerInVehicle = true;
        UIController.instance.isPlayerInVehicle = true;
    }

    public void HandlePlayerLeaveVehicle()
    {
        driveableTruck.GetComponent<CarController>().enabled = false;
        truckCamera.SetActive(false);
        player.transform.position = driveableTruck.transform.position + spawnOffset;
        player.SetActive(true);
        interactionPrompt.SetActive(true);
        responsiveUICanvas.SetActive(true);
        isPlayerInVehicle = false;
        UIController.instance.isPlayerInVehicle = false;
    }
}
