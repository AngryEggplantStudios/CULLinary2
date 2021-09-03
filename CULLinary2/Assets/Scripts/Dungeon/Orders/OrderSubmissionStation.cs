using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSubmissionStation : PlayerInteractable
{
    public SpherePlayerCollider spCollider;

    // TODO: To be set when the day begins,
    //       could also set to private
    // 
    // NOTE: Ensure that it is never set to -1,
    public int uniqueId = -1;
    
    private bool hasCached = false;
    private PlayerPickup inventory;

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
        return spCollider;
    }

    public override void OnPlayerInteract(GameObject player)
    {
        CacheReferences(player);
        OrdersManager.Instance.CompleteOrder(uniqueId);
    }

    public override void OnPlayerLeave(GameObject player)
    { }

    // Helper function to cache useful references
    private void CacheReferences(GameObject player)
    {
        if (!hasCached)
        {            
            inventory = player.GetComponent<PlayerPickup>();
            hasCached = true;
        }
    }
}
