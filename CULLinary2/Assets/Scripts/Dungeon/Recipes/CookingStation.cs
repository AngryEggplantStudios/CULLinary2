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
    public override void OnPlayerInteract()
    {
        // Open Cooking Menu
        if (!RecipeManager.instance.IsCookingActivated())
        {
            ShopManager.instance.UpdateShop();
            RecipeManager.instance.ActivateCooking();
            UIController.instance.OpenCampfireInterface();
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
            UIController.instance.CloseCampfireInterface();
        }
    }
}
