using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderSlot : MonoBehaviour
{
    public Image orderIcon;
    public Text orderDescription;

    private Order order;

    public void AssignOrder(Order newOrder)
    {
        order = newOrder;
        orderIcon.sprite = order.GetProduct().icon;
        orderDescription.text = "Give a " + order.GetProduct().itemName +
            " to house #" + order.GetSubmissionStationId()
            + ". " + order.GetDescription();
    }

    public Order GetOrder()
    {
        return order;
    }
}
