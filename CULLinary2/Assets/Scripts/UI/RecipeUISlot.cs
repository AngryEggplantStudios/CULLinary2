using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeUISlot : MonoBehaviour
{
    public GameObject selectedButton;
    public GameObject cookableButton;
    public GameObject orderedIcon;
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
    // Index of this slot in the Recipes UI
    private int indexInMenu;

    public void AddRecipe(
        Recipe newRecipe,
        bool isCookable,
        List<(int, int, int)> ingredientQuantities,
        int numberOfOrders,
        int numberInInventory,
        int indexInRecipesMenu
    )
    {
        recipe = newRecipe;
        known = true;
        cookable = isCookable;
        ordered = numberOfOrders > 0;
        invReqCount = ingredientQuantities;
        numOfOrders = numberOfOrders;
        numInInv = numberInInventory;
        indexInMenu = indexInRecipesMenu;
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
        if (known)
        {
            RecipeManager.instance.SetCurrentlySelectedRecipeInMenuUi(indexInMenu);
            DisplayRecipe();
        }
    }

    public void DisplayRecipe()
    {
        if (infoDisplay == null)
        {
            Debug.Log("RecipeUISlot: Missing information display!");
        }
        else
        {
            infoDisplay.ShowRecipe(selectedButton, recipe, invReqCount, numOfOrders, numInInv);
            RecipeManager.instance.SetCurrentlyCookingRecipe(recipe);
        }
    }

    public bool IsRecipeKnown()
    {
        return known;
    }

    public bool IsOrdered()
    {
        return ordered;
    }

    public bool IsCookable()
    {
        return cookable;
    }

    // Update the information of this recipe
    public void UpdateInfo()
    {
        List<(int, int)> ingIds = recipe.GetIngredientIds();
        List<(int, int, int)> invReq = new List<(int, int, int)>();
        if (InventoryManager.instance != null && OrdersManager.instance != null)
        {

            cookable = InventoryManager.instance.CheckIfItemsExist(ingIds, out invReq);
            numOfOrders = OrdersManager.instance.GetNumberOfOrders(recipe.recipeId);
            numInInv = InventoryManager.instance.GetAmountOfItem(recipe.cookedDishItem.inventoryItemId);
        }
        else if (TutorialInventoryManager.instance != null && TutorialOrdersManager.instance != null)
        {

            cookable = TutorialInventoryManager.instance.CheckIfItemsExist(ingIds, out invReq);
            numOfOrders = 1;
            numInInv = TutorialInventoryManager.instance.GetAmountOfItem(recipe.cookedDishItem.inventoryItemId);
        }
        invReqCount = invReq;
        ordered = numOfOrders > 0;
        UpdateUI();
    }

    public void ActivateSelectedButton()
    {
        selectedButton.SetActive(true);
    }

    public void DeactivateSelectedButton()
    {
        selectedButton.SetActive(false);
    }

    private void UpdateUI()
    {
        recipeIcon.sprite = recipe.cookedDishItem.icon;
        orderedIcon.SetActive(ordered);
        if (RecipeManager.instance != null)
        {
            cookableButton.SetActive(RecipeManager.instance.IsCookingActivated() && cookable);

        }
        else if (TutorialRecipeManager.instance != null)
        {
            cookableButton.SetActive(TutorialRecipeManager.instance.IsCookingActivated() && cookable);

        }
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
}
