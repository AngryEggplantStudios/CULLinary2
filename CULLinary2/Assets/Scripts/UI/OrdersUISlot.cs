using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrdersUISlot : MonoBehaviour
{
    public GameObject cookableButton;
    public GameObject inInventoryIcon;
    public Image productIcon;
    public GameObject mappedIcon;
    public GameObject ingredientsContainer;
    public GameObject ingredientCounterPrefab;
    
    private Order order;

    private bool cookable;
    private bool inInventory;
    private bool mapped;

    // Array of (itemId, amountInInventory, amountNeeded)
    private (int, int, int)[] ingQnties;
    private int numOfOrders;

    public void AssignOrder(
        Order newOrder,
        bool isCookable,
        bool isInInventory,
        bool isMapped,
        (int, int, int)[] ingredientQuantities
    )
    {
        order = newOrder;
        cookable = isCookable;
        inInventory = isInInventory;
        mapped = isMapped;
        ingQnties = ingredientQuantities;
        UpdateUI();
    }

    public void MapOrderOnClick()
    {
        // TODO
        Debug.Log("You clicked on an order!");
    }

    private void UpdateUI()
    {
        productIcon.sprite = order.GetProduct().icon;
        cookableButton.SetActive(cookable || inInventory);
        inInventoryIcon.SetActive(inInventory);
        mappedIcon.SetActive(mapped);

        foreach (Transform child in ingredientsContainer.transform)
        {
            Destroy(child.gameObject);
        }
        foreach ((int id, int invQnty, int reqQnty) in ingQnties)
        {
            GameObject ingCounter = Instantiate(ingredientCounterPrefab,
                                                new Vector3(0, 0, 0),
                                                Quaternion.identity,
                                                ingredientsContainer.transform) as GameObject;
            OrdersUIIngredientCounter counter = 
                ingCounter.GetComponent<OrdersUIIngredientCounter>();
            counter.SetIngredient(id, invQnty, reqQnty);
        }
    }
}
