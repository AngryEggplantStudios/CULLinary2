using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInteractable : MonoBehaviour
{
    private KeybindAction interactAction;
    private GameObject player;
    private bool hasInteracted = false;

    void Awake()
    {
        interactAction = KeybindAction.Interact;
        player = GameObject.FindGameObjectWithTag("Player");
        hasInteracted = false;
    }

    void Update()
    {
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
