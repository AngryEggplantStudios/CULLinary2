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

    public List<Item> GetIngredients()
    {
        return new List<Item>(recipe.ingredientList);
    }

    public Item GetProduct()
    {
        return recipe.cookedRecipeItem;
    }

    public string GetDescription()
    {
        return description;
    }

    public int GetSubmissionStationId()
    {
        return submissionStationId;
    }
}
