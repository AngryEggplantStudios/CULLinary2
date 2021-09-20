using System.Collections.Generic;

// A simple class to encapsulate an order
public class Order
{
    private Recipe recipe;
    private string description;
    private int submissionStationId;

    public Order(Recipe rec, string desc, int submStn)
    {
        recipe = rec;
        description = desc;
        submissionStationId = submStn;
    }

    public List<InventoryItem> GetIngredients()
    {
        return new List<InventoryItem>(recipe.ingredientList);
    }

    public int[] GetIngredientIds()
    {
        return recipe.GetIngredientIds();
    }

    public InventoryItem GetProduct()
    {
        return recipe.cookedDishItem;
    }

    public string GetDescription()
    {
        return description;
    }

    public int GetSubmissionStationId()
    {
        return submissionStationId;
    }

    public Recipe GetRecipe()
    {
        return recipe;
    }
}
