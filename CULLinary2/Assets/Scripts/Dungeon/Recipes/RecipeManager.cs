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
    public void FilterUnlockedRecipes()
    {
        bool[] recipesUnlocked = PlayerManager.instance ? PlayerManager.instance.recipesUnlocked : new bool[3] { true, true, true };
        for (int id = 0; id < recipesUnlocked.Length; id++)
        {
            Debug.Log(id);
            if (recipesUnlocked[id])
            {
                innerUnlockedRecipesList.Add(GameData.GetRecipeById(id));
            }
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
        foreach (Transform child in recipesContainer.transform)
        {
            yield return null;
            Destroy(child.gameObject);
        }

        foreach (Recipe r in innerUnlockedRecipesList)
        {
            yield return null;
            GameObject recipeEntry = Instantiate(recipeSlot,
                                                 new Vector3(0, 0, 0),
                                                 Quaternion.identity,
                                                 recipesContainer.transform) as GameObject;
            yield return null;
            RecipeSlot recipeDetails = recipeEntry.GetComponent<RecipeSlot>();
            recipeDetails.AddRecipe(r);
        }
    }
}
