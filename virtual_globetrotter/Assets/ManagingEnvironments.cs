using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using NUnit.Framework.Constraints;

public class ManagingEnvironments : MonoBehaviour
{
    // Assign the spheres in the Inspector
    [Header("Environment Spheres")]
    public GameObject sphere0; 
    public GameObject sphere1; 
    public GameObject sphere1_night; 
    public GameObject sphere2;  
    public GameObject sphere2_night;  
    public GameObject sphere3; 
    public GameObject sphere3_night; 
    public GameObject sphere4; 
    public GameObject sphere4_night; 
    public GameObject sphere4_snowy; 
    public GameObject sphere5;  
    public GameObject sphere5_night;
    public GameObject sphere5_snowy;
    public GameObject sphere6;  
    public GameObject sphere6_night;  
    public GameObject sphere6_snowy;  
    public GameObject sphere7;  
    public GameObject sphere8;  
    public GameObject sphere8_night;  
    public GameObject sphere9;
    public GameObject sphere9_night;
    public GameObject sphere10; 
    public GameObject sphere10_night; 
    public GameObject sphere10_rain; 

    [Header("WorldMap Settings")]
    public GameObject image; 
    private Image imgComponent;
    private Color originalColor;
    public GameObject button; 
    
    [Header("Transition Settings")]
    public GameObject transitionSphere;    // Sphere for transitions
    public GameObject transitionSound;
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
            string location = "Space";
            TCPServer.Instance.SendMessageToAll(location);
            Debug.Log($"[MessageSender] Sent to clients: {location}");

            StartCoroutine(SwitchSphere(sphere0));

            sphere0.transform.eulerAngles = Vector3.zero;

