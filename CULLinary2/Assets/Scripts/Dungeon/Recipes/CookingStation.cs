using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingStation : PlayerInteractable
{
    public SpherePlayerCollider spCollider;

    public void Awake()
    {
        // Add to the list in RecipeManager for use in the minimap
        RecipeManager.instance.AddCampfire(this.GetComponent<Transform>());
    }

    public override SpherePlayerCollider GetCollider()
    {
        return spCollider;
    }

    public override void OnPlayerEnter()
    {
        // Start healing
        PlayerHealth.instance.OnPlayerEnterCampfire();
        // Set this as last visited campfire
        PlayerSpawnManager.instance.SetLastVisitedCampfire(transform);
        // Update Campfire UIs
        UIController.instance.OnPlayerEnterCampfire();
        ShopManager.instance.UpdateShop(); // TODO: Should be removed when UI is merged
        // Activate cooking
        if (!RecipeManager.instance.IsCookingActivated())
        {
            RecipeManager.instance.ActivateCooking();
        }
    }
    
    public override void OnPlayerInteract()
    {
        // Open Cooking Menu
        if (!UIController.instance.isMenuActive)
        {
            UIController.instance.OpenCampfireInterface();
        }
        // Close Cooking Menu
        else
        {
            UIController.instance.CloseCampfireInterface();
        }
    }

    public override void OnPlayerLeave()
    {
        // Stop cooking anim if player walks away halfway
        // Probably can remove this once we let player stop moving when cooking
        // (we'll leave it for now just in case)
        if (RecipeManager.instance.IsCookingActivated())
        {
            RecipeManager.instance.DeactivateCooking();
        }
        // Stop healing
        PlayerHealth.instance.OnPlayerLeaveCampfire();
        // Set back to normal menu
        UIController.instance.OnPlayerLeaveCampfire();
    }
}
