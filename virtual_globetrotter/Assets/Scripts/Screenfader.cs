using UnityEngine;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup; // Drag the Canvas Group here
    public float fadeDuration = 1.0f; // Duration of fade

    // Fade out to black
    public IEnumerator FadeOut()
    {
        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
    }

    // Fade in from black
    public IEnumerator FadeIn()
    {
        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = 1.0f - Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
    }
}
