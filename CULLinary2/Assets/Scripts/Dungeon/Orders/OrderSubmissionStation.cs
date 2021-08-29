using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSubmissionStation : PlayerInteractable
{
    public SpherePlayerCollider collider;

    // To be set when the day begins
    // NOTE: Ensure that it is never set to -1,
    private int uniqueId = -1;

    public void setId(int id)
    {
        uniqueId = id;
    }

    public bool getId(out int id)
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
        return collider;
    }

    public override void OnPlayerInteract(GameObject player)
    { }

    public override void OnPlayerLeave(GameObject player)
    { }
}
