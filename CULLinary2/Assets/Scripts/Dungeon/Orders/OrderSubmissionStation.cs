using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSubmissionStation : PlayerInteractable
{
    public SpherePlayerCollider spCollider;
    public int uniqueId = -1;

    public void Awake()
    {
        // Set the unique ID of this station
        // 
        // Note that OrdersManager should have a lower execution order 
        // than OrderSubmissionStation (i.e. executed first)
        SetId(OrdersManager.instance.AddOrderSubmissionStation(this.GetComponent<Transform>()));
    }

    public void SetId(int id)
    {
        uniqueId = id;
    }

    public bool GetId(out int id)
    {
        id = uniqueId;
        if (uniqueId == -1)
        {
            Debug.Log("OrderSubmissionStation::getId failed, unique ID not set yet!");
            return false;
        }
        else
        {
            return true;
        }
    }

    public override SpherePlayerCollider GetCollider()
    {
        return spCollider;
    }

    public override void OnPlayerInteract()
    {
        OrdersManager.Instance.CompleteOrder(uniqueId);
    }

    public override void OnPlayerLeave()
    { }
}
