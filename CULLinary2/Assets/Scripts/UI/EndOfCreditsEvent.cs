using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfCreditsEvent : MonoBehaviour
{
    // Finds the UI Controller and attempts to call the OnEndCredits function
    public void OnEndCredits()
    {
        if (UIController.instance != null)
        {
            UIController.instance.OnEndCredits();
        }
    }
}
