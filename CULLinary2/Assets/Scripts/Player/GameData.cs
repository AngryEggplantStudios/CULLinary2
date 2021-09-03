using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private RecipeDatabase recipeDatabase;

    private static Dictionary<int, Item> itemDict;
    private static List<Item> itemList = new List<Item>();

    private static Dictionary<int, Recipe> recipeDict;

    private static List<Recipe> recipeList;

    public static GameData instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
            Debug.Log("Creating instance of GameData");
        }
        else
        {
            Debug.Log("Duplicate GameData Detected. Deleting new GameData");
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        itemDict = new Dictionary<int, Item>();
        itemList = itemDatabase.allItems;
        StartCoroutine(PopulateItemDatabase());

        recipeDict = new Dictionary<int, Recipe>();
        recipeList = recipeDatabase.recipes;
        StartCoroutine(PopulateRecipeDatabase());
    }

    private IEnumerator PopulateItemDatabase()
    {
        foreach (Item i in itemDatabase.allItems)
        {
            try
            {
                itemDict.Add(i.itemId, i);
            }
            catch
            {
                Debug.Log("Unable to add item: " + i.name);
            }
            yield return null;
        }
    }

    public static Item GetItemById(int id)
    {
        return itemDict[id];
    }

    public static List<Item> GetItemList()
    {
        return itemList;
    }

    public static Dictionary<int, Item> GetItemDict()
    {
        return itemDict;
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
                Debug.Log("Unable to add recipe: " + r.GetRecipeName());
            }
            yield return null;
        }
        
        // TODO: Get the unlocked recipes from saved game instead
        GameObject.FindObjectOfType<RecipeManager>().FilterUnlockedRecipes(new List<int>{0, 1, 2});
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
