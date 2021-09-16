using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class RecipeUIManager : MonoBehaviour
{
    public GameObject cookableButton;
    public Image recipeIcon;
    public TextMeshProUGUI recipeDescription;
    public GameObject orderedIcon;

    [Header("Variables")]
    public bool cookable;
    public bool ordered;

    void Update()
    {
        cookableButton.SetActive(cookable);
        orderedIcon.SetActive(ordered);
    }
}
