using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecipeBook : MonoBehaviour
{
    public GameObject recipeBookUi;
    private KeyCode openRecipeBookKeyCode;

    private void Awake()
    {
        openRecipeBookKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.OpenRecipeBook);
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
