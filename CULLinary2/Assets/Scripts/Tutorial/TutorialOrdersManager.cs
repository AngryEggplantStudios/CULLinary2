using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialOrdersManager : SingletonGeneric<TutorialOrdersManager>
{
    public Recipe recipeRequired;
    public Order orderRequired;
    public AudioSource orderSubmissionSound;
    public GameObject moneyNotif_prefab;
    // Container to attach orders to
    public GameObject ordersContainer;

    // Prefab of an order book entry
    public GameObject orderSlot;
    public GameObject canvasDisplay;
    private Transform orderSubmissionStn;
    private bool firstGeneration = true;

    public bool hasDeliveredFood = false;

    void Start()
    {
        orderRequired = new Order(recipeRequired, "Your very first order.", TutorialManager.instance.orderSubmissionStnId);

        /*InstantiateFloatingItemNotifs();*/
    }


    // Returns true only if map and orders have been generated
    // Map always generates first before orders
    public bool IsOrderGenerationComplete()
    {
        return firstGeneration;
    }

    public bool IsPlayerNearOrderSubmissionStn()
    {
        return orderSubmissionStn.GetComponent<TutOrderSubmissionStation>().isPlayerWithinRange;
    }

    public void SetOrderSubmissionStn(Transform transform)
    {
        orderSubmissionStn = transform;
        orderSubmissionStn.GetComponent<TutOrderSubmissionStation>().HideFloatingItemNotif();
    }

    public void CompleteOrder(int stationId)
    {
        if (!TutorialManager.instance.canDeliverFood)
        {
            return;
        }

        TutorialInventoryManager inventory = TutorialInventoryManager.instance;
        if (inventory.RemoveIdIfPossible(orderRequired.GetProduct().inventoryItemId))
        {
            // Order completed successfully!

            // Update money
            int earnings = orderRequired.GetDeliveryReward();
            SpawnMoneyNotif(earnings);
            PlayerManager.instance.currentMoney += earnings;

            // Update UI and play sounds
            TutorialUIController.UpdateAllUIs();
            orderSubmissionSound.Play();

            orderSubmissionStn.GetComponent<TutOrderSubmissionStation>().HideFloatingItemNotif();

            hasDeliveredFood = true;
        }
    }

    private void SpawnMoneyNotif(float money)
    {
        GameObject moneyNotif = Instantiate(moneyNotif_prefab);
        moneyNotif.transform.GetComponentInChildren<Text>().text = "+$" + money.ToString();
        moneyNotif.transform.SetParent(canvasDisplay.transform);
        moneyNotif.transform.localPosition = Vector3.zero;
    }

    public void InstantiateFloatingItemNotifs()
    {
        orderSubmissionStn.GetComponent<TutOrderSubmissionStation>().SetFloatingItemNotif(orderRequired.GetProduct().icon);
    }

    public Dictionary<int, (Transform, Sprite)> GetRelevantOrderStations()
    {
        Dictionary<int, (Transform, Sprite)> relevantStations = new Dictionary<int, (Transform, Sprite)>();
        int id = orderRequired.GetSubmissionStationId();
        relevantStations.Add(id, (orderSubmissionStn.GetComponent<TutOrderSubmissionStation>().getOrderSubmssionStationGameObj(), orderRequired.GetProduct().icon));
        return relevantStations;
    }

    public void ForceUIUpdate()
    {
        StopAllCoroutines();
        StartCoroutine(UpdateUI());
    }

    private IEnumerator UpdateUI()
    {
        foreach (Transform child in ordersContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // foreach (Order o in innerOrdersList)
        // {
        GameObject orderLog = Instantiate(orderSlot,
                                          new Vector3(0, 0, 0),
                                          Quaternion.identity,
                                          ordersContainer.transform) as GameObject;

        OrdersUISlot orderDetails = orderLog.GetComponent<OrdersUISlot>();
        TutorialInventoryManager inv = TutorialInventoryManager.instance;
        List<(int, int)> ingsList = orderRequired.GetIngredientIds();
        List<(int, int, int)> invReqCounter = new List<(int, int, int)>();

        bool isCookable = inv.CheckIfItemsExist(ingsList, out invReqCounter);
        bool isInInv = inv.CheckIfExists(orderRequired.GetProduct().inventoryItemId);
        orderDetails.AssignOrder(orderRequired, isCookable, isInInv, invReqCounter);
        // orderDetails.AssignOrder(orderRequired, false, false, invReqCounter);
        yield return null;
        // }
    }
}

