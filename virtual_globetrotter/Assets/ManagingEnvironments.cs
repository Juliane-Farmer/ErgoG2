using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using NUnit.Framework.Constraints;

public class ManagingEnvironments : MonoBehaviour
{
    // Assign the spheres in the Inspector
    [Header("Environment Spheres")]
    public GameObject sphere0;  // Sphere for Key 'Backspace' (or your chosen key)
    public GameObject sphere1;  // Sphere for Key '1'
    public GameObject sphere2;  // Sphere for Key '2'
    public GameObject sphere3;  // Sphere for Key '3'
    public GameObject sphere4;  // Sphere for Key '4'
    public GameObject sphere5;  // Sphere for Key '5'
    public GameObject sphere6;  // Sphere for Key '6'
    public GameObject sphere7;  // Sphere for Key '7'
    public GameObject sphere8;  // Sphere for Key '8'
    public GameObject sphere9;  // Sphere for Key '9'
    public GameObject sphere10; // Sphere for Key '0'
    [Header("WorldMap Settings")]
    public GameObject image; 
    private Image imgComponent;
    private Color originalColor;
    public GameObject button; 

    [Header("Transition Settings")]
    public GameObject transitionSphere;    // Sphere for transitions
    public float transitionDuration = 2.0f; // Total duration of the transition (fade in and fade out)
    public Material transitionMaterial;    // Material for the transition sphere

    private GameObject currentSphere;      // Currently active sphere
    private bool isTransitioning = false;  // Flag to prevent input during transitions

    void Start()
    {
        // Initialize: Activate sphere0 and deactivate others
        ActivateSphere(sphere0);
        currentSphere = sphere0;

        // Initialize the transition sphere's opacity to 0 (invisible)
        SetTransitionSphereOpacity(0);

        imgComponent = image.GetComponent<Image>();
        originalColor = imgComponent.color;
    }

