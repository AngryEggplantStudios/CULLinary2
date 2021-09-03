using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Database to contain all the recipes
public class RecipeDatabase : ScriptableObject
{
    // Recipes are specified in Unity interface
    public List<Recipe> recipes;
}
