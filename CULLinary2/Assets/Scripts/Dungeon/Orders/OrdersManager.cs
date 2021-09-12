using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdersManager : SingletonGeneric<OrdersManager>
{
    // Container to attach orders to
    public GameObject ordersContainer;

    // Prefab of an order book entry
    public GameObject orderSlot;

    // Order submission sound
    public AudioSource orderSubmissionSound;

    // Hard-coded recipes for demo
    // TODO: Generate orders randomly
    public Recipe order1;
    public Recipe order2;

    // Define a callback for order completion
    public delegate void OrderCompletionDelegate(int orderSubmissionStationId);

    private List<Order> innerOrdersList;
    // Hash table that maps the ID of the order submission station to their transforms
    private Dictionary<int, Transform> orderSubmissionStations = new Dictionary<int, Transform>();
    private int numberOfOrders = 0;
    private event OrderCompletionDelegate onOrderCompleteCallback;

    void Start()
    {
        // TODO: Replace these hard-coded orders
        innerOrdersList = new List<Order>{
            new Order(order1, "Give the mashed potatoes to the right house", 0),
            new Order(order2, "Give a eggplant soup to the left house", 1)
        };
        StartCoroutine(UpdateUI());
    }

    public void AddOrder(Order order)
    {
        innerOrdersList.Add(order);
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

            Debug.Log("Money + $" + stationId + ", you win!");
            orderSubmissionSound.Play();
            orderSubmissionSound.SetScheduledEndTime(AudioSettings.dspTime + 11.15f);

            StartCoroutine(UpdateUI());
            onOrderCompleteCallback.Invoke(stationId);
            return true;
        }
        else
        {
            Debug.Log("OOPS! You do not have the required " + orderToComplete.GetProduct().name);
            return false;
        }
    }

    public IEnumerator UpdateUI()
    {
        foreach (Transform child in ordersContainer.transform)
        {
            yield return null;
            Destroy(child.gameObject);
        }

        foreach (Order o in innerOrdersList)
        {
            yield return null;
            GameObject orderLog = Instantiate(orderSlot,
                                              new Vector3(0, 0, 0),
                                              Quaternion.identity,
                                              ordersContainer.transform) as GameObject;
            OrderSlot orderDetails = orderLog.GetComponent<OrderSlot>();
            orderDetails.AssignOrder(o);
        }
    }

    // Adds the order submission station to the hash table of stations and
    // returns an unique ID (also the key of the station in the hash table)
    public int AddOrderSubmissionStation(Transform stationTransform)
    {
        int orderIndex = numberOfOrders;
        orderSubmissionStations.Add(orderIndex, stationTransform);
        numberOfOrders++;
        return orderIndex;
    }

    // Register a callback to be run when an order is completed
    public void AddOrderCompletionCallback(OrderCompletionDelegate ocd)
    {
        onOrderCompleteCallback += ocd;
    }

    // Gets all relevant order stations
    // Note this this loops through all the orders
    public Dictionary<int, Transform> GetRelevantOrderStations()
    {
        Dictionary<int, Transform> relevantStations = new Dictionary<int, Transform>();
        foreach (Order o in innerOrdersList)
        {
            int id = o.GetSubmissionStationId();
            relevantStations.Add(id, orderSubmissionStations[id]);
        }
        return relevantStations;
    }
}
