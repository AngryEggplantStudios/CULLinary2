using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderSubmissionStation : PlayerInteractable
{
    public SpherePlayerCollider spCollider;
    public GameObject[] activeIfOrdered;
    public Image floatingItemNotifImage;
    public int uniqueId = -1;

    public void Awake()
    {
        // Set the unique ID of this station
        // 
        // Note that OrdersManager should have a lower execution order 
        // than OrderSubmissionStation (i.e. executed first)
        SetId(OrdersManager.instance.AddOrderSubmissionStation(this.GetComponent<Transform>()));
    }

    public void Start()
    {
        foreach (GameObject obj in activeIfOrdered)
        {
            obj.SetActive(false);
        }
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

    // Sets the floating item image above this order submission station 
    // and also makes it visible to the player when close
    public void SetFloatingItemNotif(Sprite itemSprite)
    {
        floatingItemNotifImage.sprite = itemSprite;
        foreach (GameObject obj in activeIfOrdered)
        {
            obj.SetActive(true);
        }
    }

    // Hides the floating item image above this order submission station
    // even when the player is close to the station
    public void HideFloatingItemNotif()
    {
        foreach (GameObject obj in activeIfOrdered)
        {
            obj.SetActive(false);
        }
    }

    public override SpherePlayerCollider GetCollider()
    {
        return spCollider;
    }
    
    public override void OnPlayerEnter()
    { }

    public override void OnPlayerInteract()
    {
        OrdersManager.Instance.CompleteOrder(uniqueId);
    }

    public override void OnPlayerLeave()
    { }
}
