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

    private List<Recipe> innerUnlockedRecipesList = new List<Recipe>();
    private List<Recipe> innerLockedRecipesList = new List<Recipe>();
    private bool isCooking = false;

    // Currently selected recipe in Cooking UI
    private Recipe currentRecipe = null;

    // Random number generator for recipes
    private System.Random rand = new System.Random();

    // Only populate the unlocked recipes list once
    private bool hasPopulatedUnlockedRecipes = false;

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
            InventoryManager.instance.AddItem(r.cookedDishItem);
            // UI will be updated in AddItem
        }
        else
        {
            Debug.Log("RecipeManager: ingredients required!");
        }
    }

    public IEnumerator UpdateUI()
    {
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

        foreach (Recipe r in innerUnlockedRecipesList)
        {
            // Add to Recipes UI
            GameObject recipeEntry = Instantiate(recipeSlot,
                                                 new Vector3(0, 0, 0),
                                                 Quaternion.identity,
                                                 recipesContainer.transform) as GameObject;

            RecipeUISlot recipeDetails = recipeEntry.GetComponent<RecipeUISlot>();
            int[] ingIds = r.GetIngredientIds();
            (int, bool)[] checkedIngs = new (int, bool)[] { };
            bool areItemsInInventory =
                InventoryManager.instance.CheckIfItemsExist(ingIds, out _, out checkedIngs);

            recipeDetails.AddRecipe(r, areItemsInInventory, checkedIngs, GetNumberOfOrders(r.recipeId));
            recipeDetails.SetInfoDisplay(infoDisplay);

            // Add to Cooking UI as well
            GameObject cookingUiEntry = Instantiate(cookingUiSlot,
                                                    new Vector3(0, 0, 0),
                                                    Quaternion.identity,
                                                    cookingRecipesContainer.transform) as GameObject;
            CookingUISlot cookingRecipeDetails = cookingUiEntry.GetComponent<CookingUISlot>();
            cookingRecipeDetails.AddRecipe(r, areItemsInInventory, checkedIngs, GetNumberOfOrders(r.recipeId));
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
    }
}
