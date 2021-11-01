using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewspaperDetails : MonoBehaviour
{
    public TextMeshProUGUI headlineText;
    public TextMeshProUGUI subheadText;
    public TextMeshProUGUI dayNumberText;
    public Image mainImage;
    public Image mainImageBg;
    [Header("Recipes")]
    public GameObject allRecipesParent;
    public GameObject recipes2And3Parent;
    public GameObject bigRecipe2;
    public GameObject recipe2;
    public GameObject recipe3;
    [Header("Recipe Image and Text")]
    public Image recipe1Dish;
    public TextMeshProUGUI recipe1Text;
    public Image bigRecipe2Dish;
    public TextMeshProUGUI bigRecipe2Text;
    public Image recipe2Dish;
    public TextMeshProUGUI recipe2Text;
    public Image recipe3Dish;
    public TextMeshProUGUI recipe3Text;


    public void UpdateNewspaperIssueUI(NewsIssue ni)
    {
        headlineText.text = ni.headlines.ToUpper();
        subheadText.text = ni.subhead.ToUpper();
        dayNumberText.text = "Day " + GameTimer.GetDayNumber();
        AddToImageIfPossible(mainImage, ni.mainImage);
        AddToImageIfPossible(mainImageBg, ni.imageBackground);
        AddRecipes(ni.recipesUnlocked);
    }

    // Adds a sprite to an image if it is possible
    // If the sprite is None, disable the image entirely
    // by setting its opacity to 0
    private void AddToImageIfPossible(Image img, Sprite spr)
    {
        float opacity = spr ? 1.0f : 0.0f;
        img.sprite = spr;
        img.color = new Color(img.color.r, img.color.g, img.color.b, opacity);
    }

    // Gets a recipe from the database and gets the cooked dish
    private InventoryItem ToItem(int recipeId)
    {
        return DatabaseLoader.GetRecipeById(recipeId).cookedDishItem;
    }

    // Adds recipes to the newspaper
    private void AddRecipes(int[] recipes)
    {
        if (recipes.Length == 0)
        {
            allRecipesParent.SetActive(false);
            return;
        }

        allRecipesParent.SetActive(true);
        InventoryItem r1 = ToItem(recipes[0]);
        recipe1Dish.sprite = r1.icon;
        recipe1Text.text = r1.itemName;
        if (recipes.Length == 1)
        {
            recipes2And3Parent.SetActive(false);
            return;
        }
        
        recipes2And3Parent.SetActive(true);
        InventoryItem r2 = ToItem(recipes[1]);
        if (recipes.Length == 2)
        {
            bigRecipe2.SetActive(true);
            recipe2.SetActive(false);
            recipe3.SetActive(false);

            bigRecipe2Dish.sprite = r2.icon;
            bigRecipe2Text.text = r2.itemName;
        }
        else
        {
            bigRecipe2.SetActive(false);
            recipe2.SetActive(true);
            recipe3.SetActive(true);

            // recipes.Length > 2
            // Just take the first 3
            InventoryItem r3 = ToItem(recipes[2]);

            recipe2Dish.sprite = r2.icon;
            recipe2Text.text = r2.itemName;
            recipe3Dish.sprite = r3.icon;
            recipe3Text.text = r3.itemName;
        }
    }
}
