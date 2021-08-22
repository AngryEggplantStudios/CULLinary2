using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingStation : MonoBehaviour
{
    public SpherePlayerCollider collider;

    private bool isCooking = false;

    void Update()
    {
        // Stop cooking anim if player walks away halfway
        // Probably can remove this once we let player stop moving when cooking
        // (we'll leave it for now just in case)
        if (isCooking && !PlayerWithinRange()) 
        {
            isCooking = false;
        }

        // Open Cooking Menu
        if (!isCooking && PlayerKeybinds.WasTriggered(KeybindAction.Interact) && PlayerWithinRange())
        {
            // uiController.ShowCookingPanel();
            //Debug.Log("why inventory NO SHOW UP on first F");
            // DisableMovementOfPlayer(); // Disable movement of player when menu is open
            isCooking = true;
            Debug.Log("cooking");
        }
    }

    public bool PlayerWithinRange()
    {
        return collider.PlayerWithinRange();
    }
}
