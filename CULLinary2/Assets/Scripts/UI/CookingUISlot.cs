using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingUISlot : MonoBehaviour
{
    public GameObject selectedButton;
    public GameObject cookableButton;
    public Image recipeIcon;
    public TextMeshProUGUI recipeName;
    public GameObject orderedIcon;

    private RecipeUIInfoDisplay infoDisplay = null;
    private Recipe recipe;

    private bool cookable;
    private bool ordered;

    
    private List<(int, int, int)> invReqCount;
    private int numOfOrders;
    private int numInInv;
    // Index of this slot in the Cooking UI
    private int indexInMenu;

    public void AddRecipe(
        Recipe newRecipe,
        bool isCookable,
        List<(int, int, int)> ingredientQuantities,
        int numberOfOrders,
        int numberInInventory,
        int indexInCookingMenu
    )
    {
        recipe = newRecipe;
        cookable = isCookable;
        ordered = numberOfOrders > 0;
        invReqCount = ingredientQuantities;
        numOfOrders = numberOfOrders;
        numInInv = numberInInventory;
        indexInMenu = indexInCookingMenu;
        UpdateUI();
    }

    public void SetInfoDisplay(RecipeUIInfoDisplay display)
    {
        infoDisplay = display;
    }

    public void SelectRecipeForCookingOnClick()
    {
        RecipeManager.instance.SetCurrentlySelectedRecipeInCookingUi(indexInMenu);
        SelectRecipeForCooking();
    }

    public void SelectRecipeForCooking()
    {
        if (infoDisplay == null)
        {
            Debug.Log("CookingUISlot: Missing information display!");
        }
        else
        {
            InventoryManager inv = InventoryManager.instance;
            infoDisplay.ShowRecipe(selectedButton, recipe, invReqCount, numOfOrders, numInInv);
            RecipeManager.instance.SetCurrentlyCookingRecipe(recipe);
        }
    }

    private void UpdateUI()
    {
        recipeIcon.sprite = recipe.cookedDishItem.icon;
        recipeName.text = recipe.cookedDishItem.itemName;
        cookableButton.SetActive(cookable);
        orderedIcon.SetActive(ordered);
    }
}
