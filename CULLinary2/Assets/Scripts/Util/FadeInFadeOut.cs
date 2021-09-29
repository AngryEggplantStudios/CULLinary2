using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Taken from https://forum.unity.com/threads/simple-ui-animation-fade-in-fade-out-c.439825/
/// </summary>
public enum FadeAction
{
    FadeIn,
    FadeOut,
    FadeInAndOut,
    FadeOutAndIn
}


public class FadeInFadeOut : MonoBehaviour
{
    public bool isFinished = false;

    [Tooltip("The Fade Type.")]
    [SerializeField] private FadeAction fadeType;

    [Tooltip("the image you want to fade, assign in inspector")]
    [SerializeField] private Image img;

    [Tooltip("The amount of pause for the delay")]
    [SerializeField] private float fadePause;

    public delegate void FinishedDelegate(FadeAction type);
    public event FinishedDelegate OnFinished;

    public void OnEnable()
    {
        Activate();
    }

    public void Activate()
    {
        // Debug.Log("fade in fade out activated");

        if (fadeType == FadeAction.FadeIn)
        {

            StartCoroutine(FadeIn());

        }

        else if (fadeType == FadeAction.FadeOut)
        {

            StartCoroutine(FadeOut());

        }

        else if (fadeType == FadeAction.FadeInAndOut)
        {

            StartCoroutine(FadeInAndOut());

        }

        else if (fadeType == FadeAction.FadeOutAndIn)
        {

            StartCoroutine(FadeOutAndIn());

        }
    }

    // fade from transparent to opaque
    IEnumerator FadeIn()
    {
        // loop over 1 second
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            // set color with i as alpha
            // img.color = new Color(1, 1, 1, i);
            img.color = new Color(img.color.r, img.color.g, img.color.b, i);
            yield return null;
        }
        isFinished = true;
        OnFinished?.Invoke(FadeAction.FadeIn);
    }

    // fade from opaque to transparent
    IEnumerator FadeOut()
    {
        // loop over 1 second backwards
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            // img.color = new Color(1, 1, 1, i);
            img.color = new Color(img.color.r, img.color.g, img.color.b, i);
            yield return null;
        }
        isFinished = true;
        OnFinished?.Invoke(FadeAction.FadeOut);
    }

    IEnumerator FadeInAndOut()
    {
        // loop over 1 second
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            // set color with i as alpha
            img.color = new Color(1, 1, 1, i);
            yield return null;
        }

        //Temp to Fade Out
        yield return new WaitForSeconds(fadePause);

        // loop over 1 second backwards
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            img.color = new Color(1, 1, 1, i);
            yield return null;
        }
        isFinished = true;
    }

    IEnumerator FadeOutAndIn()
    {
        // loop over 1 second backwards
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            img.color = new Color(1, 1, 1, i);
            yield return null;
        }

        //Temp to Fade In
        yield return new WaitForSeconds(fadePause);

        // loop over 1 second
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            // set color with i as alpha
            img.color = new Color(1, 1, 1, i);
            yield return null;
        }
        isFinished = true;
    }

}