    void Update()
    {
        // Ignore input during transition
        if (isTransitioning) return;

        // Check for key inputs (Backspace, 1-9, 0)
        if (Input.GetKeyDown(KeyCode.M) && currentSphere != sphere0)
        {
            StartCoroutine(SwitchSphere(sphere0));
            image.SetActive(true);
            Image imgComponent = image.GetComponent<Image>();
            Color originalColor = imgComponent.color;
            imgComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 255f);
            button.SetActive(true);
        }
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
        if (Input.GetKeyDown(KeyCode.Alpha0) && currentSphere != sphere10)
        {
            StartCoroutine(SwitchSphere(sphere10));
        }
    }

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

        // Step 3: Fade out the transition sphere
        yield return StartCoroutine(FadeTransitionSphere(1f, 0f));

        // Step 4: Deactivate the transition sphere after fade-out
        if (transitionSphere != null)
        {
            transitionSphere.SetActive(false);
        }

        isTransitioning = false;
    }


    void ActivateSphere(GameObject activeSphere)
    {
        // Deactivate all spheres
        if (sphere0 != null) sphere0.SetActive(false);
        if (sphere1 != null) sphere1.SetActive(false);
        if (sphere2 != null) sphere2.SetActive(false);
        if (sphere3 != null) sphere3.SetActive(false);
        if (sphere4 != null) sphere4.SetActive(false);
        if (sphere5 != null) sphere5.SetActive(false);
        if (sphere6 != null) sphere6.SetActive(false);
        if (sphere7 != null) sphere7.SetActive(false);
        if (sphere8 != null) sphere8.SetActive(false);
        if (sphere9 != null) sphere9.SetActive(false);
        if (sphere10 != null) sphere10.SetActive(false);
        if (activeSphere != null) activeSphere.SetActive(true);
    }

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

    void SetTransitionSphereOpacity(float opacity)
    {
        if (transitionMaterial != null)
        {
            Color color = transitionMaterial.color;
            color.a = Mathf.Clamp01(opacity); // Ensure opacity is within [0,1]
            transitionMaterial.color = color;
        }
    }

    public void SwitchToSphere1()
    {
        if (!isTransitioning)
        {
            StartCoroutine(FadeOutImageCoroutine());
        }
        else
        {
            Debug.LogWarning("Cannot initiate fade-out and switch: A transition is already in progress.");
        }
    }


    IEnumerator FadeOutImageCoroutine()
    {
        if (image == null)
        {
            Debug.LogError("Image GameObject is not assigned.");
            yield break;
        }

        // Begin transition
        isTransitioning = true;

        // Get the Image component (assuming it's a UI Image)
        if (imgComponent == null)
        {
            Debug.LogError("The assigned image GameObject does not have an Image component.");
            isTransitioning = false;
            yield break;
        }

        // Duration for the fade-out effect
        float fadeDuration = 1f; // You can adjust this as needed
        float elapsedTime = 0f;

        // Store the original color

        // Fade out the image
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeDuration);
            Debug.Log(alpha);
            imgComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure the alpha is set to 0
        imgComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // Optionally deactivate the image GameObject after fading out
        image.SetActive(false);
        button.SetActive(false);

        // Start rotating sphere0 with increasing speed
        if (sphere0 != null)
        {
            StartCoroutine(RotateSphere0Coroutine());
        }
        else
        {
            Debug.LogError("Sphere0 is not assigned.");
        }
    }

    IEnumerator RotateSphere0Coroutine()
    {
        if (sphere0 == null)
        {
            Debug.LogError("Sphere0 is not assigned.");
            yield break;
        }

        // Define rotation parameters
        float rotationDuration = 8f;        // Total rotation time in seconds
        float initialSpeed = 10f;            // Initial rotation speed in degrees per second
        float finalSpeed = 1000f;             // Maximum rotation speed in degrees per second
        float elapsedTime = 0f;
        bool hasTriggeredTransition = false;

        // Define transition parameters
        float transitionStartTime = rotationDuration / 4f; // Time to start the transition
        float transitionElapsed = 0f;
        bool isFadingIn = false;
        bool fadedIn = false;

        // Initialize transition sphere opacity to 0
        if (transitionSphere != null && transitionMaterial != null)
        {
            SetTransitionSphereOpacity(0f);
            transitionSphere.SetActive(false);
        }
        else
        {
            Debug.LogError("Transition Sphere or Transition Material is not assigned.");
            yield break;
        }

        while (elapsedTime < rotationDuration)
        {
            float deltaTime = Time.deltaTime;
            elapsedTime += deltaTime;

            // Calculate current rotation speed
            float currentSpeed = Mathf.Lerp(initialSpeed, finalSpeed, elapsedTime / rotationDuration);

            // Rotate the sphere
            sphere0.transform.Rotate(Vector3.up, currentSpeed * deltaTime, Space.World);

            // Check if it's time to start the transition
            if (!hasTriggeredTransition && elapsedTime >= transitionStartTime)
            {
                hasTriggeredTransition = true;
                isFadingIn = true;
                transitionSphere.SetActive(true);
            }

            // Handle fading in
            if (isFadingIn)
            {
                transitionElapsed += deltaTime;
                float fadeInProgress = Mathf.Clamp01(transitionElapsed / transitionDuration);
                SetTransitionSphereOpacity(Mathf.Lerp(0f, 1f, fadeInProgress));

                if (transitionElapsed >= transitionDuration)
                {
                    isFadingIn = false;
                    fadedIn = true;
                }
            }
            else if (fadedIn)
            {
                ActivateSphere(sphere1);
                currentSphere = sphere1;
                break;
            }

            yield return null;
        }
        yield return StartCoroutine(FadeTransitionSphere(1f, 0f));
        if (transitionSphere != null)
        {
            transitionSphere.SetActive(false);
        }

        isTransitioning = false;
    }
}

