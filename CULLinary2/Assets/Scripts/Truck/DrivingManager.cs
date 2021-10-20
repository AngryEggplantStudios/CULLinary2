using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles the player entering and leaving vehicle
public class DrivingManager : SingletonGeneric<DrivingManager>
{
    public GameObject driveableTruck;
    public GameObject truckCamera;
    // Hide the interaction prompt as well 
    public GameObject interactionPrompt;

    private GameObject player;
    private bool hasSetPlayer = false;
    private bool isPlayerInVehicle = false;
    private Vector3 spawnOffset = Vector3.left * 5;
    
    public void Update()
    {
        if (!hasSetPlayer)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            hasSetPlayer = player != null;
        }
        if (isPlayerInVehicle)
        {
            player.transform.position = driveableTruck.transform.position;
            player.transform.rotation = driveableTruck.transform.rotation;
        }
    }

    public void HandlePlayerEnterVehicle()
    {
        driveableTruck.GetComponent<CarController>().enabled = true;
        truckCamera.SetActive(true);
        player.SetActive(false);
        interactionPrompt.SetActive(false);
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
        isPlayerInVehicle = false;
        UIController.instance.isPlayerInVehicle = false;
    }
}
