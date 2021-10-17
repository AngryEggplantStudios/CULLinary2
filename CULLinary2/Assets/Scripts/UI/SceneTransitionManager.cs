using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionManager : SingletonGeneric<SceneTransitionManager>
{
    private FadeInFadeOut fadeInScript;
    private GameObject fadeInChild;
    private FadeInFadeOut fadeOutScript;
    private GameObject fadeOutChild;
    private FadeInFadeOut fadeInAndOutScript;
    private GameObject fadeInAndOutChild;

    private bool isFadingInAndOut = false;

    void Start()
    {
        fadeInChild = transform.GetChild(0).gameObject;
        fadeInScript = fadeInChild.GetComponent<FadeInFadeOut>();
        fadeOutChild = transform.GetChild(1).gameObject;
        fadeOutScript = fadeOutChild.GetComponent<FadeInFadeOut>();
        fadeInAndOutChild = transform.GetChild(2).gameObject;
        fadeInAndOutScript = fadeInAndOutChild.GetComponent<FadeInFadeOut>();
        fadeInScript.OnFinished += HideFade;
        fadeOutScript.OnFinished += HideFade;
        fadeInAndOutScript.OnFinished += HideFade;
        fadeInAndOutScript.OnFinished += _ =>
        {
            // Reset the Action for fading in and fading out
            fadeInAndOutScript.OnPauseBetweenFadeInAndFadeOut = () => {};
            // Allow it to run again
            isFadingInAndOut = false;
        };
    }

    public void FadeInImage()
    {
        fadeInChild.SetActive(true);
    }

    public void FadeOutImage()
    {
        fadeOutChild.SetActive(true);
    }

    private void OnDestroy()
    {
        fadeInScript.OnFinished -= HideFade;
        fadeOutScript.OnFinished -= HideFade;
        fadeInAndOutScript.OnFinished -= HideFade;
    }

    private void HideFade(FadeAction type)
    {
        if (type == FadeAction.FadeIn)
        {
            fadeInChild.SetActive(false);
        }

        if (type == FadeAction.FadeOut)
        {
            fadeOutChild.SetActive(false);
        }

        if (type == FadeAction.FadeInAndOut)
        {
            fadeInAndOutChild.SetActive(false);
        }
    }

    // Action will be called in between the fade in and fade out
    public void FadeInAndFadeOut(Action action)
    {
        // This check prevents spamming the fading in and out
        if (!isFadingInAndOut)
        {
            fadeInAndOutScript.OnPauseBetweenFadeInAndFadeOut += action;
            isFadingInAndOut = true;
            fadeInAndOutChild.SetActive(true);
        }
    }
}
