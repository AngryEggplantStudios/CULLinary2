using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionManager : SingletonGeneric<SceneTransitionManager>
{
    private FadeInFadeOut fadeInScript;
    private GameObject fadeInChild;
    private FadeInFadeOut fadeOutScript;
    private GameObject fadeOutChild;

    void Start()
    {
        fadeInChild = transform.GetChild(0).gameObject;
        fadeInScript = fadeInChild.GetComponent<FadeInFadeOut>();
        fadeOutChild = transform.GetChild(1).gameObject;
        fadeOutScript = fadeOutChild.GetComponent<FadeInFadeOut>();
        fadeInScript.OnFinished += HideFade;
        fadeOutScript.OnFinished += HideFade;
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
    }
}
