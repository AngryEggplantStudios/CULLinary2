using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecipeBook : MonoBehaviour
{
    // Entire recipe book
    public GameObject recipeBookUi;
    // Cooking indicator
    public GameObject cookingIndicator;

    private KeyCode openRecipeBookKeyCode;
    private RecipeManager recipeManager;

    private void Awake()
    {
        openRecipeBookKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenRecipeBook);
    }

    private void Start()
    {
        recipeManager = RecipeManager.Instance;
        UpdateUI();
    }

    public void ToggleRecipeBook()
    {
        recipeBookUi.SetActive(!recipeBookUi.activeSelf);
        UpdateUI();
    }

    public void OpenRecipeBook()
    {
        recipeBookUi.SetActive(true);
        UpdateUI();
    }

    public void UpdateUI()
    {
        cookingIndicator.SetActive(recipeManager.IsCookingActivated());
    }

    private void Update()
    {
        if (Input.GetKeyDown(openRecipeBookKeyCode))
        {
            ToggleRecipeBook();
        }
    }
}
