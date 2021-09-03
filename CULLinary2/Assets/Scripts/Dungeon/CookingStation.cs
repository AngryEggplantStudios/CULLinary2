using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingStation : PlayerInteractable
{
    public SpherePlayerCollider collider;

    private bool isCooking = false;

    public override SpherePlayerCollider GetCollider()
    {
        return collider;
    }

    public override void OnPlayerInteract(GameObject player)
    {
        // Open Cooking Menu
        if (!isCooking)
        {
            // uiController.ShowCookingPanel();
            //Debug.Log("why inventory NO SHOW UP on first F");
            // DisableMovementOfPlayer(); // Disable movement of player when menu is open
            isCooking = true;
            Debug.Log("cooking");
            PlayerRecipeBook recipes = player.GetComponent<PlayerRecipeBook>();
            PlayerPickup inventory = player.GetComponent<PlayerPickup>();
            recipes.OpenRecipeBook();
            inventory.OpenInventory();
        }
    }

    public override void OnPlayerLeave(GameObject player)
    {
        // Stop cooking anim if player walks away halfway
        // Probably can remove this once we let player stop moving when cooking
        // (we'll leave it for now just in case)
        if (isCooking) 
        {
            isCooking = false;
        }
    }
}
