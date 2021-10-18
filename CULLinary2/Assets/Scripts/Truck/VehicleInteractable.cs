using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleInteractable : PlayerInteractable
{
    public SpherePlayerCollider spCollider;
    public GameObject truckCamera;
    public GameObject playerCamera;
    public GameObject driveableTruck;
    // Hide this light as it is annoying
    public GameObject playerPointLight;
    // Hide the interaction prompt as well 
    public GameObject interactionPrompt;

    private GameObject player;
    private bool isPlayerInVehicle = false;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void Update()
    {
        base.Update();
        if (isPlayerInVehicle)
        {
            player.transform.position = driveableTruck.transform.position;
            player.transform.rotation = driveableTruck.transform.rotation;
        }
    }

    public override SpherePlayerCollider GetCollider()
    {
        return spCollider;
    }

    public override void OnPlayerEnter()
    { }
    
    public override void OnPlayerInteract()
    {
        driveableTruck.GetComponent<CarController>().enabled = true;
        truckCamera.SetActive(true);
        playerCamera.SetActive(false);
        playerPointLight.SetActive(false);
        interactionPrompt.SetActive(false);

        // player.GetComponent<Animator>().enabled = false;
        // player.GetComponent<CharacterController>().enabled = false;
        // player.GetComponent<BoxCollider>().enabled = false;
        // player.GetComponent<PlayerController>().enabled = false;
        // player.GetComponent<PlayerMelee>().enabled = false;
        // player.GetComponent<PlayerDash>().enabled = false;
        // player.GetComponent<PlayerSlash>().enabled = false;
        // player.GetComponent<PlayerSecondaryAttack>().enabled = false;
        // player.GetComponent<Rigidbody>().freezeRotation = true;
        // player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        player.SetActive(false);

        isPlayerInVehicle = true;
    }

    public override void OnPlayerLeave()
    { }
}
