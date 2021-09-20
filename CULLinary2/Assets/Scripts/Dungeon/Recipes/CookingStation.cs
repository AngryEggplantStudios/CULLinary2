using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingStation : PlayerInteractable
{
    public SpherePlayerCollider spCollider;

    private RecipeManager recipeManager;
    private UIController uiController;

    void Start()
    {
        recipeManager = RecipeManager.Instance;
        uiController = UIController.instance;
    }

    public override SpherePlayerCollider GetCollider()
    {
        return spCollider;
    }
    public override void OnPlayerInteract()
    {
        // Open Cooking Menu
        if (!recipeManager.IsCookingActivated())
        {
            // uiController.ShowCookingPanel();
            //Debug.Log("why inventory NO SHOW UP on first F");
            // DisableMovementOfPlayer(); // Disable movement of player when menu is open
            Debug.Log("Opening cooking menus");
            recipeManager.ActivateCooking();
            uiController.OpenCampfireInterface();
        }
    }

    public override void OnPlayerLeave()
    {
        // Stop cooking anim if player walks away halfway
        // Probably can remove this once we let player stop moving when cooking
        // (we'll leave it for now just in case)
        if (recipeManager.IsCookingActivated())
        {
            recipeManager.DeactivateCooking();
            uiController.CloseCampfireInterface();
        }
    }
}
