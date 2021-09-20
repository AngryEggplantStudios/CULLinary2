using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecipeBook : MonoBehaviour
{
    // Entire recipe book
    public GameObject recipeBookUi;

    private KeyCode openRecipeBookKeyCode;
    private RecipeManager recipeManager;

    private void Awake()
    {
        openRecipeBookKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenRecipeBook);
    }

    private void Start()
    {
        recipeManager = RecipeManager.Instance;
    }

    public void ToggleRecipeBook()
    {
        recipeBookUi.SetActive(!recipeBookUi.activeSelf);
    }

    public void OpenRecipeBook()
    {
        recipeBookUi.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(openRecipeBookKeyCode))
        {
            ToggleRecipeBook();
        }
    }
}
