using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RecipeSlot : MonoBehaviour, IPointerEnterHandler,
                          IPointerExitHandler, IPointerClickHandler
{
    public Image recipeIcon;
    public Text recipeDescription;
    public Color hoverColour = Color.white;

    // Container to attach ingredients to
    public GameObject ingredientsContainer;

    // Prefab for ingredient icons
    public GameObject recipeIngredientSlot;

    private Recipe recipe;
    private RecipeManager recipeManager;
    private Color originalTextColour;


    private void Start()
    {
        originalTextColour = recipeDescription.color;
        recipeManager = RecipeManager.Instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        recipeDescription.color = hoverColour;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        recipeDescription.color = originalTextColour;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        int[] ingredientsRequired = recipe.GetIngredientIds();
        if (!recipeManager.IsCookingActivated())
        {
            // Player is not at cooking station
            Debug.Log("OOPS! Not at campfire!");
            return;
        }

        InventoryManagerTwo inventory = InventoryManagerTwo.instance;
        bool didCookingSucceed = inventory.RemoveItemArrayFromInventory(ingredientsRequired);
        if (!didCookingSucceed)
        {
            // Player does not have ingredients
            Debug.Log("OOPS! You don't have the ingredients!");
            return;
        }

        // Cooking succeeded, give the player the food
        inventory.AddItem(recipe.cookedDishItem);
    }

    public void AddRecipe(Recipe newRecipe)
    {
        recipe = newRecipe;
        recipeIcon.sprite = recipe.cookedDishItem.icon;
        recipeDescription.text = recipe.cookedDishItem.itemName;
        UpdateIngredientsUI();
    }

    public void HideRecipe()
    {
        recipeIcon.enabled = false;
        recipeDescription.enabled = false;
    }

    public void ShowRecipe()
    {
        recipeIcon.enabled = true;
        recipeDescription.enabled = true;
    }

    public Recipe GetRecipe()
    {
        return recipe;
    }

    private void UpdateIngredientsUI()
    {
        foreach (Transform child in ingredientsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (int ingredientId in recipe.GetIngredientIds())
        {
            GameObject ingredientEntry = Instantiate(recipeIngredientSlot,
                                                     new Vector3(0, 0, 0),
                                                     Quaternion.identity,
                                                     ingredientsContainer.transform) as GameObject;
            Image ingredientIcon = ingredientEntry.GetComponent<Image>();
            ingredientIcon.sprite = DatabaseLoader.GetItemById(ingredientId).icon;
        }
    }
}
