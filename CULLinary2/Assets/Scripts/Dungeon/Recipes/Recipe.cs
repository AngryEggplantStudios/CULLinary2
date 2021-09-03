using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Recipe")]
public class Recipe : ScriptableObject
{
	public int recipeId = 0;
	public Item[] ingredientList;
    public Item cookedRecipeItem;

    // Assumes recipe name is the same as the cooked item name
    public string GetRecipeName()
    {
        return cookedRecipeItem.name;
    }

    public int[] GetIngredientIds()
    {
        int[] idArray = new int[ingredientList.Length];
        for (int i = 0; i < ingredientList.Length; i++)
        {
            idArray[i] = ingredientList[i].itemId;
        }
        return idArray;
    }
}
