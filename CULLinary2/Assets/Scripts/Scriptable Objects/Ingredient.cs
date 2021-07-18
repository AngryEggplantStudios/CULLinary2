using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ingredient class
/// </summary>
[CreateAssetMenu(fileName = "New Ingredient", menuName = "Ingredient")]
public class Ingredient : ScriptableObject
{
    new public string name = "New Ingredient";
    public Sprite icon;
    public int id;
    public ScriptableEnum type = ScriptableEnum.INGREDIENT;
}
