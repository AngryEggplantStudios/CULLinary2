using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public static PlayerManager instance;

    // Player Stats and default values
    public float currentHealth = 200f;
    public float maxHealth = 200f;
    public float currentStamina = 100f;
    public float maxStamina = 100f;
    public float meleeDamage = 20f;
    public int inventoryLimit = 16;
    public bool[] recipesUnlocked = new bool[3] { true, true, true }; //use index
    public List<InventoryItem> itemList = new List<InventoryItem>();

    // Private variables
    private static PlayerData playerData;
    private GameTimer timer;

    // Force singleton reference to be the first PlayerManager being instantiated
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
            timer = GameTimer.instance;
            Debug.Log("Creating instance of Player Manager");
        }
        else
        {
            Debug.Log("Duplicate Player Manager Detected. Deleting new Player Manager");
            Destroy(this.gameObject);
        }
    }

    public void SaveData(List<InventoryItem> itemList)
    {
        if (playerData == null)
        {
            playerData = new PlayerData();
        }
        // Save Player Stats
        playerData.currentHealth = currentHealth;
        playerData.maxHealth = maxHealth;
        playerData.currentStamina = currentStamina;
        playerData.maxStamina = maxStamina;
        playerData.inventory = SerializeInventory(itemList);
        playerData.recipesUnlocked = recipesUnlocked;
        SaveSystem.SaveData(playerData);
    }

    public void LoadInventory()
    {
        InventoryItemData[] inventory = JsonArrayParser.FromJson<InventoryItemData>(playerData.inventory);
        itemList.Clear();

        //Need to change this somehow
        foreach (InventoryItemData item in inventory)
        {
            for (int i = 0; i < item.count; i++)
            {
                itemList.Add(GameData.GetItemById(item.id));
            }
        }
    }

    public void LoadData()
    {
        playerData = SaveSystem.LoadData();
        currentHealth = playerData.currentHealth;
        maxHealth = playerData.maxHealth;
        currentStamina = playerData.currentStamina;
        maxStamina = playerData.maxStamina;
        recipesUnlocked = playerData.recipesUnlocked;
    }

    public void CreateBlankData()
    {
        //Create new data
        playerData = new PlayerData();
        itemList.Clear();
        //Load Default stats
        playerData.currentHealth = currentHealth;
        playerData.maxHealth = maxHealth;
        playerData.currentStamina = currentStamina;
        playerData.maxStamina = maxStamina;
        playerData.inventory = "";
        playerData.recipesUnlocked = new bool[3] { true, true, true };
        SaveSystem.SaveData(playerData);
    }

    private static string SerializeInventory(List<InventoryItem> itemList)
    {
        Dictionary<int, int> inventory = new Dictionary<int, int>();

        foreach (InventoryItem item in itemList)
        {
            if (inventory.ContainsKey(item.inventoryItemId))
            {
                inventory[item.inventoryItemId] += 1;
            }
            else
            {
                inventory.Add(item.inventoryItemId, 1);
            }
        }
        InventoryItemData[] items = new InventoryItemData[inventory.Count];
        int i = 0;
        foreach (var item in inventory)
        {
            InventoryItemData gameItem = new InventoryItemData(item.Key, item.Value);
            items[i] = gameItem;
            i++;
        }
        return JsonArrayParser.ToJson(items, true);
    }

}
