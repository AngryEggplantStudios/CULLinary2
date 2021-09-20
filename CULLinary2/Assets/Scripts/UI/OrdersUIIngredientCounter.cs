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
        // Clamp the number to the maximum
        if (numberInInventory > numberRequired)
        {
            numberToBeShown = numberRequired;
        }

        itemIcon.sprite = GameData.GetItemById(itemId).icon;
        text.text = numberToBeShown + separatorSymbol + numberRequired;
    }
}
