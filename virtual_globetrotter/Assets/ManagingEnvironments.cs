using UnityEngine;
using System.Collections;

public class ManagingEnvironments : MonoBehaviour
{
    // Assign the spheres in the Inspector
    public GameObject sphere1; // Sphere for "W"
    public GameObject sphere2; // Sphere for "S"
    public GameObject sphere3; // Sphere for "R"
    public GameObject transitionSphere; // Sphere for transitions

    public float transitionDuration = 2.0f; // Total duration of the transition (fade in and fade out)
    public Material transitionMaterial; // Material for the transition sphere

    private GameObject currentSphere; // Currently active sphere
    private bool isTransitioning = false; // Flag to prevent input during transitions

    void Start()
    {
        // Ensure only the first sphere is active initially
        currentSphere = sphere1;
        ActivateSphere(sphere1);

        // Initialize the transition sphere's opacity to 0
        SetTransitionSphereOpacity(0);
    }

    void Update()
    {
        // Ignore input during transition
        if (isTransitioning) return;

        // Check for key inputs
        if (Input.GetKeyDown(KeyCode.W) && currentSphere != sphere1)
        {
            StartCoroutine(SwitchSphere(sphere1));
        }
        if (Input.GetKeyDown(KeyCode.S) && currentSphere != sphere2)
        {
            StartCoroutine(SwitchSphere(sphere2));
        }
        if (Input.GetKeyDown(KeyCode.R) && currentSphere != sphere3)
        {
            StartCoroutine(SwitchSphere(sphere3));
        }
    }

    // Coroutine to handle smooth transitions with fade-in and fade-out
    IEnumerator SwitchSphere(GameObject newSphere)
    {
        isTransitioning = true;

        // Activate the transition sphere
        transitionSphere.SetActive(true);

        // Step 1: Fade in the transition sphere
        yield return StartCoroutine(FadeTransitionSphere(0, 1)); // Fade to 100% opacity

        // Step 2: Switch the sphere while transition sphere is fully visible
        ActivateSphere(newSphere);
        transitionSphere.SetActive(true);

        // Step 3: Fade out the transition sphere
        yield return StartCoroutine(FadeTransitionSphere(1, 0)); // Fade back to 0% opacity

        // Step 4: Deactivate the transition sphere after fade-out
        transitionSphere.SetActive(false);

        // Update the currently active sphere
        currentSphere = newSphere;

        isTransitioning = false;
    }

    // Method to activate one sphere and deactivate the others
    void ActivateSphere(GameObject activeSphere)
    {
        // Disable all spheres
        if (sphere1 != null) sphere1.SetActive(false);
        if (sphere2 != null) sphere2.SetActive(false);
        if (sphere3 != null) sphere3.SetActive(false);
        if (transitionSphere != null) transitionSphere.SetActive(false);

        // Enable the active sphere
        if (activeSphere != null) activeSphere.SetActive(true);
    }

    // Coroutine to fade the transition sphere's material opacity
    IEnumerator FadeTransitionSphere(float startOpacity, float targetOpacity)
    {
        float elapsed = 0;

        // Fade in or out based on the start and target opacity
        while (elapsed < transitionDuration / 2) // Divide by 2 for fade-in or fade-out
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / (transitionDuration / 2)); // Normalize time
            SetTransitionSphereOpacity(Mathf.Lerp(startOpacity, targetOpacity, t));
            yield return null;
        }

        // Ensure the final opacity is set
        SetTransitionSphereOpacity(targetOpacity);
    }

    // Set the opacity of the transition sphere's material
    void SetTransitionSphereOpacity(float opacity)
    {
        if (transitionMaterial != null)
        {
            Color color = transitionMaterial.color;
            color.a = opacity; // Update the alpha value
            transitionMaterial.color = color;
        }
    }
}
