using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeUIInfoDisplayIngredient : MonoBehaviour
{
    public Image ingredientIcon;
    public TextMeshProUGUI text;
    public string separatorSymbol = "/";
    public float nonExistentItemOpacity = 0.35f;
    public Color lessThanColour = Color.red;
    public Color enoughColour = Color.green;

    // Keep track of shown counts
    private int currentShownInvCount = 0;
    private int currentShownReqCount = 0;

    public void DisplayIngredient(int itemId, int inventoryCount, int requiredCount)
    {
        bool isItemInInventory = inventoryCount >= requiredCount;
        float opacity = isItemInInventory ? 1.0f : nonExistentItemOpacity;

        ingredientIcon.sprite = DatabaseLoader.GetItemById(itemId).icon;
        text.text = inventoryCount + separatorSymbol + requiredCount;
        currentShownInvCount = inventoryCount;
        currentShownReqCount = requiredCount;
        if (inventoryCount < requiredCount)
        {
            text.color = lessThanColour;
        }
        else
        {
            text.color = enoughColour;
        }
        ingredientIcon.color = new Color(
            ingredientIcon.color.r,
            ingredientIcon.color.g,
            ingredientIcon.color.b,
            opacity
        );
    }

    // Deduct the required count from the inventory count
    public void DeductIngredients()
    {
        currentShownInvCount -= currentShownReqCount;
        text.text = currentShownInvCount + separatorSymbol + currentShownReqCount;
        if (currentShownInvCount < currentShownReqCount)
        {
            text.color = lessThanColour;
        }
        else
        {
            text.color = enoughColour;
        }
    }
}
