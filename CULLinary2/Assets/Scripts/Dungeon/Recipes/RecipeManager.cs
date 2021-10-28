using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeManager : SingletonGeneric<RecipeManager>
{
    [Header("For Recipes UI")]
    // Container to attach recipes to
    public GameObject recipesContainer;
    // Prefab of a recipe book entry
    public GameObject recipeSlot;
    // Large info display of the recipe
    public RecipeUIInfoDisplay infoDisplay;
    public TextMeshProUGUI filterButtonText;
    public string[] filterButtonDisplayedTexts;

    public Animator ingredientsEmphasisAnimator;
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
    // Save the currently selected index of recipes in the UI
    private int currentlySelectedRecipeIndex = 0;

    // List of all campfire locations for the minimap
    private List<Transform> campfires = new List<Transform>();

    // UI filters
    // 0 - default
    // 1 - ordered
    // 2 - ready to cook
    private int currentFilterState = 0;
    private Action[] filterTransitions;

    void Start()
    {
        if (pickup == null)
        {
            Debug.Log("RecipeManager: Oops! Could not find PlayerPickup");
        }

        foreach (Transform child in recipesContainer.transform)
        {
            Destroy(child.gameObject);
        }

        filterTransitions = new Action[]{
            FilterOrderedRecipesInUI,
            FilterReadyToCookInstead,
            ResetAllRecipesInUI
        };

        filterButtonText.text = filterButtonDisplayedTexts[currentFilterState];
    }

    // To be called when save data is loaded
    public void FilterUnlockedRecipes()
    {
        if (!hasPopulatedUnlockedRecipes)
        {
            UpdateUnlockedRecipes();
        }
    }

    // Update the unlocked and the locked recipes list based on PlayerManager
    public void UpdateUnlockedRecipes()
    {
        List<int> unlockedRecipes = PlayerManager.instance != null
            ? PlayerManager.instance.unlockedRecipesList
            : new List<int> { 0, 4, 6, 10, 32 };

        StopAllCoroutines();
        innerLockedRecipesList.Clear();
        innerUnlockedRecipesList.Clear();

        foreach (int recipeId in unlockedRecipes)
        {
            innerUnlockedRecipesList.Add(DatabaseLoader.GetRecipeById(recipeId));
        }
        //Not sure if there's a faster method or a one-liner
        foreach (Recipe recipe in DatabaseLoader.GetAllRecipes())
        {
            if (!unlockedRecipes.Contains(recipe.recipeId))
            {
                innerLockedRecipesList.Add(DatabaseLoader.GetRecipeById(recipe.recipeId));
            }
        }

        hasPopulatedUnlockedRecipes = true;
        recipesChanged = true;
        ForceUIUpdate();
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
        ForceUIUpdate();
    }

    public void DeactivateCooking()
    {
        isCooking = false;
        ForceUIUpdate();
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
            ButtonAudio.Instance.ClickFailed();
            Debug.Log("RecipeManager: No recipe selected!");
            return;
        }
        Recipe r = currentRecipe;

        if (!isCooking)
        {
            ButtonAudio.Instance.ClickFailed();
            Debug.Log("RecipeManager: Not at campfire!");
            return;
        }

        if (InventoryManager.instance.RemoveIdsFromInventory(r.GetIngredientIds()))
        {
            // Sucessfully cooked!
            ButtonAudio.Instance.Click();

            // UI will be updated in AddItem
            infoDisplay.IncreaseInventoryCountAndSetIngredients();
            InventoryManager.instance.AddItem(r.cookedDishItem);
            // Create a pop-up for the player
            pickup?.PickUp(r.cookedDishItem);
            // Player cooking sfx
            cookingSound.Play();
        }
        else
        {
            ButtonAudio.Instance.ClickFailed();
            ingredientsEmphasisAnimator.SetTrigger("triggerFlash"); 
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

    public void FilterOrderedRecipesOnClick()
    {
        filterTransitions[currentFilterState]();
    }

    // Show all recipes in the UI
    public void ResetAllRecipesInUI()
    {
        foreach (Transform child in recipesContainer.transform)
        {
            child.gameObject.SetActive(true);
        }
        currentFilterState = 0;
        filterButtonText.text = filterButtonDisplayedTexts[currentFilterState];
    }

    // Filters the UI by ordered recipes
    private void FilterOrderedRecipesInUI()
    {
        foreach (Transform child in recipesContainer.transform)
        {
            RecipeUISlot recipeDetails = child.GetComponent<RecipeUISlot>();
            if (!recipeDetails.IsOrdered())
            {
                child.gameObject.SetActive(false);
            }
        }
        currentFilterState = 1;
        filterButtonText.text = filterButtonDisplayedTexts[currentFilterState];
    }

    // Switch from ordered recipes to cookable recipes
    private void FilterReadyToCookInstead()
    {
        foreach (Transform child in recipesContainer.transform)
        {
            RecipeUISlot recipeDetails = child.GetComponent<RecipeUISlot>();
            if (!recipeDetails.IsOrdered() && recipeDetails.IsCookable())
            {
                child.gameObject.SetActive(true);
            }
            else if (recipeDetails.IsOrdered() && !recipeDetails.IsCookable())
            {
                child.gameObject.SetActive(false);
            }
        }
        currentFilterState = 2;
        filterButtonText.text = filterButtonDisplayedTexts[currentFilterState];
    }

    public void ForceUIUpdate()
    {
        StopAllCoroutines();
        StartCoroutine(UpdateUI());
    }

    private IEnumerator UpdateUI()
    {
        // Wait until all orders have generated
        if (!OrdersManager.instance.IsOrderGenerationComplete())
        {
            yield break;
        }

        if (recipesChanged)
        {
            yield return StartCoroutine(RegenerateRecipes());
        }
        else
        {   
            yield return StartCoroutine(RefreshRecipes());
        }
    }

    // Goes through all unlocked recipes and updates them
    private IEnumerator RefreshRecipes()
    {
        foreach (Transform child in recipesContainer.transform)
        {
            RecipeUISlot recipeDetails = child.GetComponent<RecipeUISlot>();
            if (recipeDetails.IsRecipeKnown())
            {
                recipeDetails.UpdateInfo();
            }
            yield return null;
        }
    
        // Retain the same index for the display after updating
        recipesContainer
            .transform
            .GetChild(currentlySelectedRecipeIndex)
            .GetComponent<RecipeUISlot>()
            .DisplayRecipe();
    }

    // Destroys all the recipes and recreates them in the UI
    private IEnumerator RegenerateRecipes()
    {
        foreach (Transform child in recipesContainer.transform)
        {
            Destroy(child.gameObject);
        }
        yield return StartCoroutine(RecreateUnlockedRecipes());
    }

    private IEnumerator RecreateUnlockedRecipes()
    {
        for (int i = 0; i < innerUnlockedRecipesList.Count; i++)
        {
            Recipe r = innerUnlockedRecipesList[i];
            List<(int, int)> ingIds = r.GetIngredientIds();
            List<(int, int, int)> invReqCount = new List<(int, int, int)>();
            bool areIngresInInv =
                InventoryManager.instance.CheckIfItemsExist(ingIds, out invReqCount);
            int numberOfOrders = OrdersManager.instance.GetNumberOfOrders(r.recipeId);
            CheckIfRecipeHasCookedDish(r);
            int numberInInventory = InventoryManager.instance.GetAmountOfItem(r.cookedDishItem.inventoryItemId);

            // Add to Recipes UI
            GameObject recipeEntry = Instantiate(recipeSlot,
                                                 new Vector3(0, 0, 0),
                                                 Quaternion.identity,
                                                 recipesContainer.transform) as GameObject;
            RecipeUISlot recipeDetails = recipeEntry.GetComponent<RecipeUISlot>();
            recipeDetails.AddRecipe(r, areIngresInInv, invReqCount, numberOfOrders, numberInInventory, i);
            recipeDetails.SetInfoDisplay(infoDisplay);
            yield return null;
        }
        yield return StartCoroutine(RecreateLockedRecipes());
    }

    private IEnumerator RecreateLockedRecipes()
    {
        foreach (Recipe r in innerLockedRecipesList)
        {
            CheckIfRecipeHasCookedDish(r);
            GameObject recipeEntry = Instantiate(recipeSlot,
                                                 new Vector3(0, 0, 0),
                                                 Quaternion.identity,
                                                 recipesContainer.transform) as GameObject;
            RecipeUISlot recipeDetails = recipeEntry.GetComponent<RecipeUISlot>();
            recipeDetails.AddUnknownRecipe(r);
            recipeDetails.SetInfoDisplay(infoDisplay);
            yield return null;
        }
        
        // Reset selected recipe index to 0
        currentlySelectedRecipeIndex = 0;
        recipesChanged = false;
        recipesContainer
            .transform
            .GetChild(currentlySelectedRecipeIndex)
            .GetComponent<RecipeUISlot>()
            .DisplayRecipe();
    }

    private void CheckIfRecipeHasCookedDish(Recipe r)
    {
        if (r.cookedDishItem == null)
        {
            Debug.LogError("RecipeManager.cs: Cooked dish item for recipe ID " + r.recipeId + " not set!");
        }
    }
}



/*
bool[] recipesUnlocked = PlayerManager.instance
? PlayerManager.instance.recipesUnlocked
: new bool[36] {
    true, true, true, true, true,
    false, true, true, true, true,
    true, false , false, false, false,
    false, false, false, false , false,
    false, false, false, false , false,
    false, false, false, false , false,
    false, false, false, false , false,
    false, };

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
*/