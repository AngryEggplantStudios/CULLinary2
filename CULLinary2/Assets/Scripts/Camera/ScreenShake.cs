using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private static ScreenShake _instance;
    public static ScreenShake Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void Shake(float duration, float magnitude, float fadeIn, float fadeOut)
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(ShakeSequence(duration, magnitude, fadeIn, fadeOut));
        }
        else
        {
            Debug.LogWarning("ScreenShake Object is not active!");
        }
    }

    private IEnumerator ShakeSequence(float duration, float magnitude, float fadeIn, float fadeOut)
    {
        Vector3 origPos = transform.localPosition;


        for (float elapsed = 0; elapsed < duration; elapsed += Time.unscaledDeltaTime)
        {
            float fadeInMultiplier = fadeIn == 0 ? 1 : Mathf.Lerp(0, 1, elapsed / fadeIn);
            float fadeOutMultiplier = fadeOut == 0 ? 1 : Mathf.Lerp(0, 1, (duration - elapsed) / fadeOut);
            float scale = fadeInMultiplier * fadeOutMultiplier * magnitude;
            float x = Random.Range(-1f, 1f) * scale;
            float y = Random.Range(-1f, 1f) * scale;

            transform.localPosition = new Vector3(x, y, origPos.z);

            yield return null;
        }

        transform.localPosition = origPos;
    }
}
