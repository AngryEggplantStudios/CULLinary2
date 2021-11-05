using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutOrderSubmissionStation : PlayerInteractable
{
    public SpherePlayerCollider spCollider;
    public GameObject[] activeIfOrdered;
    public Image floatingItemNotifImage;
    public int uniqueId = -1;

    private bool hasDialogue = false;

    public void Awake()
    {
        // Set the unique ID of this station
        // 
        // Note that OrdersManager should have a lower execution order 
        // than OrderSubmissionStation (i.e. executed first)
        // SetId(OrdersManager.instance.AddOrderSubmissionStation(this.GetComponent<Transform>()));
        uniqueId = TutorialManager.instance.orderSubmissionStnId;
        TutorialOrdersManager.instance.SetOrderSubmissionStn(transform);
        // TutorialOrdersManager.instance.InstantiateFloatingItemNotifs();
        spCollider.SetPlayerInteractable(this);
    }

    public void Start()
    {
        /*		foreach (GameObject obj in activeIfOrdered)
                {
                    obj.SetActive(false);
                }*/
    }

    public Transform getOrderSubmssionStationGameObj()
	{
        return this.gameObject.transform;
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
        TutorialOrdersManager.instance.CompleteOrder(uniqueId);
        // if (OrdersManager.Instance.CompleteOrder(uniqueId))
        // {
        //     hasDialogue = true;
        // }
        // if (hasDialogue)
        // {
        //     DialogueManager.instance.LoadAndRun(DialogueDatabase.GetRandomDialogue());
        //     hasDialogue = false;
        // }
    }

    public override void OnPlayerLeave()
    { }
}
