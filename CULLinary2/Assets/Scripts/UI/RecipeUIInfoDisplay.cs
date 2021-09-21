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
    public TextMeshProUGUI orderCount;
    public string beforeOrderCount = "You have ";
    public string afterOrderCount = " order(s) for this dish.\nCook this dish at a campfire!";


    [Header("Parent Container for Ingredients")]
    public Transform ingredientsParentContainer;


    [Header("RecipeInfoDisplayIngredient Prefab")]
    public GameObject recipeInfoDisplayIngredient;
    

    private Recipe recipe;

    public void ShowRecipe(Recipe r, (int, bool)[] checkedIngredients, int numberOfOrders)
    {
        recipe = r;
        recipeIcon.sprite = recipe.cookedDishItem.icon;
        recipeName.text = recipe.cookedDishItem.itemName;
        recipeDescription.text = "good soup.";
        recipeEffect.text = "XXX when consumed";
        orderCount.text = beforeOrderCount + numberOfOrders + afterOrderCount;

        foreach (Transform child in ingredientsParentContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach ((int ingredientId, bool isPresent) in checkedIngredients)
        {
            GameObject ingredientEntry = Instantiate(recipeInfoDisplayIngredient,
                                                     new Vector3(0, 0, 0),
                                                     Quaternion.identity,
                                                     ingredientsParentContainer.transform) as GameObject;
            RecipeUIInfoDisplayIngredient infoDisplayIngredient =
                ingredientEntry.GetComponent<RecipeUIInfoDisplayIngredient>();
            infoDisplayIngredient.DisplayIngredient(ingredientId, isPresent);
        }
    }
}
