using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeManager : SingletonGeneric<RecipeManager>
{
    [Header("For Recipes UI")]
    // Container to attach recipes to
    public GameObject recipesContainer;
    // Prefab of a recipe book entry
    public GameObject recipeSlot;
    // Large info display of the recipe
    public RecipeUIInfoDisplay infoDisplay;

    [Header("For Cooking UI")]
    public GameObject cookingRecipesContainer;
    public GameObject cookingUiSlot;
    public RecipeUIInfoDisplay cookingInfoDisplay;
    [Header("Item Pop-Up")]
    // For cooking, to play a sound and show a pop-up
    public PlayerPickup pickup;
    public AudioSource cookingSound;

    private List<Recipe> innerUnlockedRecipesList = new List<Recipe>();
    private List<Recipe> innerLockedRecipesList = new List<Recipe>();
    private bool isCooking = false;

    // Currently selected recipe in Cooking UI
    private Recipe currentRecipe = null;

    // Random number generator for recipes
    private System.Random rand = new System.Random();

    // Only populate the unlocked recipes list once
    private bool hasPopulatedUnlockedRecipes = false;
    // Keep track of changes in the recipe list for the UI
    private bool recipesChanged = false;
    // Save the currently selected index of recipes in the UIs
    private int currentlySelectedRecipeIndex = 0;
    private int currentlySelectedCookingIndex = 0;

    // List of all campfire locations for the minimap
    private List<Transform> campfires = new List<Transform>();


    void Start()
    {
        if (pickup == null)
        {
            Debug.Log("RecipeManager: Oops! Could not find PlayerPickup");
        }
    }

    // To be called when save data is loaded
    public void FilterUnlockedRecipes()
    {
        if (!hasPopulatedUnlockedRecipes)
        {
            UpdateUnlockedRecipes();
        }
    }

    // Update the unlocked and the locked recipes list
    private void UpdateUnlockedRecipes()
    {
        bool[] recipesUnlocked = PlayerManager.instance
            ? PlayerManager.instance.recipesUnlocked
            : new bool[3] { true, true, true };

        for (int id = 0; id < recipesUnlocked.Length; id++)
        {
            if (recipesUnlocked[id])
            {
                innerUnlockedRecipesList.Add(DatabaseLoader.GetRecipeById(id));
            }
            else
            {
                innerLockedRecipesList.Add(DatabaseLoader.GetRecipeById(id));
            }
        }
        hasPopulatedUnlockedRecipes = true;
        recipesChanged = true;
        StopCoroutine(UpdateUI());
        StartCoroutine(UpdateUI());
    }

    public Recipe GetRandomRecipe()
    {
        FilterUnlockedRecipes();
        int randomIndex = rand.Next(innerUnlockedRecipesList.Count);
        return innerUnlockedRecipesList[randomIndex];
    }

    public List<Recipe> GetUnlockedRecipes()
    {
        FilterUnlockedRecipes();
        return innerUnlockedRecipesList;
    }

    public void ActivateCooking()
    {
        isCooking = true;
    }

    public void DeactivateCooking()
    {
        isCooking = false;
        currentRecipe = null;
    }

    public bool IsCookingActivated()
    {
        return isCooking;
    }

    public void SetCurrentlyCookingRecipe(Recipe r)
    {
        currentRecipe = r;
    }

    public void CookCurrentlySelected()
    {
        if (currentRecipe == null)
        {
            Debug.Log("RecipeManager: No recipe selected!");
            return;
        }
        Recipe r = currentRecipe;

        if (!isCooking)
        {
            Debug.Log("RecipeManager: Not at campfire!");
            return;
        }

        if (InventoryManager.instance.RemoveIdsFromInventory(r.GetIngredientIds()))
        {
            // Sucessfully cooked!

            // UI will be updated in AddItem
            cookingInfoDisplay.IncreaseInventoryCountAndSetIngredients();
            InventoryManager.instance.AddItem(r.cookedDishItem);
            // Create a pop-up for the player
            pickup?.PickUp(r.cookedDishItem);
            // Player cooking sfx
            cookingSound.Play();
        }
        else
        {
            Debug.Log("RecipeManager: ingredients required!");
        }
    }

    public void AddCampfire(Transform fire)
    {
        campfires.Add(fire);
    }

    public List<Transform> GetAllCampfires()
    {
        return campfires;
    }

    public void SetCurrentlySelectedRecipeInMenuUi(int index)
    {
        currentlySelectedRecipeIndex = index;
    }

    public void SetCurrentlySelectedRecipeInCookingUi(int index)
    {
        currentlySelectedCookingIndex = index;
    }

    public IEnumerator UpdateUI()
    {
        // Wait until all orders have generated
        if (!OrdersManager.instance.IsOrderGenerationComplete())
        {
            yield break;
        }

        foreach (Transform child in recipesContainer.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in cookingRecipesContainer.transform)
        {
            Destroy(child.gameObject);
        }
        yield return null;

        Dictionary<int, int> ordersByRecipe = OrdersManager.instance.GetNumberOfOrdersByRecipe();
        int GetNumberOfOrders(int recipeId)
        {
            if (ordersByRecipe.ContainsKey(recipeId))
            {
                return ordersByRecipe[recipeId];
            }
            else
            {
                return 0;
            }
        }

        for (int i = 0; i < innerUnlockedRecipesList.Count; i++)
        {
            Recipe r = innerUnlockedRecipesList[i];
            List<(int, int)> ingIds = r.GetIngredientIds();
            List<(int, int, int)> invReqCount = new List<(int, int, int)>();
            bool areItemsInInventory =
                InventoryManager.instance.CheckIfItemsExist(ingIds, out invReqCount);
            int numberOfOrders = GetNumberOfOrders(r.recipeId);
            int numberInInventory = InventoryManager.instance.GetAmountOfItem(r.cookedDishItem.inventoryItemId);

            // Add to Recipes UI
            GameObject recipeEntry = Instantiate(recipeSlot,
                                                 new Vector3(0, 0, 0),
                                                 Quaternion.identity,
                                                 recipesContainer.transform) as GameObject;
            RecipeUISlot recipeDetails = recipeEntry.GetComponent<RecipeUISlot>();
            recipeDetails.AddRecipe(r, areItemsInInventory, invReqCount, numberOfOrders, numberInInventory, i);
            recipeDetails.SetInfoDisplay(infoDisplay);

            // Add to Cooking UI as well
            GameObject cookingUiEntry = Instantiate(cookingUiSlot,
                                                    new Vector3(0, 0, 0),
                                                    Quaternion.identity,
                                                    cookingRecipesContainer.transform) as GameObject;
            CookingUISlot cookingRecipeDetails = cookingUiEntry.GetComponent<CookingUISlot>();
            cookingRecipeDetails.AddRecipe(r, areItemsInInventory, invReqCount, numberOfOrders, numberInInventory, i);
            cookingRecipeDetails.SetInfoDisplay(cookingInfoDisplay);
            yield return null;
        }

        foreach (Recipe r in innerLockedRecipesList)
        {
            GameObject recipeEntry = Instantiate(recipeSlot,
                                                 new Vector3(0, 0, 0),
                                                 Quaternion.identity,
                                                 recipesContainer.transform) as GameObject;
            RecipeUISlot recipeDetails = recipeEntry.GetComponent<RecipeUISlot>();
            recipeDetails.AddUnknownRecipe(r);
            recipeDetails.SetInfoDisplay(infoDisplay);
            yield return null;
        }

        // Retain the same index for the display after updating, unless recipes list changed
        if (recipesChanged)
        {
            currentlySelectedRecipeIndex = 0;
            currentlySelectedCookingIndex = 0;
            recipesChanged = false;
        }
        recipesContainer
            .transform
            .GetChild(currentlySelectedRecipeIndex)
            .GetComponent<RecipeUISlot>()
            .DisplayRecipe();
        cookingRecipesContainer
            .transform
            .GetChild(currentlySelectedCookingIndex)
            .GetComponent<CookingUISlot>()
            .SelectRecipeForCooking();
    }
}
