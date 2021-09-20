using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInteractable : MonoBehaviour
{
    private KeybindAction interactAction;
    private GameObject player;
    private bool hasInteracted = false;

    // Keep track of game object failures
    // for the error message
    private const int findGameObjectFailureLimit = 10;
    private int numberOfFailures = 0;

    void Awake()
    {
        interactAction = KeybindAction.Interact;
        hasInteracted = false;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (!BiomeGeneratorManager.IsGenerationComplete())
        {
            return;
        }

        if (player == null && numberOfFailures < findGameObjectFailureLimit)
        {
            // Sometimes, it does not work in Awake or Start
            // So finding the game object has to be done here
            player = GameObject.FindGameObjectWithTag("Player");

            numberOfFailures++;
            if (numberOfFailures == findGameObjectFailureLimit)
            {
                Debug.Log("PlayerInteractable.cs: Could not find Player in scene");
            }
        }

        if (hasInteracted && !PlayerWithinRange())
        {
            hasInteracted = false;
            OnPlayerLeave(player);

        }

        if (!hasInteracted && PlayerKeybinds.WasTriggered(interactAction) && PlayerWithinRange())
        {
            hasInteracted = true;
            OnPlayerInteract(player);
        }
    }

    private bool PlayerWithinRange()
    {
        return GetCollider().PlayerWithinRange();
    }

    // Gets the sphere player collider of the object
    public abstract SpherePlayerCollider GetCollider();

    // Called when player is in range and interacts with this object
    public abstract void OnPlayerInteract(GameObject player);

    // Called when player has interacted with the object and goes out of range
    public abstract void OnPlayerLeave(GameObject player);
}
