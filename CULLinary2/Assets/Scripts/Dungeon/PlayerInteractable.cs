using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInteractable : MonoBehaviour
{
    private bool hasSetColliderParent = false;

    void Update()
    {
        if (!hasSetColliderParent && BiomeGeneratorManager.IsGenerationComplete())
        {
            GetCollider().SetPlayerInteractable(this);
            hasSetColliderParent = true;
        }
    }

    public bool PlayerWithinRange()
    {
        return GetCollider().PlayerWithinRange();
    }

    // Gets the sphere player collider of the object
    public abstract SpherePlayerCollider GetCollider();

    // Called when player is in range
    public abstract void OnPlayerEnter();

    // Called when player is in range and interacts with this object
    public abstract void OnPlayerInteract();

    // Called when player has interacted with the object and goes out of range
    public abstract void OnPlayerLeave();
}
