using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfCreditsEvent : MonoBehaviour
{
    [SerializeField] private GameObject rollingCreditsToHide;

    // Finds the UI Controller and attempts to call the OnEndCredits function
    public void OnEndCredits()
    {
        if (UIController.instance != null)
        {
            // Main Scene credits
            UIController.instance.OnEndCredits();
        }
        else
        {
            // Main Menu credits
            rollingCreditsToHide.SetActive(false);
        }
    }
}
