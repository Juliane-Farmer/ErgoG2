using UnityEngine;
using System.Collections;

public class ManagingEnvironments : MonoBehaviour
{
    // Assign the spheres in the Inspector
    [Header("Environment Spheres")]
    public GameObject sphere1;  // Sphere for Key '1'
    public GameObject sphere2;  // Sphere for Key '2'
    public GameObject sphere3;  // Sphere for Key '3'
    public GameObject sphere4;  // Sphere for Key '4'
    public GameObject sphere5;  // Sphere for Key '5'
    public GameObject sphere6;  // Sphere for Key '6'
    public GameObject sphere7;  // Sphere for Key '7'
    public GameObject sphere8;  // Sphere for Key '8'
    public GameObject sphere9;  // Sphere for Key '9'

    [Header("Transition Settings")]
    public GameObject transitionSphere;    // Sphere for transitions
    public float transitionDuration = 2.0f; // Total duration of the transition (fade in and fade out)
    public Material transitionMaterial;    // Material for the transition sphere

    [Header("Special Effects")]
    public GameObject particleSystemObject; // Particle System to activate for sphere8

    private GameObject currentSphere;      // Currently active sphere
    private bool isTransitioning = false;  // Flag to prevent input during transitions

    void Start()
    {
        // Initialize: Activate sphere1 and deactivate others
        ActivateSphere(sphere1);
        currentSphere = sphere1;

        // Initialize the transition sphere's opacity to 0 (invisible)
        SetTransitionSphereOpacity(0);

        // Initialize Particle System to be inactive at start
        if (particleSystemObject != null)
        {
            particleSystemObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Particle System Object is not assigned.");
        }
    }

    void Update()
    {
        // Ignore input during transition
        if (isTransitioning) return;

        // Check for key inputs (1-9, 0)
        if (Input.GetKeyDown(KeyCode.Alpha1) && currentSphere != sphere1)
        {
            StartCoroutine(SwitchSphere(sphere1));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && currentSphere != sphere2)
        {
            StartCoroutine(SwitchSphere(sphere2));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && currentSphere != sphere3)
        {
            StartCoroutine(SwitchSphere(sphere3));
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && currentSphere != sphere4)
        {
            StartCoroutine(SwitchSphere(sphere4));
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && currentSphere != sphere5)
        {
            StartCoroutine(SwitchSphere(sphere5));
        }
        if (Input.GetKeyDown(KeyCode.Alpha6) && currentSphere != sphere6)
        {
            StartCoroutine(SwitchSphere(sphere6));
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) && currentSphere != sphere7)
        {
            StartCoroutine(SwitchSphere(sphere7));
        }
        if (Input.GetKeyDown(KeyCode.Alpha8) && currentSphere != sphere8)
        {
            StartCoroutine(SwitchSphere(sphere8));
        }
        if (Input.GetKeyDown(KeyCode.Alpha9) && currentSphere != sphere9)
        {
            StartCoroutine(SwitchSphere(sphere9));
        }
    }

    /// <summary>
    /// Coroutine to handle smooth transitions with fade-in and fade-out.
    /// </summary>
    /// <param name="newSphere">The sphere to switch to.</param>
    /// <returns></returns>
    IEnumerator SwitchSphere(GameObject newSphere)
    {
        isTransitioning = true;

        // Activate the transition sphere
        if (transitionSphere != null)
        {
            transitionSphere.SetActive(true);
        }
        else
        {
            Debug.LogError("Transition Sphere is not assigned.");
            isTransitioning = false;
            yield break;
        }

        // Step 1: Fade in the transition sphere
        yield return StartCoroutine(FadeTransitionSphere(0f, 1f));

        // Step 2: Switch the active sphere while transition sphere is fully visible
        ActivateSphere(newSphere);
        currentSphere = newSphere;

        // Handle special effects based on the selected sphere
        HandleSpecialEffects(newSphere);

        // Step 3: Fade out the transition sphere
        yield return StartCoroutine(FadeTransitionSphere(1f, 0f));

        // Step 4: Deactivate the transition sphere after fade-out
        if (transitionSphere != null)
        {
            transitionSphere.SetActive(false);
        }

        isTransitioning = false;
    }

    /// <summary>
    /// Activates the specified sphere and deactivates all others.
    /// Also manages special effects based on the active sphere.
    /// </summary>
    /// <param name="activeSphere">The sphere to activate.</param>
    void ActivateSphere(GameObject activeSphere)
    {
        // Deactivate all spheres
        if (sphere1 != null) sphere1.SetActive(false);
        if (sphere2 != null) sphere2.SetActive(false);
        if (sphere3 != null) sphere3.SetActive(false);
        if (sphere4 != null) sphere4.SetActive(false);
        if (sphere5 != null) sphere5.SetActive(false);
        if (sphere6 != null) sphere6.SetActive(false);
        if (sphere7 != null) sphere7.SetActive(false);
        if (sphere8 != null) sphere8.SetActive(false);
        if (sphere9 != null) sphere9.SetActive(false);

        // Activate the desired sphere
        if (activeSphere != null)
        {
            activeSphere.SetActive(true);
        }
        else
        {
            Debug.LogError("Attempted to activate a null sphere.");
        }
    }

    /// <summary>
    /// Coroutine to fade the transition sphere's material opacity.
    /// </summary>
    /// <param name="startOpacity">Starting opacity value.</param>
    /// <param name="targetOpacity">Target opacity value.</param>
    /// <returns></returns>
    IEnumerator FadeTransitionSphere(float startOpacity, float targetOpacity)
    {
        if (transitionMaterial == null)
        {
            Debug.LogError("Transition Material is not assigned.");
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < transitionDuration / 2f) // Half duration for fade-in or fade-out
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / (transitionDuration / 2f));
            float currentOpacity = Mathf.Lerp(startOpacity, targetOpacity, t);
            SetTransitionSphereOpacity(currentOpacity);
            yield return null;
        }

        // Ensure the final opacity is set
        SetTransitionSphereOpacity(targetOpacity);
    }

    /// <summary>
    /// Sets the opacity of the transition sphere's material.
    /// </summary>
    /// <param name="opacity">Desired opacity (0 to 1).</param>
    void SetTransitionSphereOpacity(float opacity)
    {
        if (transitionMaterial != null)
        {
            Color color = transitionMaterial.color;
            color.a = Mathf.Clamp01(opacity); // Ensure opacity is within [0,1]
            transitionMaterial.color = color;
        }
    }

    /// <summary>
    /// Handles special effects based on the active sphere.
    /// Specifically, activates the Particle System when sphere8 is active.
    /// </summary>
    /// <param name="activeSphere">The currently active sphere.</param>
    void HandleSpecialEffects(GameObject activeSphere)
    {
        if (particleSystemObject == null)
        {
            // Particle System not assigned; no action needed
            return;
        }

        if (activeSphere == sphere8)
        {
            // Activate Particle System for sphere8
            particleSystemObject.SetActive(true);
        }
        else
        {
            // Deactivate Particle System for all other spheres
            particleSystemObject.SetActive(false);
        }
    }
}