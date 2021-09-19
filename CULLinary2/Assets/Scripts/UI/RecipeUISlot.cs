using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeUISlot : MonoBehaviour
{
    public GameObject cookableButton;
    public Image recipeIcon;
    public TextMeshProUGUI recipeDescription;
    public GameObject orderedIcon;

    [Header("For Unknown Recipes")]
    public Color colourForUnknownRecipe = Color.black;
    public string textForUnknownRecipe = "???";


    private RecipeUIInfoDisplay infoDisplay = null;
    private Recipe recipe;

    private bool known;
    private bool cookable;
    private bool ordered;

    
    (int, bool)[] checkedIngs;
    private int numOfOrders;

    public void AddRecipe(
        Recipe newRecipe,
        bool isCookable,
        (int, bool)[] checkedIngredients,
        int numberOfOrders
    )
    {
        recipe = newRecipe;
        known = true;
        cookable = isCookable;
        ordered = numberOfOrders > 0;
        checkedIngs = checkedIngredients;
        numOfOrders = numberOfOrders;
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
            infoDisplay.ShowRecipe(recipe, checkedIngs, numOfOrders);
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
        cookableButton.SetActive(cookable);
        orderedIcon.SetActive(ordered);
    }
}