            image.SetActive(true);
            Image imgComponent = image.GetComponent<Image>();
            Color originalColor = imgComponent.color;
            imgComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 255f);

            button.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentSphere == sphere10_night || currentSphere == sphere10)
            {
                StartCoroutine(SwitchSphere(sphere10_rain));
            }
        }
    }

    public void SwitchToSnow()
    {
        if (currentSphere == sphere4_night || currentSphere == sphere4)
        {
            StartCoroutine(SwitchSphere(sphere4_snowy));
        }
        if (currentSphere == sphere5_night || currentSphere == sphere5)
        {
            StartCoroutine(SwitchSphere(sphere5_snowy));
        }
        if (currentSphere == sphere6_night || currentSphere == sphere6)
        {
            StartCoroutine(SwitchSphere(sphere6_snowy));
        }
    }

    public void SwitchToNight()
    {
        if (currentSphere == sphere1)
        {
            StartCoroutine(SwitchSphere(sphere1_night));
        }
        if (currentSphere == sphere2)
        {
            StartCoroutine(SwitchSphere(sphere2_night));
        }
        if (currentSphere == sphere3)
        {
            StartCoroutine(SwitchSphere(sphere3_night));
        }
        if (currentSphere == sphere4 || currentSphere == sphere4_snowy)
        {
            StartCoroutine(SwitchSphere(sphere4_night));
        }
        if (currentSphere == sphere5 || currentSphere == sphere5_snowy)
        {
            StartCoroutine(SwitchSphere(sphere5_night));
        }
        if (currentSphere == sphere6 || currentSphere == sphere6_snowy)
        {
            StartCoroutine(SwitchSphere(sphere6_night));
        }
        if (currentSphere == sphere8)
        {
            StartCoroutine(SwitchSphere(sphere8_night));
        }
        if (currentSphere == sphere9)
        {
            StartCoroutine(SwitchSphere(sphere9_night));
        }
        if (currentSphere == sphere10 || currentSphere == sphere10_rain)
        {
            StartCoroutine(SwitchSphere(sphere10_night));
        }
    }

    public void SwitchToDay()
    {
        if (currentSphere == sphere1_night)
        {
            StartCoroutine(SwitchSphere(sphere1));
        }
        if (currentSphere == sphere2_night)
        {
            StartCoroutine(SwitchSphere(sphere2));
        }
        if (currentSphere == sphere3_night)
        {
            StartCoroutine(SwitchSphere(sphere3));
        }
        if (currentSphere == sphere4_night || currentSphere == sphere4_snowy)
        {
            StartCoroutine(SwitchSphere(sphere4));
        }
        if (currentSphere == sphere5_night || currentSphere == sphere5_snowy)
        {
            StartCoroutine(SwitchSphere(sphere5));
        }
        if (currentSphere == sphere6_night || currentSphere == sphere6_snowy)
        {
            StartCoroutine(SwitchSphere(sphere6));
        }
        if (currentSphere == sphere8_night)
        {
            StartCoroutine(SwitchSphere(sphere8));
        }
        if (currentSphere == sphere9_night)
        {
            StartCoroutine(SwitchSphere(sphere9));
        }
        if (currentSphere == sphere10_night || currentSphere == sphere10_rain)
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
        if (sphere1_night != null) sphere1_night.SetActive(false);
        if (sphere2 != null) sphere2.SetActive(false);
        if (sphere2_night != null) sphere2_night.SetActive(false);
        if (sphere3 != null) sphere3.SetActive(false);
        if (sphere3_night != null) sphere3_night.SetActive(false);
        if (sphere4 != null) sphere4.SetActive(false);
        if (sphere4_night != null) sphere4_night.SetActive(false);
        if (sphere4_snowy != null) sphere4_snowy.SetActive(false);
        if (sphere5 != null) sphere5.SetActive(false);
        if (sphere5_night != null) sphere5_night.SetActive(false);
        if (sphere5_snowy != null) sphere5_snowy.SetActive(false);
        if (sphere6 != null) sphere6.SetActive(false);
        if (sphere6_night != null) sphere6_night.SetActive(false);
        if (sphere6_snowy != null) sphere6_snowy.SetActive(false);
        if (sphere7 != null) sphere7.SetActive(false);
        if (sphere8 != null) sphere8.SetActive(false);
        if (sphere8_night != null) sphere8_night.SetActive(false);
        if (sphere9 != null) sphere9.SetActive(false);
        if (sphere9_night != null) sphere9_night.SetActive(false);
        if (sphere10 != null) sphere10.SetActive(false);
        if (sphere10_night != null) sphere10_night.SetActive(false);
        if (sphere10_rain != null) sphere10_rain.SetActive(false);

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
            string location = "Egypt, Pyramids";
            TCPServer.Instance.SendMessageToAll(location);
            Debug.Log($"[MessageSender] Sent to clients: {location}");
            StartCoroutine(FadeOutImageCoroutine(sphere1));
        }
        else
        {
            Debug.LogWarning("Cannot initiate fade-out and switch: A transition is already in progress.");
        }
    }
    public void SwitchToSphere2()
    {
        if (!isTransitioning)
        {
            string location = "India, Taj Mahal";
            TCPServer.Instance.SendMessageToAll(location);
            Debug.Log($"[MessageSender] Sent to clients: {location}");
            StartCoroutine(FadeOutImageCoroutine(sphere2));
        }
        else
        {
            Debug.LogWarning("Cannot initiate fade-out and switch: A transition is already in progress.");
        }
    }
    public void SwitchToSphere3()
    {
        if (!isTransitioning)
        {
            string location = "Mount Everest";
            TCPServer.Instance.SendMessageToAll(location);
            Debug.Log($"[MessageSender] Sent to clients: {location}");
            StartCoroutine(FadeOutImageCoroutine(sphere3));
        }
        else
        {
            Debug.LogWarning("Cannot initiate fade-out and switch: A transition is already in progress.");
        }
    }
    public void SwitchToSphere4()
    {
        if (!isTransitioning)
        {
            string location = "Japan, Temple";
            TCPServer.Instance.SendMessageToAll(location);
            Debug.Log($"[MessageSender] Sent to clients: {location}");
            StartCoroutine(FadeOutImageCoroutine(sphere4));
        }
        else
        {
            Debug.LogWarning("Cannot initiate fade-out and switch: A transition is already in progress.");
        }
    }
    public void SwitchToSphere5()
    {
        if (!isTransitioning)
        {
            string location = "China, The Great Wall of China";
            TCPServer.Instance.SendMessageToAll(location);
            Debug.Log($"[MessageSender] Sent to clients: {location}");
            StartCoroutine(FadeOutImageCoroutine(sphere5));
        }
        else
        {
            Debug.LogWarning("Cannot initiate fade-out and switch: A transition is already in progress.");
        }
    }
    public void SwitchToSphere6()
    {
        if (!isTransitioning)
        {
            string location = "Paris, Eiffel Tower";
            TCPServer.Instance.SendMessageToAll(location);
            Debug.Log($"[MessageSender] Sent to clients: {location}");
            StartCoroutine(FadeOutImageCoroutine(sphere6));
        }
        else
        {
            Debug.LogWarning("Cannot initiate fade-out and switch: A transition is already in progress.");
        }
    }
    public void SwitchToSphere7()
    {
        if (!isTransitioning)
        {
            StartCoroutine(FadeOutImageCoroutine(sphere7));
        }
        else
        {
            Debug.LogWarning("Cannot initiate fade-out and switch: A transition is already in progress.");
        }
    }
    public void SwitchToSphere8()
    {
        if (!isTransitioning)
        {
            StartCoroutine(FadeOutImageCoroutine(sphere8));
        }
        else
        {
            Debug.LogWarning("Cannot initiate fade-out and switch: A transition is already in progress.");
        }
    }
    public void SwitchToSphere9()
    {
        if (!isTransitioning)
        {
            StartCoroutine(FadeOutImageCoroutine(sphere9));
        }
        else
        {
            Debug.LogWarning("Cannot initiate fade-out and switch: A transition is already in progress.");
        }
    }
    public void SwitchToSphere10()
    {
        if (!isTransitioning)
        {
            StartCoroutine(FadeOutImageCoroutine(sphere10));
        }
        else
        {
            Debug.LogWarning("Cannot initiate fade-out and switch: A transition is already in progress.");
        }
    }


    IEnumerator FadeOutImageCoroutine(GameObject newSphere)
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
            StartCoroutine(RotateSphere0Coroutine(newSphere));
        }
        else
        {
            Debug.LogError("Sphere0 is not assigned.");
        }
    }

    IEnumerator RotateSphere0Coroutine(GameObject newSphere)
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

        transitionSound.SetActive(true);
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
                ActivateSphere(newSphere);
                currentSphere = newSphere;
                break;
            }

            yield return null;
        }
        transitionSound.SetActive(false);
        yield return StartCoroutine(FadeTransitionSphere(1f, 0f));
        if (transitionSphere != null)
        {
            transitionSphere.SetActive(false);
        }

        isTransitioning = false;
    }
}

