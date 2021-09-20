using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A sphere to check whether the player is within the range
// Note that these spheres should not overlap!
public class SpherePlayerCollider : MonoBehaviour
{    
    // Assumes player is not within the collider when game starts
    bool playerWithinRange = false;
    // The PlayerInteractable which is the parent of this collider
    private PlayerInteractable parentInteractable = null;

    public bool PlayerWithinRange()
    {
        return playerWithinRange;
    }

    public void SetPlayerInteractable(PlayerInteractable parent)
    {
        parentInteractable = parent;
    }

    private void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.tag == "Player" && !playerWithinRange)
        {
            playerWithinRange = true;
            UIController.instance.SetPlayerInteractable(parentInteractable);
        }
    }

    private void OnTriggerExit(Collider otherObject)
    {
        if (otherObject.tag == "Player" && playerWithinRange)
        {
            playerWithinRange = false;
            UIController.instance.TriggerLeaveAndClearPlayerInteractable();
        }
    }
}
