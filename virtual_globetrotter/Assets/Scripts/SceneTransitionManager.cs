using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class SceneTransitionManager : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup; // Assign the UI Panel's Canvas Group
    public float fadeDuration = 1.0f;

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoadScene(sceneName));
    }

    IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        // Fade out
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        // Load new scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // Fade in
        elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = 1.0f - Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
    }
}
