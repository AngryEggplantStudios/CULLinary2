using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCampfire : PlayerInteractable
{
    public SpherePlayerCollider spCollider;

    static List<GameObject> campfires = new List<GameObject>();

    public void Awake()
    {
        // Add to the list in RecipeManager for use in the minimap
        TutorialRecipeManager.instance.AddCampfire(this.GetComponent<Transform>());
        campfires.Add(this.gameObject);
    }

    public static List<GameObject> GetAll()
    {
        return campfires;
    }

    public override SpherePlayerCollider GetCollider()
    {
        return spCollider;
    }

    public override void OnPlayerEnter()
    {
        // Start healing
        // PlayerHealth.instance.OnPlayerEnterCampfire();
        // Set this as last visited campfire
        // PlayerSpawnManager.instance.SetLastVisitedCampfire(transform);
        // Update Campfire UI
        Debug.Log("enter campfire");
        TutorialUIController.instance.OnPlayerEnterCampfire();
        // ShopManager.instance.UpdateShop(); // TODO: Should be removed when UI is merged
        // Activate cooking
        // if (!RecipeManager.instance.IsCookingActivated())
        // {
        //     RecipeManager.instance.ActivateCooking();
        // }
    }

    public override void OnPlayerInteract()
    {
        Debug.Log("interact with campfire");
        // Open Cooking Menu
        if (!TutorialUIController.instance.isMenuActive)
        {
            TutorialUIController.instance.OpenCampfireInterface();
        }
        // Close Cooking Menu
        else
        {
            TutorialUIController.instance.CloseCampfireInterface();
        }
    }

    public override void OnPlayerLeave()
    {
        Debug.Log("leave campfire");
        // Stop cooking anim if player walks away halfway
        // Probably can remove this once we let player stop moving when cooking
        // (we'll leave it for now just in case)
        // if (RecipeManager.instance.IsCookingActivated())
        // {
        //     RecipeManager.instance.DeactivateCooking();
        // }
        // Stop healing
        // PlayerHealth.instance.OnPlayerLeaveCampfire();
        // Set back to normal menu
        TutorialUIController.instance.OnPlayerLeaveCampfire();
    }
}

