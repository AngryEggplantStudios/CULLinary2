using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RecipeSlot : MonoBehaviour, IPointerEnterHandler,
                          IPointerExitHandler, IPointerClickHandler 
{
    public Image recipeIcon;
    public Text recipeDescription;
    public Color hoverColour = Color.white;

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

        InventoryManager inventory = InventoryManager.instance; 
        bool didCookingSucceed = inventory.RemoveIdsFromInventory(ingredientsRequired);
        if (!didCookingSucceed)
        {
            // Player does not have ingredients
            Debug.Log("OOPS! You don't have the ingredients!");
            return;
        }
        
        // Cooking succeeded, give the player the food
        inventory.AddItem(recipe.cookedRecipeItem);
    }

    public void AddRecipe(Recipe newRecipe)
    {
        recipe = newRecipe;
        recipeIcon.sprite = recipe.cookedRecipeItem.icon;
        recipeDescription.text = recipe.GetRecipeName();
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
}
