using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeUISlot : MonoBehaviour
{
    public GameObject selectedButton;
    public Image recipeIcon;
    public TextMeshProUGUI recipeDescription;

    [Header("For Unknown Recipes")]
    public Color colourForUnknownRecipe = Color.black;
    public string textForUnknownRecipe = "Locked Recipe";


    private RecipeUIInfoDisplay infoDisplay = null;
    private Recipe recipe;

    private bool known;
    private bool cookable;
    private bool ordered;

    
    private List<(int, int, int)> invReqCount;
    private int numOfOrders;
    private int numInInv;

    public void AddRecipe(
        Recipe newRecipe,
        bool isCookable,
        List<(int, int, int)> ingredientQuantities,
        int numberOfOrders,
        int numberInInventory
    )
    {
        recipe = newRecipe;
        known = true;
        cookable = isCookable;
        ordered = numberOfOrders > 0;
        invReqCount = ingredientQuantities;
        numOfOrders = numberOfOrders;
        numInInv = numberInInventory;
        UpdateUI();
    }

    public void AddUnknownRecipe(Recipe newRecipe)
    {
        recipe = newRecipe;
        known = false;
        cookable = false;
        ordered = false;
        UpdateUI();
    }

    public void SetInfoDisplay(RecipeUIInfoDisplay display)
    {
        infoDisplay = display;
    }

    public void DisplayRecipeOnClick()
    {
        if (known && infoDisplay != null)
        {
            infoDisplay.ShowRecipe(selectedButton, recipe, invReqCount, numOfOrders, numInInv);
        }
        if (infoDisplay == null)
        {
            Debug.Log("RecipeUISlot: Missing information display!");
        }
    }

    private void UpdateUI()
    {
        recipeIcon.sprite = recipe.cookedDishItem.icon;
        if (known)
        {
            recipeDescription.text = recipe.cookedDishItem.itemName;
        }
        else
        {
            recipeDescription.text = textForUnknownRecipe;
            recipeIcon.color = colourForUnknownRecipe;
        }
    }

    public void ActivateSelectedButton()
    {
        selectedButton.SetActive(true);
    }

    public void DeactivateSelectedButton()
    {
        selectedButton.SetActive(false);
    }
}
