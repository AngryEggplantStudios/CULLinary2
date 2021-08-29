using System.Collections.Generic;

// A simple class to encapsulate an order
public class Order
{
    private List<Item> ingredients;
    private Item product;
    private string description;
    private int submissionStationId;

    public Order(List<Item> ings, Item prod, string desc, int submStn)
    {
        ingredients = ings;
        product = prod;
        description = desc;
        submissionStationId = submStn;
    }

    public List<Item> GetIngredients()
    {
        return ingredients;
    }

    public Item GetProduct()
    {
        return product;
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
