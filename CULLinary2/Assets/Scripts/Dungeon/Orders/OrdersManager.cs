using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrdersManager : SingletonGeneric<OrdersManager>
{
    // Container to attach orders to
    public GameObject ordersContainer;

    // Prefab of an order book entry
    public GameObject orderSlot;

    // For UI display
    public GameObject canvasDisplay;

    // Prefab to spawn on successful order
    public GameObject moneyNotif_prefab;

    // Prefab to spawn on failed order
    public GameObject missingOrderNotif_prefab;

    // Order submission sound
    public AudioSource orderSubmissionSound;

    // Hard-coded recipes for demo
    // TODO: Generate orders randomly
    public Recipe order1;
    public Recipe order2;

    // Number of orders to generate in a day
    public int numberOfDailyOrders = 6;


    // Define a callback for order completion
    public delegate void OrderCompletionDelegate(int orderSubmissionStationId, int recipeId);
    // Define a callback for order generation
    public delegate void OrderGenerationDelegate();

    private List<Order> innerOrdersList;
    // Hash table that maps the ID of the order submission station to their transforms
    private Dictionary<int, Transform> orderSubmissionStations = new Dictionary<int, Transform>();
    private int numberOfOrderSubStations = 0;
    private event OrderCompletionDelegate onOrderCompleteCallback;
    private event OrderGenerationDelegate onOrderGenerationCallback;
    // Random number generator for new orders
    private System.Random rand;
    // First generation of orders
    private bool firstGeneration = false;
    // Do not rely on game timer for first day generation
    private bool firstDay = true;

    // Cache for number of orders by recipe ID
    private Dictionary<int, int> numberOfOrdersByRecipeCache;
    private bool isCacheValid = false;

    // Track number of orders completed in a day for statistics
    private int numOfOrdersCompletedToday = 0;

    // Track amount of money earned today
    private int moneyEarnedToday = 0;

    // Buff bonus
    private bool isDoubled = false;

    void Start()
    {
        innerOrdersList = new List<Order>();
        rand = new System.Random();

        // Generate random orders daily
        GameTimer.OnStartNewDay += OnNewDay;
    }

    // A function to be run every new day 
    private void OnNewDay()
    {
        if (!firstDay)
        {
            GenerateRandomOrders();
            numOfOrdersCompletedToday = 0;
            moneyEarnedToday = 0;
        }
        else
        {
            firstDay = false;
        }
    }

    public void AddOrder(Order order)
    {
        innerOrdersList.Add(order);
        isCacheValid = false;
        StopCoroutine(UpdateUI());
        StartCoroutine(UpdateUI());
    }

    public IEnumerator ToggleDoubleEarnings(float duration)
    {
        isDoubled = true;
        yield return new WaitForSeconds(duration);
        isDoubled = false;
    }

    // Tries to complete an order with a certain station ID
    // If the station is not in any orders, returns false
    // Otherwise, returns true and deletes the order
    public bool CompleteOrder(int stationId)
    {
        // Linear-time search, not very efficient
        // But, number of orders is assumed to be small
        int orderIndex = -1;
        bool foundOrder = false;
        for (int i = 0; i < innerOrdersList.Count; i++)
        {
            Order currentOrder = innerOrdersList[i];
            if (currentOrder.GetSubmissionStationId() == stationId)
            {
                foundOrder = true;
                orderIndex = i;
                break;
            }
        }

        if (!foundOrder)
        {
            // No such order exists
            Debug.Log("OOPS! You do not have an order for NPC #" + stationId + "!");
            return false;
        }

        Order orderToComplete = innerOrdersList[orderIndex];
        InventoryManager inventory = InventoryManager.instance;
        if (inventory.RemoveIdIfPossible(orderToComplete.GetProduct().inventoryItemId))
        {
            // Order completed successfully!
            innerOrdersList.RemoveAt(orderIndex);

            // Update money
            int earnings = orderToComplete.GetDeliveryReward();
            if (isDoubled)
            {
                earnings *= 2;
            }
            SpawnMoneyNotif(earnings);
            PlayerManager.instance.currentMoney += earnings;

            // Update UI and play sounds
            UIController.UpdateAllUIs();
            orderSubmissionSound.Play();

            // Invoke additional callbacks given to OrdersManager
            onOrderCompleteCallback.Invoke(stationId, orderToComplete.GetRecipe().recipeId);

            // Update statistics
            numOfOrdersCompletedToday++;
            moneyEarnedToday = moneyEarnedToday + earnings;

            // Invalidate the cache of number of orders for each recipe
            isCacheValid = false;
            return true;
        }
        else
        {
            SpawnMissingOrderNotif(orderToComplete.GetProduct().name);
            return false;
        }
    }

    private void SpawnMissingOrderNotif(string order)
    {
        GameObject missingOrderNotif = Instantiate(missingOrderNotif_prefab);
        missingOrderNotif.transform.GetComponentInChildren<Text>().text = "You do not have a " + order + "!";
        missingOrderNotif.transform.SetParent(canvasDisplay.transform);
        missingOrderNotif.transform.localPosition = Vector3.zero;
    }

    private void SpawnMoneyNotif(float money)
    {
        GameObject moneyNotif = Instantiate(moneyNotif_prefab);
        moneyNotif.transform.GetComponentInChildren<Text>().text = "+$" + money.ToString();
        moneyNotif.transform.SetParent(canvasDisplay.transform);
        moneyNotif.transform.localPosition = Vector3.zero;
    }

    public IEnumerator UpdateUI()
    {
        foreach (Transform child in ordersContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Order o in innerOrdersList)
        {
            GameObject orderLog = Instantiate(orderSlot,
                                              new Vector3(0, 0, 0),
                                              Quaternion.identity,
                                              ordersContainer.transform) as GameObject;

            OrdersUISlot orderDetails = orderLog.GetComponent<OrdersUISlot>();
            InventoryManager inv = InventoryManager.instance;
            List<(int, int)> ingsList = o.GetIngredientIds();
            List<(int, int, int)> invReqCounter = new List<(int, int, int)>();

            bool isCookable = inv.CheckIfItemsExist(ingsList, out invReqCounter);
            bool isInInv = inv.CheckIfExists(o.GetProduct().inventoryItemId);
            orderDetails.AssignOrder(o, isCookable, isInInv, invReqCounter);
            yield return null;
        }
    }

    // Adds the order submission station to the hash table of stations and
    // returns an unique ID (also the key of the station in the hash table)
    public int AddOrderSubmissionStation(Transform stationTransform)
    {
        int orderIndex = numberOfOrderSubStations;
        orderSubmissionStations.Add(orderIndex, stationTransform);
        numberOfOrderSubStations++;
        return orderIndex;
    }

    // Gets the order submission station with the ID given
    // Warning: This method can return null!
    public Transform GetOrderSubmissionStation(int id)
    {
        if (orderSubmissionStations.ContainsKey(id))
        {
            return orderSubmissionStations[id];
        }
        else
        {
            return null;
        }
    }

    // Register a callback to be run when an order is completed
    public void AddOrderCompletionCallback(OrderCompletionDelegate ocd)
    {
        onOrderCompleteCallback += ocd;
    }

    public void AddOrderGenerationCallback(OrderGenerationDelegate ogd)
    {
        onOrderGenerationCallback += ogd;
    }

    // Gets all relevant order stations and icons
    // Note this this loops through all the orders
    public Dictionary<int, (Transform, Sprite)> GetRelevantOrderStations()
    {
        Dictionary<int, (Transform, Sprite)> relevantStations = new Dictionary<int, (Transform, Sprite)>();
        foreach (Order o in innerOrdersList)
        {
            int id = o.GetSubmissionStationId();
            relevantStations.Add(id, (orderSubmissionStations[id], o.GetProduct().icon));
        }
        return relevantStations;
    }

    // Generate the orders for the first time in MainScene
    // TODO: Load from save game/player data instead
    public void FirstGenerationOfOrders()
    {
        if (!firstGeneration)
        {
            GenerateRandomOrders();
            firstGeneration = true;
        }
    }

    // Returns true only if map and orders have been generated
    // Map always generates first before orders
    public bool IsOrderGenerationComplete()
    {
        return firstGeneration;
    }

    // Gets a dictionary of recipe IDs mapped to the number
    // of orders that the player has currently for that recipe.
    public Dictionary<int, int> GetNumberOfOrdersByRecipe()
    {
        if (!isCacheValid)
        {
            Dictionary<int, int> ordersByRecipe = new Dictionary<int, int>();
            foreach (Order o in innerOrdersList)
            {
                int recipeId = o.GetRecipe().recipeId;
                if (!ordersByRecipe.ContainsKey(recipeId))
                {
                    ordersByRecipe.Add(recipeId, 0);
                }
                ordersByRecipe[recipeId]++;
            }
            numberOfOrdersByRecipeCache = ordersByRecipe;
            isCacheValid = true;
        }
        return numberOfOrdersByRecipeCache;
    }

    // Gets the number of orders that are generated automatically every day
    public static int GetNumberOfOrdersGeneratedDaily()
    {
        return OrdersManager.instance.numberOfDailyOrders;
    }

    // Gets the number of orders that were completed today so far
    public static int GetNumberOfOrdersCompletedToday()
    {
        return OrdersManager.instance.numOfOrdersCompletedToday;
    }

    // Gets the amount of money earned from completing orders today so far
    public static int GetMoneyEarnedToday()
    {
        return OrdersManager.instance.moneyEarnedToday;
    }

    // To be called when a new day begins
    // Generates random orders and populates the order list
    private void GenerateRandomOrders()
    {
        innerOrdersList.Clear();
        List<int> stations = new List<int>(orderSubmissionStations.Keys);
        int numberOfStations = stations.Count;

        for (int i = 0; i < numberOfDailyOrders && i < numberOfStations; i++)
        {
            int randomIndex = rand.Next(stations.Count);
            int stationId = stations[randomIndex];
            Recipe randomRecipe = RecipeManager.instance.GetRandomRecipe();
            Order newOrder = new Order(randomRecipe, "¡Hola mundo!", stationId);

            stations.RemoveAt(randomIndex);
            innerOrdersList.Add(newOrder);
        }

        isCacheValid = false;
        StopCoroutine(UpdateUI());
        StartCoroutine(UpdateUI());
        if (onOrderGenerationCallback != null)
        {
            onOrderGenerationCallback.Invoke();
        }
    }

    public void ClearBuffs()
    {
        isDoubled = false;
    }

    public void OnDestroy()
    {
        GameTimer.OnStartNewDay -= OnNewDay;
    }
}
