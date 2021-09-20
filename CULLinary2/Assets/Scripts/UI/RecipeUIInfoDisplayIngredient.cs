using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUIInfoDisplayIngredient : MonoBehaviour
{
    public Image ingredientIcon;
    public float nonExistentItemOpacity = 0.35f;

    public void DisplayIngredient(int itemId, bool isItemInInventory)
    {
        float opacity = isItemInInventory ? 1.0f : nonExistentItemOpacity;
        ingredientIcon.sprite = DatabaseLoader.GetItemById(itemId).icon;
        ingredientIcon.color = new Color(
            ingredientIcon.color.r,
            ingredientIcon.color.g,
            ingredientIcon.color.b,
            opacity
        );
    }
}
