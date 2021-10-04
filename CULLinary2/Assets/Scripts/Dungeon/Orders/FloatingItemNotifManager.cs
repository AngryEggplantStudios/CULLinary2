using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingItemNotifManager : MonoBehaviour
{
    private bool hasInstantiated = false;
    private List<OrderSubmissionStation> currentStations = new List<OrderSubmissionStation>();

    // Update is called once per frame
    void Update()
    {
        if (OrdersManager.instance.IsOrderGenerationComplete() && !hasInstantiated)
        {
            InstantiateFloatingItemNotifs();
            return;
        }
    }

    // Calling this will trigger the manager to reset the floating items
    private void ResetInstantiatedFloatingItemNotifsFlag()
    {
        hasInstantiated = false;
    }

    // Instantiates all the floating items based on the order list
    void InstantiateFloatingItemNotifs()
    {
        // Clear the floating item of the old stations
        foreach (OrderSubmissionStation stn in currentStations)
        {
            stn.HideFloatingItemNotif();
        }
        currentStations = new List<OrderSubmissionStation>();

        // Set the floating item for the new stations
        Dictionary<int, (Transform, Sprite)> relevantOrders = OrdersManager.instance.GetRelevantOrderStations();
        foreach (KeyValuePair<int, (Transform, Sprite)> order in relevantOrders)
        {
            // Set image of floating item notification
            OrderSubmissionStation orderStation = order.Value.Item1.GetComponent<OrderSubmissionStation>();
            orderStation.SetFloatingItemNotif(order.Value.Item2);
            currentStations.Add(orderStation);
        }

        // Register the callbacks
        OrdersManager.instance.AddOrderCompletionCallback((stationId, _) =>
            OrdersManager
                .instance
                .GetOrderSubmissionStation(stationId)
                .GetComponent<OrderSubmissionStation>()
                .HideFloatingItemNotif()
        );
        OrdersManager.instance.AddOrderGenerationCallback(ResetInstantiatedFloatingItemNotifsFlag);
        hasInstantiated = true; 
    }
}
