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
    public Camera cam;

    // Prefab to spawn on successful order
    public GameObject moneyNotif_prefab;

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

    void Update()
    {
        // Generate orders for the first day in MainScene
        if (!firstGeneration && BiomeGeneratorManager.IsGenerationComplete())
        {
            FirstGenerationOfOrders();
        }
    }

    public void AddOrder(Order order)
    {
        innerOrdersList.Add(order);
        isCacheValid = false;
        StopCoroutine(UpdateUI());
        StartCoroutine(UpdateUI());
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
            Debug.Log("OOPS! You do not have the required " + orderToComplete.GetProduct().name);
            return false;
        }
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
            int[] ingsArr = o.GetIngredientIds();
            (int, bool)[] missingItems = new (int, bool)[0];
            bool isCookable = inv.CheckIfItemsExist(ingsArr, out _, out missingItems);
            bool isInInv = inv.CheckIfExists(o.GetProduct().inventoryItemId);

            Dictionary<int, (int, int)> itemQuantities = new Dictionary<int, (int, int)>();
            foreach ((int itemId, bool isPresent) in missingItems)
            {
                if (!itemQuantities.ContainsKey(itemId))
                {
                    itemQuantities.Add(itemId, (0, 0));
                }
                int newInvItemAmount = itemQuantities[itemId].Item1;
                if (isPresent)
                {
                    newInvItemAmount++;
                }
                itemQuantities[itemId] = (newInvItemAmount, itemQuantities[itemId].Item2 + 1);
            }

            List<(int, int, int)> itemsCounted = new List<(int, int, int)>();
            foreach (KeyValuePair<int, (int, int)> idCountPair in itemQuantities)
            {
                itemsCounted.Add((
                    idCountPair.Key,
                    idCountPair.Value.Item1,
                    idCountPair.Value.Item2
                ));
            }
            orderDetails.AssignOrder(o, isCookable, isInInv, itemsCounted.ToArray());
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

    public void OnDestroy()
    {
        GameTimer.OnStartNewDay -= OnNewDay;
    }
}
