using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    private static ScreenFlash _instance;
    public static ScreenFlash Instance { get { return _instance; } }

    private Image image;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        image = GetComponent<Image>();
    }

    public void Flash(Color color, float duration, float magnitude, float fadeIn, float fadeOut)
    {
        StartCoroutine(FlashSequence(color, duration, magnitude, fadeIn, fadeOut));
    }

    public void Flash(float duration, float magnitude, float fadeIn, float fadeOut)
    {
        StartCoroutine(FlashSequence(image.color, duration, magnitude, fadeIn, fadeOut));
    }
    
    private IEnumerator FlashSequence(Color color, float duration, float magnitude, float fadeIn, float fadeOut)
    {
        Color tempColor = color;
        
        for (float elapsed = 0; elapsed < duration; elapsed += Time.unscaledDeltaTime)
        {
            float fadeInMultiplier = fadeIn == 0 ? 1 : Mathf.Lerp(0, 1, elapsed / fadeIn);
            float fadeOutMultiplier = fadeOut == 0 ? 1 : Mathf.Lerp(0, 1, (duration - elapsed) / fadeOut);
            float alpha = fadeInMultiplier * fadeOutMultiplier * magnitude;
            
            tempColor.a = alpha;
            image.color = tempColor;

            yield return null;
        }

        tempColor.a = 0;
        image.color = tempColor;
    }
}
