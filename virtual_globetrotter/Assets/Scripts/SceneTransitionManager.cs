using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup; // Assign the UI Panel's Canvas Group
    public float fadeDuration = 1.0f; // Duration for fade transitions

    // Change scenes with a fade effect
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // Fade out (to black)
        yield return StartCoroutine(Fade(1.0f));

        // Load the new scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // Fade in (from black)
        yield return StartCoroutine(Fade(0.0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeScene("EiffelTowerSnowyScene"); // Replace "EiffelTowerSnowyScene" with your actual scene name
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeScene("SanFranciscoSunnyScene"); // Replace "SanFranciscoSunnyScene" with your actual scene name
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        // Ensure the fadeCanvasGroup color is set to black
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.GetComponent<UnityEngine.UI.Image>().color = Color.black; // Or any desired color
        }
    }
}
