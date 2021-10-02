using System.Collections.Generic;

// A simple class to encapsulate an order
public class Order
{
    private Recipe recipe;
    private string description;
    private int submissionStationId;
    private int deliveryReward;

    public Order(Recipe rec, string desc, int submStn)
    {
        recipe = rec;
        description = desc;
        submissionStationId = submStn;
        deliveryReward = rec.recipeEarnings;
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

    // Sets a custom cash amount as the reward for completing this order
    public void SetDeliveryReward(int newReward)
    {
        deliveryReward = newReward;
    }

    // Gets the reward for completing this order
    public int GetDeliveryReward()
    {
        return deliveryReward;
    }
}
