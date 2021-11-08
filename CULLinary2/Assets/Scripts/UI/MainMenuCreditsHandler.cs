using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCreditsHandler : MonoBehaviour
{
    [SerializeField] private GameObject credits;
    [SerializeField] private Animator creditsAnimator;

    private KeyCode closeMenuKeybind;

    private void Start()
    {
        closeMenuKeybind = PlayerKeybinds.GetKeybind(KeybindAction.CloseMenu);
    }
    private void Update()
    {
        if (credits.activeSelf && Input.GetKeyDown(closeMenuKeybind))
        {
            credits.SetActive(false);
        }
    }

    public void PlayCredits()
    {
        creditsAnimator.SetBool("TurnBlack", true);
        credits.SetActive(true);
    }
}
