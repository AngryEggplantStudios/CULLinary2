using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePlayerCollider : MonoBehaviour
{    
    // Assumes player is not within the collider when game starts
    bool playerWithinRange = false;

    public bool PlayerWithinRange()
    {
        return playerWithinRange;
    }

    private void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.tag == "Player")
        {
            playerWithinRange = true;
        }
    }

    private void OnTriggerExit(Collider otherObject)
    {
        if (otherObject.tag == "Player")
        {
            playerWithinRange = false;
        }
    }
}
