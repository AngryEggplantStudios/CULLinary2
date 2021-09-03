using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeManager : SingletonGeneric<RecipeManager>
{
    // Container to attach recipes to
    public GameObject recipesContainer;
    
    // Prefab of a recipe book entry
    public GameObject recipeSlot;

    private List<Recipe> innerUnlockedRecipesList = new List<Recipe>();
    private bool isCooking = false;

    // To be called when save data is loaded
    public void FilterUnlockedRecipes(List<int> recipeIds)
    {
        foreach (int id in recipeIds)
        {
           innerUnlockedRecipesList.Add(GameData.GetRecipeById(id));
        }
        StartCoroutine(UpdateUI());
    }

    public void ActivateCooking()
    {
        isCooking = true;
    }

    public void DeactivateCooking()
    {
        isCooking = false;
    }

    public bool IsCookingActivated()
    {
        return isCooking;
    }

    public IEnumerator UpdateUI()
    {
        foreach (Recipe r in innerUnlockedRecipesList)
        {
            yield return null;
            GameObject recipeEntry = Instantiate(recipeSlot,
                                                 new Vector3(0, 0, 0),
                                                 Quaternion.identity,
                                                 recipesContainer.transform) as GameObject;
            RecipeSlot recipeDetails = recipeEntry.GetComponent<RecipeSlot>();
            recipeDetails.AddRecipe(r);
        }
    }
}
