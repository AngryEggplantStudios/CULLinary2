using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

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
    [SerializeField] private WeaponSkillDatabase weaponSkillDatabase;
    [SerializeField] private NewspaperDatabase newspaperDatabase;

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
    //Weapon Skill Items
    private static Dictionary<int, WeaponSkillItem> weaponSkillDict;
    private static List<WeaponSkillItem> weaponSkillList;
    //Newspaper
    private static Dictionary<int, NewsIssue> orderedNewsIssuesDict;
    private static List<NewsIssue> orderedNewsIssuesList;
    private static List<NewsIssue> randomNewsIssuesList;

    private void Awake()
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
        weaponSkillDict = new Dictionary<int, WeaponSkillItem>();
        weaponSkillList = new List<WeaponSkillItem>();
        orderedNewsIssuesDict = new Dictionary<int, NewsIssue>();
        orderedNewsIssuesList = new List<NewsIssue>();
        randomNewsIssuesList = new List<NewsIssue>();
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
        yield return StartCoroutine(PopulateWeaponSkillItemDatabase());
        yield return StartCoroutine(PopulateNewspaperDatabase());
    }

    private IEnumerator PopulateInventoryItemDatabase()
    {
        foreach (InventoryItem i in inventoryItemDatabase.allItems)
        {
            try
            {
                inventoryItemDict.Add(i.inventoryItemId, i);
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e);
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
        catch (Exception e)
        {
            Debug.Log("Error: " + e);
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
            catch (Exception e)
            {
                Debug.Log("Error: " + e);
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
        catch (Exception e)
        {
            Debug.Log("Error: " + e);
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
            catch (Exception e)
            {
                Debug.Log("Error: " + e);
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
        catch (Exception e)
        {
            Debug.Log("Error: " + e);
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
            catch (Exception e)
            {
                Debug.Log("Error: " + e);
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
        catch (Exception e)
        {
            Debug.Log("Error: " + e);
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
            catch (Exception e)
            {
                Debug.Log("Error: " + e);
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
        catch (Exception e)
        {
            Debug.Log("Error: " + e);
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

    private IEnumerator PopulateWeaponSkillItemDatabase()
    {
        foreach (WeaponSkillItem i in weaponSkillDatabase.allItems)
        {
            try
            {
                weaponSkillDict.Add(i.weaponSkillId, i);
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e);
                Debug.Log("Unable to add item: " + i.itemName);
            }
            yield return null;
        }

        Debug.Log("Weapon Skill Item Database populated.");
    }

    public static WeaponSkillItem GetWeaponSkillById(int id)
    {
        WeaponSkillItem item;
        try
        {
            item = weaponSkillDict[id];
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e);
            item = new WeaponSkillItem();
        }
        return item;
    }
    public static List<WeaponSkillItem> GetWeaponSkillList()
    {
        return weaponSkillList;
    }
    public static Dictionary<int, WeaponSkillItem> GetWeaponSkillDict()
    {
        return weaponSkillDict;
    }

    private IEnumerator PopulateNewspaperDatabase()
    {
        foreach (NewsIssue ni in newspaperDatabase.orderedIssues)
        {
            try
            {
                orderedNewsIssuesDict.Add(ni.issueId, ni);
                orderedNewsIssuesList.Add(ni);
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e);
                Debug.Log("Unable to add news issue: " + ni.headlines);
            }
            yield return null;
        }
        
        foreach (NewsIssue ni in newspaperDatabase.randomIssues)
        {
            try
            {
                randomNewsIssuesList.Add(ni);
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e);
                Debug.Log("Unable to add news issue: " + ni.headlines);
            }
            yield return null;
        }

        Debug.Log("Newspaper Database populated.");
    }

    public static NewsIssue GetOrderedNewsIssueById(int id)
    {
        NewsIssue issue;
        try
        {
            issue = orderedNewsIssuesDict[id];
            return issue;
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e);
            return null;
        }
    }
    public static List<NewsIssue> GetOrderedNewsIssuesList()
    {
        return orderedNewsIssuesList;
    }
    public static Dictionary<int, NewsIssue> GetOrderedNewsIssuesDict()
    {
        return orderedNewsIssuesDict;
    }

    public static List<NewsIssue> GetRandomNewsIssuesList()
    {
        return randomNewsIssuesList;
    }

    public static NewsIssue GetRandomNewsIssue()
    {
        return randomNewsIssuesList[Random.Range(0, randomNewsIssuesList.Count)];
    }
}
