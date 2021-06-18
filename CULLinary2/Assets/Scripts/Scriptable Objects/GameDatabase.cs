using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game Data containing all scriptable objects to be loaded before the game starts.
/// </summary>
[CreateAssetMenu(fileName = "New Database", menuName = "Game Database")]
public class GameDatabase : ScriptableObject
{
  public List<Ingredient> allIngredients;
}
