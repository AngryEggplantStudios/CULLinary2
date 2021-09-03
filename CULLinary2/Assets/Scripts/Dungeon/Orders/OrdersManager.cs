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

    private List<Order> innerOrdersList;

    void Start()
    {
        // TODO: Replace these hard-coded orders
        innerOrdersList = new List<Order>{
            new Order(order1, "Give the pizza to the right guy", 1234),
            new Order(order2, "Give a fried eggplant to the left guy", 5678)
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
        if (inventory.RemoveIdIfPossible(orderToComplete.GetProduct().itemId))
        {
            // Order completed successfully!
            innerOrdersList.RemoveAt(orderIndex);

            Debug.Log("Money + $" + stationId + ", you win!");
            orderSubmissionSound.Play();
            orderSubmissionSound.SetScheduledEndTime(AudioSettings.dspTime + 11.15f);

            StartCoroutine(UpdateUI());
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
}
