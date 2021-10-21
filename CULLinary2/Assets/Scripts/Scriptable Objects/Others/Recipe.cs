using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Recipe")]
public class Recipe : ScriptableObject
{
    public int recipeId;
    public InventoryItem cookedDishItem;
    public int recipeEarnings = 100; //Default
    // Important: Each inventory item should be unique!
    // Also, both arrays should be the same length.
    [SerializeField] private InventoryItem[] ingredientsList;
    [SerializeField] private int[] ingredientsCount;

    // Cache for lists
    private List<(InventoryItem, int)> ingredientListCache = null;
    private List<(int, int)> ingredientIdsCache = null;

    public List<(InventoryItem, int)> GetIngredientList()
    {
        if (ingredientListCache == null)
        {
            ingredientListCache = new List<(InventoryItem, int)>();
            for (int i = 0; i < ingredientsList.Length; i++)
            {
                ingredientListCache.Add((ingredientsList[i], ingredientsCount[i]));
            }
        }
        return ingredientListCache;
    }

    // Gets a list of pairs of ingredient ids to the
    // amount required by this recipe for cooking
    public List<(int, int)> GetIngredientIds()
    {
        if (ingredientIdsCache == null)
        {
            ingredientIdsCache = new List<(int, int)>();
            for (int i = 0; i < ingredientsList.Length; i++)
            {
                ingredientIdsCache.Add((ingredientsList[i].inventoryItemId, ingredientsCount[i]));
            }
        }
        return ingredientIdsCache;
    }
}
