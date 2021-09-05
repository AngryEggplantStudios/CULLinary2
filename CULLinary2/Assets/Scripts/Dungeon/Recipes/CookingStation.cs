using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingStation : PlayerInteractable
{
    public SpherePlayerCollider spCollider;
    
    private bool hasCached = false;
    private PlayerRecipeBook recipeBook;
    private PlayerPickup inventory;
    private RecipeManager recipeManager;

    void Start()
    {
        recipeManager = RecipeManager.Instance;
    }

    public override SpherePlayerCollider GetCollider()
    {
        return spCollider;
    }
    public override void OnPlayerInteract(GameObject player)
    {
        CacheReferences(player);

        // Open Cooking Menu
        if (!recipeManager.IsCookingActivated())
        {
            // uiController.ShowCookingPanel();
            //Debug.Log("why inventory NO SHOW UP on first F");
            // DisableMovementOfPlayer(); // Disable movement of player when menu is open
            Debug.Log("Opening cooking menus");
            recipeManager.ActivateCooking();
            recipeBook.OpenRecipeBook();
            inventory.OpenInventory();
        }
    }

    public override void OnPlayerLeave(GameObject player)
    {
        CacheReferences(player);

        // Stop cooking anim if player walks away halfway
        // Probably can remove this once we let player stop moving when cooking
        // (we'll leave it for now just in case)
        if (recipeManager.IsCookingActivated()) 
        {
            recipeManager.DeactivateCooking();
            recipeBook.UpdateUI();
        }
    }

    // Helper function to cache useful references
    private void CacheReferences(GameObject player)
    {
        if (!hasCached)
        {            
            recipeBook = player.GetComponent<PlayerRecipeBook>();
            inventory = player.GetComponent<PlayerPickup>();
            hasCached = true;
        }
    }
}
