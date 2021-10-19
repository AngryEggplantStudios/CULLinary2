using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRecipeIdTest : MonoBehaviour
{
    public int[] recipeId = new int[5] { 1, 2, 3, 4, 5 };
    public int currentIndex = 0;
    public void TestAddRecipe()
    {
        if (PlayerManager.instance != null && currentIndex < recipeId.Length)
        {
            PlayerManager.instance.unlockedRecipesList.Add(recipeId[currentIndex]);
            RecipeManager.instance.UpdateUnlockedRecipes();
            Debug.Log("Added index: " + currentIndex);
            currentIndex++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            TestAddRecipe();
        }

    }
}
