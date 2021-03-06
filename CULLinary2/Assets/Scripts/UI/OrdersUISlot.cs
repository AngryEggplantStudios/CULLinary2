using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrdersUISlot : MonoBehaviour
{
    public GameObject cookableIndicator;
    public GameObject deliverableButton;
    public GameObject deliverableIndicator;
    public Image productIcon;
    public GameObject ingredientsContainer;
    public GameObject ingredientCounterPrefab;
    public TextMeshProUGUI rewardsMoneyText;
    public TextMeshProUGUI dishNameText;

    private Order order;

    private bool cookable;
    private bool inInventory;

    // Array of (itemId, amountInInventory, amountNeeded)
    private List<(int, int, int)> ingQnties;
    private int numOfOrders;

    public void AssignOrder(
        Order newOrder,
        bool isCookable,
        bool isInInventory,
        List<(int, int, int)> ingredientQuantities
    )
    {
        if (isInInventory)
        {
            cookable = false;
        }
        else
        {
            cookable = isCookable;
        }

        order = newOrder;
        inInventory = isInInventory;
        ingQnties = ingredientQuantities;
        UpdateUI();
    }
    private void UpdateUI()
    {
        productIcon.sprite = order.GetProduct().icon;
        cookableIndicator.SetActive(cookable);
        rewardsMoneyText.text = order.GetDeliveryReward().ToString();
        dishNameText.text = order.GetProduct().itemName;

        if (inInventory)
        {
            deliverableButton.SetActive(true);
            deliverableIndicator.SetActive(true);
            ingredientsContainer.SetActive(false);
        }
        else
        {
            deliverableButton.SetActive(false);
            deliverableIndicator.SetActive(false);
            ingredientsContainer.SetActive(true);

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
}
