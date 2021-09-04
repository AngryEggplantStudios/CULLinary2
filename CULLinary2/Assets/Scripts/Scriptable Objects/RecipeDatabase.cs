using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe Database", menuName = "Database/Recipe Database")]
public class RecipeDatabase : ScriptableObject
{
    public List<Recipe> recipes;
}
