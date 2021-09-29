using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionManager : SingletonGeneric<SceneTransitionManager>
{
    [SerializeField] private Animator fadeIn;
    [SerializeField] private GameObject fadeInCanvas;
    [SerializeField] private Animator fadeOut;
    [SerializeField] private GameObject fadeOutCanvas;
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

    public void FadeSceneIn()
    {
        fadeInChild.SetActive(true);
        Debug.Log("end fade in");
        // yield return new WaitForSeconds(1);
        // fadeInCanvas.SetActive(true);
        // Debug.Log("starting fade in playback");
        // fadeIn.StartPlayback();
        // // Debug.Log("playback time: " + fadeIn.playbackTime);
        // // yield return new WaitForSeconds(0.5f);
        // fadeInCanvas.SetActive(false);

    }

    public void FadeSceneOut()
    {
        fadeOutChild.SetActive(true);
        Debug.Log("end fade out");

        // fadeOutCanvas.SetActive(true);
        // fadeOut.StartPlayback();
        // // yield return new WaitForSeconds(0.5f);
        // fadeOutCanvas.SetActive(false);
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
            Debug.Log("fade finished, hiding fade in");
            fadeInChild.SetActive(false);
        }

        if (type == FadeAction.FadeOut)
        {
            Debug.Log("fade finished, hiding fade out");
            fadeOutChild.SetActive(false);
        }
    }
}
