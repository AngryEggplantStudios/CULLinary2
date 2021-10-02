using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrdersUIIngredientCounter : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI text;
    public string separatorSymbol = "/";

    public void SetIngredient(int itemId, int numberInInventory, int numberRequired)
    {
        int numberToBeShown = numberInInventory;

        if (numberInInventory < numberRequired)
        {
            text.color = Color.red;
        }
        else
        {
            text.color = Color.white;
            // Clamp the number to the maximum
            numberToBeShown = numberRequired;
        }

        itemIcon.sprite = DatabaseLoader.GetItemById(itemId).icon;
        text.text = numberToBeShown + separatorSymbol + numberRequired;
    }
}
