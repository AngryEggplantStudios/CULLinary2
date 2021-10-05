using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseLoader : MonoBehaviour
{
    [Header("Autoload?")]
    [SerializeField] private bool isAutoload = false;
    [Header("All Databases")]
    [SerializeField] private InventoryItemDatabase inventoryItemDatabase;
    [SerializeField] private RecipeDatabase recipeDatabase;
    [SerializeField] private ShopItemDatabase shopItemDatabase;
    [SerializeField] private EventsDatabase eventsDatabase;
    [SerializeField] private MonsterDatabase monsterDatabase;

    //Inventory Items
    private static Dictionary<int, InventoryItem> inventoryItemDict;
    private static List<InventoryItem> inventoryItemList = new List<InventoryItem>();
    //Recipes
    private static Dictionary<int, Recipe> recipeDict;
    private static List<Recipe> recipeList;
    //Shop Items (Upgrades)
    private static Dictionary<int, ShopItem> shopItemDict;
    private static List<ShopItem> shopitemList;
    //Events
    private static Dictionary<int, SpecialEvent> specialEventDict;
    private static List<SpecialEvent> eventList;
    //Monsters
    private static Dictionary<MonsterName, MonsterData> monsterDict;
    private static List<MonsterData> monsterList;

    private void Start()
    {
        inventoryItemDict = new Dictionary<int, InventoryItem>();
        inventoryItemList = inventoryItemDatabase.allItems;
        recipeDict = new Dictionary<int, Recipe>();
        recipeList = recipeDatabase.recipes;
        shopItemDict = new Dictionary<int, ShopItem>();
        shopitemList = shopItemDatabase.allItems;
        specialEventDict = new Dictionary<int, SpecialEvent>();
        eventList = eventsDatabase.allEvents;
        monsterDict = new Dictionary<MonsterName, MonsterData>();
        monsterList = monsterDatabase.allMonsters;
        if (isAutoload)
        {
            StartCoroutine(Populate());
        }
    }

    public IEnumerator Populate()
    {
        yield return StartCoroutine(PopulateInventoryItemDatabase());
        yield return StartCoroutine(PopulateRecipeDatabase());
        yield return StartCoroutine(PopulateShopItemDatabase());
        yield return StartCoroutine(PopulateEventDatabase());
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
                Debug.Log("Unable to add item: " + i.itemName);
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
        InventoryItem item;
        try
        {
            item = inventoryItemDict[id];
        }
        catch
        {
            item = new InventoryItem();
        }
        return item;
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
                Debug.Log("Unable to add recipe: " + r.cookedDishItem.itemName + "| id: " + r.recipeId);
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
        Recipe recipe;
        try
        {
            recipe = recipeDict[id];
        }
        catch
        {
            recipe = new Recipe();
        }
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

    private IEnumerator PopulateShopItemDatabase()
    {
        foreach (ShopItem i in shopitemList)
        {
            try
            {
                shopItemDict.Add(i.shopItemId, i);
            }
            catch
            {
                Debug.Log("Unable to add shop item: " + i.itemName);
            }
            yield return null;
        }
        if (ShopManager.instance != null)
        {
            ShopManager.instance.SetupShop();
        }
        Debug.Log("Shop Item Database populated.");
    }

    public static ShopItem GetShopItemById(int id)
    {
        ShopItem shopItem;
        try
        {
            shopItem = shopItemDict[id];
        }
        catch
        {
            shopItem = new ShopItem();
        }
        return shopItem;
    }

    public static List<ShopItem> GetAllShopItems()
    {
        return shopitemList;
    }

    public static Dictionary<int, ShopItem> GetShopItemDict()
    {
        return shopItemDict;
    }

    private IEnumerator PopulateEventDatabase()
    {
        foreach (SpecialEvent i in eventList)
        {
            try
            {
                specialEventDict.Add(i.eventId, i);
            }
            catch
            {
                Debug.Log("Unable to add shop item: " + i.eventName);
            }
            yield return null;
        }
        Debug.Log("Shop Event Database populated.");
    }

    public static SpecialEvent GetEventById(int id)
    {
        SpecialEvent specialEvent;
        try
        {
            specialEvent = specialEventDict[id];
        }
        catch
        {
            specialEvent = new SpecialEvent();
        }
        return specialEvent;
    }

    public static List<SpecialEvent> GetAllEvents()
    {
        return eventList;
    }

    public static Dictionary<int, SpecialEvent> GetSpecialEventDict()
    {
        return specialEventDict;
    }

    private IEnumerator PopulateMonsterDatabase()
    {
        foreach (MonsterData i in monsterList)
        {
            try
            {
                monsterDict.Add(i.monsterName, i);
            }
            catch
            {
                Debug.Log("Unable to add shop item: " + i.monsterName);
            }
            yield return null;
        }
        Debug.Log("Shop Event Database populated.");
    }

    public static MonsterData GetMonsterByMonsterName(MonsterName monsterName)
    {
        MonsterData monsterData;
        try
        {
            monsterData = monsterDict[monsterName];
        }
        catch
        {
            monsterData = new MonsterData();
        }
        return monsterData;
    }

    public static List<MonsterData> GetAllMonsters()
    {
        return monsterList;
    }

    public static Dictionary<MonsterName, MonsterData> GetMonsterDict()
    {
        return monsterDict;
    }
}
