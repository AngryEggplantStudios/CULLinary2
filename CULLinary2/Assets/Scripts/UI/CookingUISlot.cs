using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingUISlot : MonoBehaviour
{
    public GameObject cookableButton;
    public Image recipeIcon;
    public TextMeshProUGUI recipeDescription;
    public GameObject orderedIcon;

    private RecipeUIInfoDisplay infoDisplay = null;
    private Recipe recipe;

    private bool cookable;
    private bool ordered;

    
    private (int, bool)[] checkedIngs;
    private int numOfOrders;

    public void AddRecipe(
        Recipe newRecipe,
        bool isCookable,
        (int, bool)[] checkedIngredients,
        int numberOfOrders
    )
    {
        recipe = newRecipe;
        cookable = isCookable;
        ordered = numberOfOrders > 0;
        checkedIngs = checkedIngredients;
        numOfOrders = numberOfOrders;
        UpdateUI();
    }

    public void SetInfoDisplay(RecipeUIInfoDisplay display)
    {
        infoDisplay = display;
    }

    public void SelectRecipeForCooking()
    {
        // TODO: Select recipe and cook it
        if (infoDisplay == null)
        {
            Debug.Log("CookingUISlot: Missing information display!");
        }
        else
        {
            infoDisplay.ShowRecipe(recipe, checkedIngs, numOfOrders, cookable);
            RecipeManager.instance.SetCurrentlyCookingRecipe(recipe);
        }
    }

    private void UpdateUI()
    {
        recipeIcon.sprite = recipe.cookedDishItem.icon;
        recipeDescription.text = recipe.cookedDishItem.itemName;
        cookableButton.SetActive(cookable);
        orderedIcon.SetActive(ordered);
    }
}
