using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Recipe")]
public class Recipe : ScriptableObject
{
    public int recipeId;
    public InventoryItem[] ingredientList;
    public InventoryItem cookedDishItem;
    public int recipeEarnings;

    public int[] GetIngredientIds()
    {
        int[] idArray = new int[ingredientList.Length];
        for (int i = 0; i < ingredientList.Length; i++)
        {
            idArray[i] = ingredientList[i].inventoryItemId;
        }
        return idArray;
    }
}
