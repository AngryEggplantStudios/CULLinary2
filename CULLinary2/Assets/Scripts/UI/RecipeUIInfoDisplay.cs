using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RecipeUIInfoDisplay : MonoBehaviour
{
    public Image recipeIcon;
    public TextMeshProUGUI recipeName;
    public TextMeshProUGUI recipeDescription;
    public TextMeshProUGUI recipeEffect;

    [Header("Order Information")]
    public TextMeshProUGUI orderAndInventoryCount;
    public string beforeOrderCount = "You have <b>";
    public string afterOrderCount = "</b> ";
    public string orderWordZero = "orders";
    public string orderWordSingular = "order";
    public string orderWordPlural = "orders";
    public string afterOrderWord = " for this dish.\n<b>";
    public string afterInventoryCount = "</b> in inventory.";


    [Header("Parent Container for Ingredients")]
    public Transform ingredientsParentContainer;


    [Header("RecipeInfoDisplayIngredient Prefab")]
    public GameObject recipeInfoDisplayIngredient;
    

    private Recipe recipe;
    // Currently selected button of the selected recipe
    private GameObject currentSelectedSlot = null;
    // Currently shown amount of inventory items
    private int currentInInventory = 0;
    // Currently shown number of orders
    private int currentNumberOfOrders = 0;
    private string orderWord = "";
    // CUrrently shown ingredients
    private List<RecipeUIInfoDisplayIngredient> ingredients = new List<RecipeUIInfoDisplayIngredient>();

    // Reset the recipes filter on disable
    public void OnDisable()
    {
        RecipeManager.instance.ResetAllRecipesInUI();
    }

    public void ShowRecipe(GameObject selectedButton,
                           Recipe r,
                           List<(int, int, int)> ingredientsCount,
                           int numberOfOrders,
                           int numberInInventory)
    {
        if (currentSelectedSlot != null)
        {
            currentSelectedSlot.SetActive(false);
        }
        selectedButton.SetActive(true);
        currentSelectedSlot = selectedButton;

        recipe = r;
        recipeIcon.sprite = recipe.cookedDishItem.icon;
        recipeName.text = recipe.cookedDishItem.itemName;
        recipeDescription.text = recipe.cookedDishItem.description;
        recipeEffect.text = recipe.cookedDishItem.GetConsumeEffect();

        if (numberOfOrders == 0)
        {
            orderWord = orderWordZero;
        }
        else if (numberOfOrders == 1)
        {
            orderWord = orderWordSingular;
        }
        else
        {
            orderWord = orderWordPlural;
        }
        orderAndInventoryCount.text = beforeOrderCount + numberOfOrders + afterOrderCount + orderWord +
                                      afterOrderWord + numberInInventory + afterInventoryCount;
        currentInInventory = numberInInventory;
        currentNumberOfOrders = numberOfOrders;

        foreach (Transform child in ingredientsParentContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach ((int ingredientId, int amount, int required) in ingredientsCount)
        {
            GameObject ingredientEntry = Instantiate(recipeInfoDisplayIngredient,
                                                     new Vector3(0, 0, 0),
                                                     Quaternion.identity,
                                                     ingredientsParentContainer.transform) as GameObject;
            RecipeUIInfoDisplayIngredient infoDisplayIngredient =
                ingredientEntry.GetComponent<RecipeUIInfoDisplayIngredient>();
            infoDisplayIngredient.DisplayIngredient(ingredientId, amount, required);
            ingredients.Add(infoDisplayIngredient);
        }
    }

    // Small hack to make cooking more interactive
    public void IncreaseInventoryCountAndSetIngredients()
    {
        currentInInventory++;
        orderAndInventoryCount.text = beforeOrderCount + currentNumberOfOrders + afterOrderCount + orderWord +
                                      afterOrderWord + currentInInventory + afterInventoryCount;
        foreach (RecipeUIInfoDisplayIngredient ing in ingredients)
        {
            ing.DeductIngredients();
        }
    }
}
