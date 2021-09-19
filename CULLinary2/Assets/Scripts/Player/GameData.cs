using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [Header("All Databases")]
    [SerializeField] private InventoryItemDatabase inventoryItemDatabase;
    [SerializeField] private RecipeDatabase recipeDatabase;

    //Inventory Items
    private static Dictionary<int, InventoryItem> inventoryItemDict;
    private static List<InventoryItem> inventoryItemList = new List<InventoryItem>();
    //Recipes
    private static Dictionary<int, Recipe> recipeDict;
    private static List<Recipe> recipeList;

    private void Start()
    {
        inventoryItemDict = new Dictionary<int, InventoryItem>();
        inventoryItemList = inventoryItemDatabase.allItems;
        //StartCoroutine(PopulateInventoryItemDatabase());

        recipeDict = new Dictionary<int, Recipe>();
        recipeList = recipeDatabase.recipes;
        //StartCoroutine(PopulateRecipeDatabase());

        StartCoroutine(Populate());
    }

    private IEnumerator Populate()
    {
        yield return StartCoroutine(PopulateInventoryItemDatabase());
        yield return StartCoroutine(PopulateRecipeDatabase());
    }

    private IEnumerator PopulateInventoryItemDatabase()
    {
        foreach (InventoryItem i in inventoryItemDatabase.allItems)
        {
            try
            {
                inventoryItemDict.Add(i.inventoryItemId, i);
            }
            catch
            {
                Debug.Log("Unable to add item: " + i.name);
            }
            yield return null;
        }

        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.LoadInventory();
        }
        Debug.Log("Inventory Item Database populated.");
    }

    public static InventoryItem GetItemById(int id)
    {
        return inventoryItemDict[id];
    }
    public static List<InventoryItem> GetItemList()
    {
        return inventoryItemList;
    }
    public static Dictionary<int, InventoryItem> GetItemDict()
    {
        return inventoryItemDict;
    }

    private IEnumerator PopulateRecipeDatabase()
    {
        foreach (Recipe r in recipeList)
        {
            try
            {
                recipeDict.Add(r.recipeId, r);
            }
            catch
            {
                Debug.Log("Unable to add recipe: " + r.cookedDishItem.itemName);
            }
            yield return null;
        }

        if (RecipeManager.instance != null)
        {
            RecipeManager.instance.FilterUnlockedRecipes();
        }

        Debug.Log("Recipe Database populated.");
    }

    public static Recipe GetRecipeById(int id)
    {
        return recipeDict[id];
    }

    public static List<Recipe> GetAllRecipes()
    {
        return recipeList;
    }

    public static Dictionary<int, Recipe> GetRecipeDict()
    {
        return recipeDict;
    }
}