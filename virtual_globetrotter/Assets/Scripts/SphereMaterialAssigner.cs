using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class SphereMaterialAssigner : MonoBehaviour
{
    // Egypt Materials
    public Material egyptSunnyMaterial;
    public Material egyptRainyMaterial;
    public Material egyptSnowyMaterial;

    // San Francisco Materials
    public Material sanFranciscoSunnyMaterial;
    public Material sanFranciscoRainyMaterial;
    public Material sanFranciscoSnowyMaterial;

    // Twin Towers Materials
    public Material twinTowersSunnyMaterial;
    public Material twinTowersRainyMaterial;
    public Material twinTowersSnowyMaterial;

    // Eiffel Tower Materials
    public Material eiffelSunnyMaterial;
    public Material eiffelRainyMaterial;
    public Material eiffelSnowyMaterial;

    // Times Square Materials
    public Material timeSquareSunnyMaterial;
    public Material timeSquareRainyMaterial;
    public Material timeSquareSnowyMaterial;

    // Weather effects
    // public GameObject rainEffect;
    // public GameObject snowEffect;

    // Material transition duration
    public float transitionDuration = 2.0f;

    void Start()
    {
        // Get the current scene name and weather
        string sceneName = SceneManager.GetActiveScene().name;
        string weather = GetWeatherFromAI();

        // Find the ImageSphere GameObject
        GameObject imageSphere = GameObject.Find("ImageSphere");
        if (imageSphere != null)
        {
            Renderer sphereRenderer = imageSphere.GetComponent<Renderer>();

            // Assign materials based on the scene and weather
            switch (sceneName)
            {
                case "EgyptSunnyScene":
                    AssignWeatherMaterial(sphereRenderer, weather, egyptSunnyMaterial, egyptRainyMaterial, egyptSnowyMaterial);
                    break;
                case "EgyptRainScene":
                    AssignWeatherMaterial(sphereRenderer, weather, egyptSunnyMaterial, egyptRainyMaterial, egyptSnowyMaterial);
                    break;

                case "SanFranciscoSunnyScene":
                 AssignWeatherMaterial(sphereRenderer, weather, eiffelSunnyMaterial, eiffelRainyMaterial, eiffelSnowyMaterial);
                    break;

                case "SanFranciscoRainScene":
                    AssignWeatherMaterial(sphereRenderer, weather, sanFranciscoSunnyMaterial, sanFranciscoRainyMaterial, sanFranciscoSnowyMaterial);
                    break;

                case "TwinTowersScene":
                    AssignWeatherMaterial(sphereRenderer, weather, twinTowersSunnyMaterial, twinTowersRainyMaterial, twinTowersSnowyMaterial);
                    break;

                case "EiffelTowerSunnyScene":
                 AssignWeatherMaterial(sphereRenderer, weather, eiffelSunnyMaterial, eiffelRainyMaterial, eiffelSnowyMaterial);
                    break;

                case "EiffelTowerSnowyScene":
                    AssignWeatherMaterial(sphereRenderer, weather, eiffelSunnyMaterial, eiffelRainyMaterial, eiffelSnowyMaterial);
                    break;

                case "TimesSquareSunnyScene":
                    AssignWeatherMaterial(sphereRenderer, weather, timeSquareSunnyMaterial, timeSquareRainyMaterial, timeSquareSnowyMaterial);
                    break;

                default:
                    Debug.LogWarning("No materials assigned for this scene: " + sceneName);
                    break;
            }
        }
        else
        {
            Debug.LogError("ImageSphere not found in the scene!");
        }
    }

    void AssignWeatherMaterial(Renderer sphereRenderer, string weather, Material sunny, Material rainy, Material snowy)
    {
        // Select the appropriate material
        Material targetMaterial = null;
        switch (weather)
        {
            case "sunny":
                targetMaterial = sunny;
                // rainEffect?.SetActive(false);
                // snowEffect?.SetActive(false);
                break;

            case "rainy":
                targetMaterial = rainy;
                // rainEffect?.SetActive(true);
                // snowEffect?.SetActive(false);
                break;

            case "snowy":
                targetMaterial = snowy;
                // rainEffect?.SetActive(false);
                // snowEffect?.SetActive(true);
                break;

            default:
                Debug.LogWarning("Unknown weather type: " + weather);
                return;
        }

        // Smoothly transition to the target material
        StartCoroutine(FadeMaterial(sphereRenderer, targetMaterial));
    }
     // GameObjects for weather effects
    // public GameObject rainEffect;  // Temporarily disable rainEffect
    public GameObject snowEffect; // Drag your SnowEffect prefab here in the Inspector (optional)

    // The ImageSphere Renderer
    public Renderer sphereRenderer; // Drag the ImageSphere's Renderer here in the Inspector

    // Materials for each weather type
    public Material sunnyMaterial;
    public Material rainyMaterial;
    public Material snowyMaterial;

    // Method to change the weather
    public void ChangeWeather(string weather)
    {
        // Disable all weather effects initially
        // if (rainEffect != null) rainEffect.SetActive(false); // Temporarily comment out rainEffect
        if (snowEffect != null) snowEffect.SetActive(false);

        // Handle weather changes
        switch (weather.ToLower())
        {
            case "sunny":
                StartCoroutine(FadeMaterial(sphereRenderer, sunnyMaterial));
                // sphereRenderer.material = sunnyMaterial; // Set the sphere material to sunny
                Debug.Log("Weather changed to Sunny.");
                break;

            case "rainy":
                StartCoroutine(FadeMaterial(sphereRenderer, rainyMaterial));
                // sphereRenderer.material = rainyMaterial; // Set the sphere material to rainy
                // if (rainEffect != null) rainEffect.SetActive(true); // Temporarily comment out rainEffect
                Debug.Log("Weather changed to Rainy.");
                break;

            case "snowy":
                StartCoroutine(FadeMaterial(sphereRenderer, snowyMaterial));
                // sphereRenderer.material = snowyMaterial; // Set the sphere material to snowy
                if (snowEffect != null) snowEffect.SetActive(true); // Enable snow particles
                Debug.Log("Weather changed to Snowy.");
                break;

            default:
                Debug.LogWarning("Unknown weather type: " + weather);
                break;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ChangeWeather("sunny"); // Change to sunny weather
            Console.WriteLine('S');
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeWeather("rainy"); // Change to rainy weather
            Console.WriteLine('R');
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeWeather("snowy"); // Change to snowy weather
            Console.WriteLine('W');
        }
    }

    // Coroutine to blend materials over time
    IEnumerator FadeMaterial(Renderer sphereRenderer, Material targetMaterial)
    {
        Material currentMaterial = sphereRenderer.material;
        float elapsed = 0;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);

            // Lerp between materials (this works well if the materials share similar shaders/textures)
            sphereRenderer.material.Lerp(currentMaterial, targetMaterial, t);
            yield return null;
        }

        // Ensure the final material is set
        sphereRenderer.material = targetMaterial;
    }

    string GetWeatherFromAI()
    {
        // Placeholder for AI integration: Replace this with real weather data from AI or other sources
        return "sunny"; // Example: default to "sunny"
    }
}
