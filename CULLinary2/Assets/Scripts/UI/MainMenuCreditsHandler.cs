using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCreditsHandler : MonoBehaviour
{
    [SerializeField] private GameObject credits;
    [SerializeField] private Animator creditsAnimator;

    public void PlayCredits()
    {
        creditsAnimator.SetBool("TurnBlack", true);
        credits.SetActive(true);
    }
}
